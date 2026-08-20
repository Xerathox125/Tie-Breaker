using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ScenesManager : MonoBehaviour
{
    private static HashSet<string> completedLevels = new HashSet<string>();

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
        // Recorremos los niveles y desactivamos los que ya estén en la lista de la sesión actual
        foreach (string levelName in completedLevels)
        {
            string buttonName = "btnLvl" + levelName.Replace("Level", "");
            GameObject btnObj = GameObject.Find(buttonName);

            if (btnObj != null)
            {
                Button btn = btnObj.GetComponent<Button>();
                if (btn != null)
                {
                    btn.interactable = false;
                }
            }
        }
    }

    // Método para registrar el nivel completado solo en memoria
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
}
