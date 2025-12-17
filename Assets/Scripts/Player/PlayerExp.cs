using UnityEngine;

public class PlayerExp : MonoBehaviour
{
    [Header("UI References")]
    public GameObject upgradeCanvasUI;
    public GameObject skillLearnUI;
    public ExpBar expBar;

    [Header("Variáveis de Nível")]
    public float xpToLevelUp;
    public float currentXp;
    public int level;
    public float xpMultiplierIncrease;
    public UpgradeManager upgradeManager;
    public UpgradeManagerStats upgradeManagerStats;

    private void Start()
    {
        if (upgradeCanvasUI != null) upgradeCanvasUI.SetActive(false);
        if (skillLearnUI != null) skillLearnUI.SetActive(false);
        if (expBar != null)
        {
            expBar.SetMaxExp(xpToLevelUp);
            expBar.SetExp(currentXp);
        }
    }

    void Awake()
    {
        currentXp=0;
        level = 1;
    }

    public void AddExp(float amount)
    {
        currentXp += amount;

    if (expBar != null)
    {
        expBar.AddExp(amount);
    }

    while (currentXp >= xpToLevelUp)
    {
        currentXp -= xpToLevelUp;
        level++;
        xpToLevelUp *= xpMultiplierIncrease;

        if (expBar != null)
        {
            expBar.SetMaxExp(xpToLevelUp);
            expBar.SetExp(currentXp);
        }

        HandleLevelUp();
    }
    }

    public void HandleLevelUp()
    {
        if (upgradeManager != null) {

            // Regra: Nível PAR (2, 4, 6...) E Menor ou igual a 14
            bool isSkillLevel = (level % 2 == 0) && (level <= 14);

            if (isSkillLevel)
            {
                if (skillLearnUI) skillLearnUI.SetActive(true);
                if (upgradeCanvasUI) upgradeCanvasUI.SetActive(false);
                upgradeManager.OpenUpgradeMenu();
            }
            else
            {
                upgradeManagerStats.OnPlayerLevelUp(level);
            }

            Time.timeScale = 0f;
        }
    }
}
