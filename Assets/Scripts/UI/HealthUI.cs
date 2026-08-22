using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Sprite[] healthSprites;

    public void UpdateHealthBar(int currentHealth)
    {
        if (healthBarImage == null || healthSprites == null || healthSprites.Length == 0) return;

        // Escudo de seguridad para ignorar números absurdos
        if (currentHealth > healthSprites.Length - 1) return;

        int index = Mathf.Clamp(currentHealth, 0, healthSprites.Length - 1);
        healthBarImage.sprite = healthSprites[index];
    }
}