using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthHandler : MonoBehaviour
{
    public int maxHealth;
    [SerializeField] private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
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

        //Destroy(gameObject);
    }
   
}
