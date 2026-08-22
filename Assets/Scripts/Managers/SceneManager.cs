using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    private static HashSet<string> completedLevels = new HashSet<string>();
    private static int pendingCinematics = 0;
    private static bool hasShownContext = false;

    [SerializeField] private int totalLevelsToComplete = 2;

    [Header("Referencias del Botón Final")]
    [SerializeField] private Button finalLevelButton;

    [Header("Panel de Contexto Inicial")]
    [SerializeField] private GameObject contextPanel;

    [Header("Paneles de Lore (Ordenados)")]
    [SerializeField] private List<GameObject> lorePanelsList;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LevelSelection")
        {
            UpdateLevelButtons();
            ManageInitialContext();
        }
    }

    private void UpdateLevelButtons()
    {
        Debug.Log("Actualizando botones. Niveles completados en memoria: " + completedLevels.Count);
        foreach (string levelName in completedLevels)
        {
            string buttonName = "btnLvl" + levelName.Replace("Level", "");
            GameObject btnObj = GameObject.Find(buttonName);
            if (btnObj != null)
            {
                btnObj.GetComponent<Button>().interactable = false;
                Debug.Log("Botón desactivado: " + buttonName);
            }
        }

        if (finalLevelButton != null)
        {
            bool shouldUnlock = (completedLevels.Count >= totalLevelsToComplete);
            finalLevelButton.interactable = shouldUnlock;
            Debug.Log("Botón final interactuable: " + shouldUnlock);
        }
    }

    private void ManageInitialContext()
    {
        Debug.Log("Gestionando contexto inicial. hasShownContext = " + hasShownContext);

        if (!hasShownContext && contextPanel != null)
        {
            Debug.Log("Mostrando panel de Contexto Inicial por primera vez.");
            contextPanel.SetActive(true);
            hasShownContext = true;
        }
        else
        {
            if (contextPanel != null) contextPanel.SetActive(false);
            CheckAndShowCinematic();
        }
    }

    public static void MarkLevelCompleted(string levelName)
    {
        if (!completedLevels.Contains(levelName))
        {
            completedLevels.Add(levelName);
            pendingCinematics++;
            Debug.Log(">>> NIVEL COMPLETADO: " + levelName + " | Total completados: " + completedLevels.Count + " | Cinemáticas pendientes: " + pendingCinematics);
        }
        else
        {
            Debug.Log(">>> El nivel " + levelName + " ya estaba registrado como completado.");
        }
    }

    private void CheckAndShowCinematic()
    {
        Debug.Log("Comprobando cinemáticas. Pendientes: " + pendingCinematics);

        if (lorePanelsList == null || lorePanelsList.Count == 0)
        {
            Debug.LogWarning("¡La lista de paneles de lore está vacía o no asignada!");
            return;
        }

        if (pendingCinematics > 0)
        {
            // CORRECCIÓN: El índice correcto se basa en el total de niveles completados menos 1
            int currentLoreIndex = completedLevels.Count - 1;
            Debug.Log("Cálculo corregido de índice de Lore -> Total completados menos 1 = Índice: " + currentLoreIndex);

            if (currentLoreIndex >= 0 && currentLoreIndex < lorePanelsList.Count)
            {
                foreach (GameObject panel in lorePanelsList)
                {
                    if (panel != null) panel.SetActive(false);
                }

                GameObject activePanel = lorePanelsList[currentLoreIndex];
                if (activePanel != null)
                {
                    activePanel.SetActive(true);
                    Debug.Log("¡Panel de Lore activado con éxito! Nombre: " + activePanel.name);
                }
                else
                {
                    Debug.LogError("El panel de lore en el índice " + currentLoreIndex + " es NULL.");
                }
            }
            else
            {
                Debug.LogError("El índice calculado (" + currentLoreIndex + ") está fuera del rango de la lista de paneles (Tamaño: " + lorePanelsList.Count + ").");
            }
        }
        else
        {
            Debug.Log("No hay cinemáticas pendientes. Apagando todos los paneles de lore.");
            foreach (GameObject panel in lorePanelsList)
            {
                if (panel != null) panel.SetActive(false);
            }
        }
    }

    public void CloseContextPanel()
    {
        Debug.Log("Cerrando panel de Contexto Inicial.");
        if (contextPanel != null)
        {
            contextPanel.SetActive(false);
        }
        CheckAndShowCinematic();
    }

    public void CloseCinematicPanel()
    {
        Debug.Log("Cerrando panel de cinemática actual. Pendientes antes de restar: " + pendingCinematics);
        if (pendingCinematics > 0)
        {
            pendingCinematics--;
        }
        Debug.Log("Pendientes después de restar: " + pendingCinematics);

        foreach (GameObject panel in lorePanelsList)
        {
            if (panel != null) panel.SetActive(false);
        }

        CheckAndShowCinematic();
    }

    // Añade este método estático dentro de tu ScenesManager.cs existente:

    public static string GetCurrentPlayerSkin()
    {
        int completedCount = completedLevels.Count;

        // Si ya completó 2 niveles, le toca el skin del 3er nivel
        if (completedCount >= 2)
        {
            return "Skin3";
        }
        // Si ya completó 1 nivel, le toca el skin del 2do nivel
        else if (completedCount == 1)
        {
            return "Skin2";
        }

        // Por defecto para el inicio (primer nivel que juegue)
        return "Skin1";
    }

    // Métodos de navegación de escenas
    public void CloseApp() { Application.Quit(); }
    public void ResumeGame() { }
    public void PauseGame() { Time.timeScale = 0; }
    public void MainMenu() { SceneManager.LoadScene("MainMenu"); }
    public void LevelSelection() { SceneManager.LoadScene("LevelSelection"); }
    public void LoadLevel1() { SceneManager.LoadScene("Level1"); }
    public void LoadLevel2() { SceneManager.LoadScene("Level2"); }
    public void FinalLevel() { SceneManager.LoadScene("FinalLevel"); }
}