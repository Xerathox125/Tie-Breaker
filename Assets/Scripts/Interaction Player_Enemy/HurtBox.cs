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
        // SEGURIDAD: Si tiene padre usa la posición del padre, si no, usa su propia posición
        Vector2 originPosition = (transform.parent != null) ? (Vector2)transform.parent.position : (Vector2)transform.position;
        Vector2 center = originPosition + hurtBoxOffSet;

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
        // Hacemos lo mismo en los Gizmos para que la caja roja se dibuje bien en ambos casos
        Vector2 originPosition = (transform.parent != null) ? (Vector2)transform.parent.position : (Vector2)transform.position;
        Vector2 center = originPosition + hurtBoxOffSet;
        Gizmos.DrawWireCube(center, hurtBoxSize);
    }
}