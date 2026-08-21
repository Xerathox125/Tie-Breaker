using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthHandler : MonoBehaviour
{
    public int maxHealth;
    [SerializeField] private int currentHealth;

    private HealthUI healthUI; // Referencia al script de la UI

    private void Awake()
    {
        currentHealth = maxHealth;

        // Buscamos automáticamente el componente de UI en la escena
        healthUI = FindFirstObjectByType<HealthUI>();
    }

    private void Start()
    {
        // Actualizamos la barra al iniciar la escena para que refleje la vida máxima
        if (healthUI != null)
        {
            healthUI.UpdateHealthBar(currentHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Actualizamos la barra de vida cada vez que recibe daño
        if (healthUI != null)
        {
            healthUI.UpdateHealthBar(currentHealth);
        }

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}