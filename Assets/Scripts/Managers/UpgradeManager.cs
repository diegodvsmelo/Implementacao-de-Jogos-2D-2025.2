using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject upgradeCanvasUI; // Menu de Status (Vida, Dano...)
    public GameObject skillLearnUI;    // Menu de Skills (Fogo, Água...)
    
    [Header("Systems")]
    public PlayerExp playerXp;
    public PlayerStats playerStats;
    public SkillQueueManager skillManager;

    [Header("Upgrades de Status")]
    public Button btnHealth;
    public Button btnDamage;
    public Button btnSpeed;
    public Button btnCooldown;
    
    [Header("Textos de Status")]
    public TextMeshProUGUI healthtxt;
    public TextMeshProUGUI damagetxt;
    public TextMeshProUGUI speedtxt;
    public TextMeshProUGUI cooldowntxt;

    [Header("Valores de Status")]
    public int healthToAdd = 20;
    public float damageToAdd = 0.1f;
    public float speedToAdd = 0.5f;
    public float cooldownToAdd = 0.1f;

    [Header("Botões de Habilidade")]
    // (Agrupando para ficar organizado)
    public Button btnUnlockWater; public SkillData waterSkillData;
    public Button btnUnlockFire;  public SkillData fireSkillData;
    public Button btnUnlockAir;   public SkillData airSkillData;
    public Button btnUnlockEarth; public SkillData earthSkillData;
    
    public Button btnUnlockLight; public SkillData lightSkillData;
    public Button btnUnlockDark;  public SkillData darkSkillData;
    public Button btnUnlockNature; public SkillData natureSkillData;
    public Button btnUnlockLightning; public SkillData lightningSkillData;

    void Start()
    {
        if(playerXp == null) playerXp = GameObject.FindWithTag("Player").GetComponent<PlayerExp>();

        if(upgradeCanvasUI != null) upgradeCanvasUI.SetActive(false);
        if(skillLearnUI != null) skillLearnUI.SetActive(false);

        if(btnHealth) btnHealth.onClick.AddListener(() => ApplyUpgrade("health"));
        if(btnDamage) btnDamage.onClick.AddListener(() => ApplyUpgrade("damage"));
        if(btnSpeed) btnSpeed.onClick.AddListener(() => ApplyUpgrade("speed"));
        if(btnCooldown) btnCooldown.onClick.AddListener(() => ApplyUpgrade("cooldown"));

        SetupSkillButton(btnUnlockWater, waterSkillData);
        SetupSkillButton(btnUnlockFire, fireSkillData);
        SetupSkillButton(btnUnlockAir, airSkillData);
        SetupSkillButton(btnUnlockEarth, earthSkillData);
        SetupSkillButton(btnUnlockLight, lightSkillData);
        SetupSkillButton(btnUnlockDark, darkSkillData);
        SetupSkillButton(btnUnlockNature, natureSkillData);
        SetupSkillButton(btnUnlockLightning, lightningSkillData);

        UpdateStatusUI();
    }

    void SetupSkillButton(Button btn, SkillData data)
    {
        if (btn != null && data != null)
        {
            btn.onClick.AddListener(() => ApplySkillUnlock(data));
        }
    }

    public void OpenUpgradeMenu()
    {
        UpdateStatusUI();
        bool isEvenLevel = (playerXp.level % 2 == 0);

        if (isEvenLevel&&playerXp.level <15)
        {
            skillLearnUI.SetActive(true);
            upgradeCanvasUI.SetActive(false); 
            
            RefreshSkillButtonsState();
        }
        else
        {
            upgradeCanvasUI.SetActive(true);
            skillLearnUI.SetActive(false);
        }
        
        Time.timeScale = 0f; 
    }

    void RefreshSkillButtonsState()
    {
        CheckButtonState(btnUnlockWater, waterSkillData);
        CheckButtonState(btnUnlockFire, fireSkillData);
        CheckButtonState(btnUnlockAir, airSkillData);
        CheckButtonState(btnUnlockEarth, earthSkillData);
        CheckButtonState(btnUnlockLight, lightSkillData);
        CheckButtonState(btnUnlockDark, darkSkillData);
        CheckButtonState(btnUnlockNature, natureSkillData);
        CheckButtonState(btnUnlockLightning, lightningSkillData);
    }

    void CheckButtonState(Button btn, SkillData data)
    {
        if (btn == null || data == null) return;

        bool alreadyLearned = skillManager.HasLearnedSkill(data);

        if (alreadyLearned)
        {
            btn.interactable = false; 
            
            var colors = btn.colors;
            colors.disabledColor = new Color(1, 1, 1, 0.45f); 
            btn.colors = colors;
        }
        else
        {
            btn.interactable = true; 
            
            var colors = btn.colors;
            colors.normalColor = Color.white;
            btn.colors = colors;
        }
    }
    public void ApplyUpgrade(string type)
    {
        if (playerStats == null) return;

        switch (type)
        {
            case "health":
                playerStats.UpgradeHealth(healthToAdd);
                break;
            case "damage":
                playerStats.UpgradeDamage(damageToAdd);
                break;
            case "speed":
                playerStats.UpgradeSpeed(speedToAdd);
                break;
            case "cooldown":
                break;
        }
        CloseUpgradeMenu();
    }

    public void ApplySkillUnlock(SkillData skill)
    {
        if (skillManager != null)
        {
            skillManager.LearnNewSkill(skill);
            Debug.Log($"Habilidade {skill.skillName} Aprendida!");
        }
        CloseUpgradeMenu();
    }
    public void CloseUpgradeMenu()
    {
        upgradeCanvasUI.SetActive(false);
        skillLearnUI.SetActive(false);
        Time.timeScale = 1f;
    }
    
    private void UpdateStatusUI()
    {
        if(healthtxt != null) healthtxt.text = "+ " + healthToAdd + " HP";
        if(damagetxt != null) damagetxt.text = "+ " + (damageToAdd * 100) + "% Damage";
        if(speedtxt != null) speedtxt.text = "+ " + speedToAdd + " Speed";
        if(cooldowntxt != null) cooldowntxt.text = "- " + (cooldownToAdd * 100) + "% Cooldown";
    }
}