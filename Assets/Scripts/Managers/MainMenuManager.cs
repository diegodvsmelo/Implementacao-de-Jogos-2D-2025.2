using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Configuração de Cenas")]
    [Tooltip("Nome EXATO da cena de gameplay")]
    public string gameSceneName = "GameScene";

    [Header("Painéis da Interface")]
    public GameObject mainMenuPanel;    
    public GameObject settingsPanel;   
    

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        ShowMainMenu();
    }
    

    // --- FUNÇÕES DOS BOTÕES ---

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Saindo do Jogo...");
        Application.Quit();
    }

    private void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }
}