using System.Collections;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [Header("HurtBox")]
    public int damageEnemy;
    public float knockBackForce;
    public float timeKnockBack;

    public LayerMask targetLayer;

    [Header("Collider Source (Opcional para Jefes)")]
    [Tooltip("Si se activa, el área de daño leerá el BoxCollider2D animado en lugar de las variables manuales.")]
    public bool useAttachedCollider = false;
    public BoxCollider2D customCollider;

    [Header("Manual Box Settings (Enemigos normales)")]
    public Vector2 hurtBoxSize;
    public Vector2 hurtBoxOffSet;


    // CoolDown
    public float damageCoolDown;
    private float coolDownTimer;

    private void Awake()
    {
        // Si activaste la casilla pero no asignaste el collider, intenta buscarlo en el mismo GameObject
        if (useAttachedCollider && customCollider == null)
        {
            customCollider = GetComponent<BoxCollider2D>();
        }
    }


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
        GetBoxParameters(out Vector2 center, out Vector2 size);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, targetLayer);

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



    // Calcula posición y tamaño dependiendo de la configuración
    private void GetBoxParameters(out Vector2 center, out Vector2 size)
    {
        if (useAttachedCollider && customCollider != null)
        {
            // Lee directamente las dimensiones y el offset transformado por la animación y el localScale
            center = customCollider.bounds.center;
            size = customCollider.bounds.size;
        }
        else
        {
            // Modo manual: aplica la dirección según la escala del objeto (para que los enemigos normales también volteen el offset)
            float facingSign = Mathf.Sign(transform.localScale.x);
            center = (Vector2)transform.position + new Vector2(hurtBoxOffSet.x * facingSign, hurtBoxOffSet.y);
            size = hurtBoxSize;
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        GetBoxParameters(out Vector2 center, out Vector2 size);
        Gizmos.DrawWireCube(center, size);
    }
}