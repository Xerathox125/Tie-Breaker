using System.Collections;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [Header("HurtBox")]
    public int damageEnemy;
    public float knockBackForce;
    public float timeKnockBack;
    public Vector2 hurtBoxSize;
    public Vector2 hurtBoxOffSet;
    public LayerMask targetLayer;

    // CoolDown
    public float damageCoolDown;
    private float coolDownTimer;

    void Update()
    {
        if (coolDownTimer > 0)
        {
            coolDownTimer -= Time.deltaTime;
            return;
        }

        CheckHurtBox();
    }

    private void CheckHurtBox()
    {
        // Cambiamos transform.position por transform.parent.position para que lea al padre (Bee Enemy)
        Vector2 center = (Vector2)transform.parent.position + hurtBoxOffSet;
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, hurtBoxSize, 0f, targetLayer);

        foreach (Collider2D hit in hits)
        {
            Damageable damage = hit.GetComponent<Damageable>();
            if (damage != null)
            {
                Vector2 contactPoint = hit.ClosestPoint(center);
                if ((contactPoint - center).sqrMagnitude > 0.0001f)
                {
                    contactPoint = center;
                }
                damage.ApplyDamage(damageEnemy, contactPoint, knockBackForce);
            }

            PlayerMovement playerMovement = hit.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.ApplyKnockBack(timeKnockBack);
            }

            coolDownTimer = damageCoolDown;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 center = (Vector2)transform.position + hurtBoxOffSet;
        Gizmos.DrawWireCube(center, hurtBoxSize);
    }
}