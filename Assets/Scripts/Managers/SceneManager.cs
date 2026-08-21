using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    private static HashSet<string> completedLevels = new HashSet<string>();
    private static int pendingCinematics = 0;

    // NUEVO: Bandera para saber si el contexto inicial ya fue mostrado en esta sesión
    private static bool hasShownContext = false;

    [SerializeField] private int totalLevelsToComplete = 2;

    [Header("Referencias del Botón Final")]
    [SerializeField] private Button finalLevelButton;

    [Header("Panel de Contexto Inicial")]
    [SerializeField] private GameObject contextPanel; // Arrastra aquí tu objeto "Contexto"

    [Header("Paneles de Lore (Ordenados)")]
    [SerializeField] private List<GameObject> lorePanelsList; // [0] Lore1, [1] Lore2, [2] Lore Final Boss

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
            ManageInitialContext(); // Controla si debe mostrarse el contexto o el lore
        }
    }

    private void UpdateLevelButtons()
    {
        foreach (string levelName in completedLevels)
        {
            string buttonName = "btnLvl" + levelName.Replace("Level", "");
            GameObject btnObj = GameObject.Find(buttonName);
            if (btnObj != null)
            {
                btnObj.GetComponent<Button>().interactable = false;
            }
        }

        if (finalLevelButton != null)
        {
            bool shouldUnlock = (completedLevels.Count >= totalLevelsToComplete);
            finalLevelButton.interactable = shouldUnlock;
        }
    }

    private void ManageInitialContext()
    {
        // 1. Si el contexto NO se ha visto todavía y el objeto está asignado
        if (!hasShownContext && contextPanel != null)
        {
            contextPanel.SetActive(true);
            hasShownContext = true; // Marcamos que ya se mostró para que no se repita
        }
        else
        {
            // Si ya se vio el contexto, nos aseguramos de que esté apagado y pasamos a revisar los lores de niveles
            if (contextPanel != null) contextPanel.SetActive(false);
            CheckAndShowCinematic();
        }
    }

    // Se llama automáticamente desde LevelTransition al ganar un nivel
    public static void MarkLevelCompleted(string levelName)
    {
        if (!completedLevels.Contains(levelName))
        {
            completedLevels.Add(levelName);
            pendingCinematics++;
        }
    }

    private void CheckAndShowCinematic()
    {
        if (lorePanelsList == null || lorePanelsList.Count == 0) return;

        if (pendingCinematics > 0)
        {
            int currentLoreIndex = completedLevels.Count - pendingCinematics;

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
                }
            }
        }
        else
        {
            foreach (GameObject panel in lorePanelsList)
            {
                if (panel != null) panel.SetActive(false);
            }
        }
    }

    // Método para el botón "Cerrar" del Contexto Inicial
    public void CloseContextPanel()
    {
        if (contextPanel != null)
        {
            contextPanel.SetActive(false);
        }
        Time.timeScale = 1; // Reanuda el juego

        // Por si acaso completó algún nivel antes de ver el contexto (caso raro, pero seguro)
        CheckAndShowCinematic();
    }

    // Método para el botón "Cerrar" de los paneles de Lore de niveles
    public void CloseCinematicPanel()
    {
        if (pendingCinematics > 0)
        {
            pendingCinematics--;
        }

        foreach (GameObject panel in lorePanelsList)
        {
            if (panel != null) panel.SetActive(false);
        }

        Time.timeScale = 1;
        CheckAndShowCinematic();
    }

    public void CloseApp() { Application.Quit(); }
    public void ResumeGame() { Time.timeScale = 1; }
    public void PauseGame() { Time.timeScale = 0; }
    public void MainMenu() { SceneManager.LoadScene("MainMenu"); Time.timeScale = 1; }
    public void LevelSelection() { SceneManager.LoadScene("LevelSelection"); Time.timeScale = 1; }
    public void LoadLevel1() { SceneManager.LoadScene("Level1"); }
    public void LoadLevel2() { SceneManager.LoadScene("Level2"); }
    public void FinalLevel() { SceneManager.LoadScene("FinalLevel"); }
}