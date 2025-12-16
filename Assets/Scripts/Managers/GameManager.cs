using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Configurações de UI")]
    public GameObject pauseMenuPanel; 
    public GameObject settingsPanel;
    
    [Header("Configurações de Cena")]
    public string mainMenuSceneName = "MainMenuScene"; 

    
    public static bool isPaused = false;

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; 
        
        if (pauseMenuPanel != null) 
            pauseMenuPanel.SetActive(true); 
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; 
        
        if (pauseMenuPanel != null){
            pauseMenuPanel.SetActive(false); 
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; 
        isPaused = false;
        
        SceneManager.LoadScene(mainMenuSceneName);
    }
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}