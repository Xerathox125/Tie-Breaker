using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private LayerMask collisionLayers;
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackForce = 3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Revisa el tag del objeto o de su padre/raíz por si chocó contra un hijo (HurtBox, etc.)
        bool isPlayer = collision.CompareTag("Player") ||
                       (collision.attachedRigidbody != null && collision.attachedRigidbody.CompareTag("Player"));

        if (isPlayer)
        {
            Debug.Log("¡Detectó al Player!");

            // Aplicar daño si tiene el componente Damageable en la jerarquía[cite: 1]
            if (collision.GetComponentInParent<Damageable>() != null)
            {
                collision.GetComponentInParent<Damageable>().ApplyDamage(damage, transform.position, knockbackForce);
            }

            Destroy(gameObject);
        }
    }
}
