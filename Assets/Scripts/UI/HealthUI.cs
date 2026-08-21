using UnityEngine;
using UnityEngine.UI; // Necesario para trabajar con UI

public class HealthUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image healthBarImage; // Componente Image de la UI donde se muestra la barra
    [SerializeField] private Sprite[] healthSprites; // Arreglo con tus sprites cortados (HPBar_0, HPBar_1, etc.)

    // Método para actualizar el sprite según la salud actual
    public void UpdateHealthBar(int currentHealth)
    {
        if (healthBarImage == null || healthSprites == null || healthSprites.Length == 0) return;

        // Asegurarnos de que el índice esté dentro de los límites del arreglo
        int index = Mathf.Clamp(currentHealth, 0, healthSprites.Length - 1);

        // Cambiamos el sprite de la UI
        healthBarImage.sprite = healthSprites[index];
    }
}