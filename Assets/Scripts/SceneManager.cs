using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    private static HashSet<string> completedLevels = new HashSet<string>();

    [SerializeField] private int totalLevelsToComplete = 2;

    [Header("Referencias del Botón Final")]
    [SerializeField] private Button finalLevelButton; // Arrastra el botón aquí desde Unity

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
        }
    }

    private void UpdateLevelButtons()
    {
        Debug.Log("Niveles completados en memoria actualmente: " + completedLevels.Count);

        // 1. Recorremos los niveles y desactivamos los que ya estén en la lista
        foreach (string levelName in completedLevels)
        {
            string buttonName = "btnLvl" + levelName.Replace("Level", "");
            GameObject btnObj = GameObject.Find(buttonName);
            if (btnObj != null)
            {
                btnObj.GetComponent<Button>().interactable = false;
            }
        }

        // 2. Lógica para el botón final usando la referencia directa del Inspector
        if (finalLevelButton != null)
        {
            bool shouldUnlock = (completedLevels.Count >= totalLevelsToComplete);
            finalLevelButton.interactable = shouldUnlock;
            Debug.Log("Botón final actualizado. ¿Debería estar activado? " + shouldUnlock);
        }
        else
        {
            Debug.LogError("¡Falta asignar el botón final en el inspector del ScenesManager!");
        }
    }

    public static void MarkLevelCompleted(string levelName)
    {
        if (!completedLevels.Contains(levelName))
        {
            completedLevels.Add(levelName);
        }
    }

    public void CloseApp()
    {
        Application.Quit();
        Debug.Log("Application closed.");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }
    public void LevelSelection()
    {
        SceneManager.LoadScene("LevelSelection");
        Time.timeScale = 1;
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void FinalLevel()
    {
        SceneManager.LoadScene("FinalLevel");
    }
}