using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthHandler : MonoBehaviour
{
    public int maxHealth = 5; // Asignamos 5 por defecto para evitar que inicie en 0
    [SerializeField] private int currentHealth;

    private HealthUI healthUI; // Referencia al script de la UI
    private bool isPlayer;


    [Header("Transición al morir (Opcional para Jefes)")]
    [SerializeField] private bool loadSceneOnDeath = false;
    [SerializeField] private string sceneToLoad = "MainMenu";
    [SerializeField] private float delayBeforeLoad = 3f;



    private void Awake()
    {
        currentHealth = maxHealth;
        isPlayer  = gameObject.CompareTag("Player");

        if(isPlayer)
        {
            // Buscamos automáticamente el componente de UI en la escena
            healthUI = FindFirstObjectByType<HealthUI>();
        }

        // Buscamos automáticamente el componente de UI en la escena
        //healthUI = FindFirstObjectByType<HealthUI>();
    }

    private void Start()
    {
        // Actualizamos la barra al iniciar la escena para que refleje la vida máxima
        if (isPlayer && healthUI != null)
        {
            healthUI.UpdateHealthBar(currentHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Actualizamos la barra de vida cada vez que recibe daño
        if (isPlayer && healthUI != null)
        {
            healthUI.UpdateHealthBar(currentHealth);
        }

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Time.timeScale = 1f;

        if (!isPlayer)
        {
            // Desactivar colliders para evitar más interacciones durante la muerte
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            // Si está configurado para cambiar de escena (Caso del Boss)
            if (loadSceneOnDeath)
            {
                // Guarda el progreso completado
                ScenesManager.MarkLevelCompleted(gameObject.scene.name);

                // Oculta el Sprite pero mantiene el objeto vivo brevemente para ejecutar la corrutina
                SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
                if (sr != null) sr.enabled = false;

                StartCoroutine(LoadSceneRoutine());
            }
            else
            {
                Destroy(gameObject);
            }
            return;
        }

        // Si muere el jugador
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    private IEnumerator LoadSceneRoutine()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(sceneToLoad);
    }


}