using System.Collections.Generic;
using UnityEngine;
 

public class BossController : MonoBehaviour
{
    public enum PatrolMode { Loop, PingPong }

    [Header("Patrol Settings")]
    [SerializeField] private List<Transform> patrolPoints = new List<Transform>();
    [SerializeField] private PatrolMode patrolMode = PatrolMode.PingPong;
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float waitTimeAtPoint = 1f;


    [Header("Box Detection & Attack")]
    [SerializeField] private Vector2 boxSize = new Vector2(4f, 3f);
    [SerializeField] private Vector2 boxOffset = new Vector2(2f, 0f);
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float attackCooldown = 2f;


    [Header("Shooter Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;


    private Rigidbody2D rb;
    private Animator animator;
 

    private int currentPointIndex = 0;
    private bool isReversing = false;
    private float waitTimer;
    private float lastAttackTime;
    private bool isWaiting;

    private bool isAttacking;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
 
    }

 

    // Update is called once per frame
    void FixedUpdate()
    {
        // 1. Si está ejecutando la animación de ataque, NO se mueve bajo ninguna circunstancia
        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            UpdateAnimationState(false);
            return;
        }

        // 2. Si no está atacando, evalúa la presencia del jugador
        bool playerDetected = CheckPlayerInBox();

        if (playerDetected)
        {
            rb.linearVelocity = Vector2.zero;
            UpdateAnimationState(false);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                TriggerAttack();
            }
        }
        else
        {
            PatrolLogic();
        }
    }
    private bool CheckPlayerInBox()
    {

        float facingSign = Mathf.Sign(transform.localScale.x);

        Vector2 boxCenter = (Vector2)transform.position + new Vector2(boxOffset.x * facingSign, boxOffset.y);
        Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, 0f, playerLayer);

        return hit != null && hit.CompareTag("Player");
    }
 
    private void PatrolLogic()
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            rb.linearVelocity = Vector2.zero;
            UpdateAnimationState(false);
            return;
        }

        Transform targetPoint = patrolPoints[currentPointIndex];
        if (targetPoint == null) return;

        if (isWaiting)
        {
            waitTimer += Time.fixedDeltaTime;
            rb.linearVelocity = Vector2.zero;
            UpdateAnimationState(false);

            if (waitTimer >= waitTimeAtPoint)
            {
                isWaiting = false;
                waitTimer = 0f;
                AdvanceToNextPoint();
            }
            return;
        }

        FollowTarget(targetPoint.position);
        UpdateAnimationState(true);
 
        if (Vector2.Distance(transform.position, targetPoint.position) < 1.7f)
        {
    
            isWaiting = true;
        }
    }
 
    private void AdvanceToNextPoint()
    {
        if (patrolPoints.Count <= 1) return;

        if (patrolMode == PatrolMode.Loop)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Count;
        }
        else if (patrolMode == PatrolMode.PingPong)
        {
            if (!isReversing)
            {
                currentPointIndex++;
                if (currentPointIndex >= patrolPoints.Count - 1) isReversing = true;
            }
            else
            {
                currentPointIndex--;
                if (currentPointIndex <= 0) isReversing = false;
            }
        }

    }
 
    private void FollowTarget(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * speed;

        if (Mathf.Abs(direction.x) > 0.05f)
        {
            bool isMovingLeft = direction.x < 0;
 
            Vector3 scale = transform.localScale;
            scale.x = isMovingLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    private void UpdateAnimationState(bool isMoving)
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
        }
    }

    private void TriggerAttack()
    {
        isAttacking = true; // Bloquea el movimiento
        lastAttackTime = Time.time;

        if (animator != null)
        {
            animator.SetTrigger("IsAttacking");
        }
    }
    
    public void ShootProjectile()
    {
        if (projectilePrefab == null) return;

        Transform spawnPoint = firePoint != null ? firePoint : transform;
        GameObject proj = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        Vector2 shootDirection = transform.localScale.x < 0 ? Vector2.left : Vector2.right;

        Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
        if (projRb != null)
        {
            projRb.linearVelocity = shootDirection * 8f;
        }
    }
 
    public void FinishAttack()
    {
        isAttacking = false;  
    }


    private void OnDrawGizmosSelected()
    {
        float facingSign = Mathf.Sign(transform.localScale.x);
        Vector3 boxCenter = transform.position + new Vector3(boxOffset.x * facingSign, boxOffset.y, 0f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(boxCenter, boxSize);

        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < patrolPoints.Count; i++)
            {
                if (patrolPoints[i] == null) continue;
                Gizmos.DrawSphere(patrolPoints[i].position, 0.2f);

                if (i < patrolPoints.Count - 1 && patrolPoints[i + 1] != null)
                {
                    Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                }
            }
        }
    }
}
