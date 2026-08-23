using System.Collections.Generic;
using UnityEngine;

public class BatAIController : MonoBehaviour
{
    public enum PatrolMode { Loop, PingPong }
    public enum EnemyBehaviorType { Chaser, Shooter }

    [Header("Enemy Type Settings")]
    [SerializeField] private EnemyBehaviorType behaviorType = EnemyBehaviorType.Chaser;

    [Header("Patrol Settings")]
    [SerializeField] private List<Transform> patrolPoints = new List<Transform>();
    [SerializeField] private PatrolMode patrolMode = PatrolMode.Loop;
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float waitTimeAtPoint = 1f;

    [Header("Detection & Attack")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float detectionRadius = 4f;
    [SerializeField] private float attackRadius = 1.2f;
    [SerializeField] private float attackCooldown = 2f;

    [Header("Shooter Settings (Solo para Shooter)")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;


    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private int currentPointIndex = 0;
    private bool isReversing = false; // Útil para el modo PingPong
    private float waitTimer;
    private float lastAttackTime;
    private bool isWaiting;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        // Buscar al Jugador automáticamente si no se asignó en el Inspector
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }
    }

    private void FixedUpdate()
    {
        bool isPlayerDetected = false;

        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= detectionRadius)
            {
                isPlayerDetected = true;

                switch (behaviorType)
                {
                    case EnemyBehaviorType.Chaser:
                        // Si está a rango de ataque melé
                        if (distanceToPlayer <= attackRadius)
                        {
                            rb.linearVelocity = Vector2.zero;
                            LookAt(playerTransform.position);
                            TriggerAttack();
                        }
                        else
                        {
                            // Perseguir al jugador
                            FollowTarget(playerTransform.position);
                        }
                        break;

                    case EnemyBehaviorType.Shooter:
                        // Se detiene, mira al jugador y dispara
                        rb.linearVelocity = Vector2.zero;
                        LookAt(playerTransform.position);
                        TriggerAttack();  
                        break;
                }
            }
        }

        // Si no se detectó al jugador o se perdió de vista, seguir patrullando
        if (!isPlayerDetected)
        {
            PatrolLogic();
        }
    }

    private void PatrolLogic()
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Transform targetPoint = patrolPoints[currentPointIndex];
        if (targetPoint == null) return;


        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            rb.linearVelocity = Vector2.zero;

            if (waitTimer >= waitTimeAtPoint)
            {
                isWaiting = false;
                waitTimer = 0f;
                AdvanceToNextPoint();
            }
            return;
        }

        FollowTarget(targetPoint.position);

        // Comprobar si llegó al punto actual
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.3f)
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
                if (currentPointIndex >= patrolPoints.Count - 1)
                {
                    isReversing = true;
                }
            }
            else
            {
                currentPointIndex--;
                if (currentPointIndex <= 0)
                {
                    isReversing = false;
                }
            }
        }
    }

    private void FollowTarget(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * speed;

        // Voltear el sprite en X según la dirección
        if (direction.x != 0 && spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
    }

    private void TriggerAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;

        if (animator != null)
        {
            animator.SetTrigger("IsAttacking");
        }

        // Instanciar el proyectil inmediatamente si es Shooter
        if (behaviorType == EnemyBehaviorType.Shooter)
        {
            ShootProjectile();
        }
    }

    private void LookAt(Vector2 targetPosition)
    {
        float directionX = targetPosition.x - transform.position.x;
        if (Mathf.Abs(directionX) > 0.05f && spriteRenderer != null)
        {
            spriteRenderer.flipX = directionX < 0;
        }
    }

    public void ShootProjectile()
    {
        if (projectilePrefab == null || playerTransform == null) return;

        Transform spawnPoint = firePoint != null ? firePoint : transform;
        GameObject proj = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        // Orientar o empujar el proyectil hacia la dirección del jugador
        Vector2 direction = (playerTransform.position - spawnPoint.position).normalized;

        Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
        if (projRb != null)
        {
            projRb.linearVelocity = direction * 8f; // Ajusta la velocidad del proyectil
        }
    }


    private void OnDrawGizmosSelected()
    {
        // Dibuja los rangos
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        // Dibuja la ruta de patrulla en la vista Scene
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

            // Conectar el último punto con el primero si es Loop
            if (patrolMode == PatrolMode.Loop && patrolPoints.Count > 1 && patrolPoints[0] != null)
            {
                Gizmos.DrawLine(patrolPoints[patrolPoints.Count - 1].position, patrolPoints[0].position);
            }
        }
    }

}
