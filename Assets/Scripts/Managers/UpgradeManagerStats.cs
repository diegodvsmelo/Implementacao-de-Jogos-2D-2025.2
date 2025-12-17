using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UpgradeManagerStats : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private UpgradeData[] allUpgrades; 
    [SerializeField] private int upgradesPerLevel = 4; 
    
    [Header("UI")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private UpgradeOption[] upgradeOptions; 
    
    [Header("Balanceamento")]
    [SerializeField] private bool allowDuplicates = false; 
    [SerializeField] private bool useRarityWeight = true; 
    
    private List<UpgradeData> currentUpgradeSelection = new List<UpgradeData>();
    private int currentPlayerLevel = 1;
    
    public static UpgradeManagerStats Instance { get; private set; }
    private PlayerHealth playerHealthScript;
    public GameObject player;

    private void Awake()
    {
        playerHealthScript = player.GetComponent<PlayerHealth>();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
        
        ResetAllUpgrades();
    }
    
    public void OnPlayerLevelUp(int newLevel)
    {
        currentPlayerLevel = newLevel;
        ShowUpgradeOptions();
    }
    
    public void ShowUpgradeOptions()
    {
        currentUpgradeSelection = SelectRandomUpgrades(upgradesPerLevel);
        
        if (currentUpgradeSelection.Count == 0)
        {
            Debug.LogWarning("Nenhum upgrade disponível!");
            return;
        }
        
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
            Time.timeScale = 0f;
        }
        
        for (int i = 0; i < upgradeOptions.Length; i++)
        {
            if (i < currentUpgradeSelection.Count)
            {
                upgradeOptions[i].Setup(currentUpgradeSelection[i], this);
                upgradeOptions[i].gameObject.SetActive(true);
            }
            else
            {
                upgradeOptions[i].gameObject.SetActive(false);
            }
        }
    }
    
    private List<UpgradeData> SelectRandomUpgrades(int count)
    {
        List<UpgradeData> selected = new List<UpgradeData>();
        
        List<UpgradeData> availableUpgrades = allUpgrades
            .Where(u => u.CanBeOffered(currentPlayerLevel))
            .ToList();
        
        if (availableUpgrades.Count == 0)
        {
            Debug.LogWarning("Nenhum upgrade disponível para o nível atual!");
            return selected;
        }
        
        int attempts = 0;
        int maxAttempts = 100;
        
        while (selected.Count < count && attempts < maxAttempts)
        {
            UpgradeData randomUpgrade;
            
            if (useRarityWeight)
            {
                randomUpgrade = SelectWeightedRandom(availableUpgrades);
            }
            else
            {
                randomUpgrade = availableUpgrades[Random.Range(0, availableUpgrades.Count)];
            }
            
            if (!allowDuplicates && selected.Contains(randomUpgrade))
            {
                attempts++;
                continue;
            }
            
            selected.Add(randomUpgrade);
            
            if (!allowDuplicates)
            {
                availableUpgrades.Remove(randomUpgrade);
            }
            
            if (availableUpgrades.Count == 0)
                break;
            
            attempts++;
        }
        
        return selected;
    }
    
    private UpgradeData SelectWeightedRandom(List<UpgradeData> upgrades)
    {
        float totalWeight = 0f;
        foreach (var upgrade in upgrades)
        {
            totalWeight += 11f - upgrade.rarity;
        }
        
        float randomValue = Random.Range(0f, totalWeight);
        
        float currentWeight = 0f;
        foreach (var upgrade in upgrades)
        {
            currentWeight += 11f - upgrade.rarity;
            if (randomValue <= currentWeight)
            {
                return upgrade;
            }
        }
        
        return upgrades[0];
    }
    
    public void OnUpgradeSelected(UpgradeData upgrade)
    {
        upgrade.currentLevel++;
        
        ApplyUpgrade(upgrade);
        
        CloseUpgradePanel();
    }
    
    private void ApplyUpgrade(UpgradeData upgrade)
    {
        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
        
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats não encontrado!");
            return;
        }
        
        float valueToAdd = upgrade.value;
        
        if (upgrade.isPercentage)
        {
            // Exemplo: se dano atual é 10 e upgrade dá +20%, adiciona 2
            // Isso depende de como seu PlayerStats está estruturado
        }
        
        // Aplica baseado no tipo
        switch (upgrade.upgradeType)
        {
            case UpgradeType.AttackSpeed:
                playerStats.UpgradeAttackSpeed(valueToAdd);
                break;

            case UpgradeType.Cooldown:
                playerStats.UpgradeCooldown(valueToAdd);
                break;

            case UpgradeType.CriticalChance:
                playerStats.UpgradeCriticalChance(valueToAdd);
                break;
            
            case UpgradeType.CriticalMultiplier:
                playerStats.UpgradeCriticalMultiplier(valueToAdd);
                break;

            case UpgradeType.Damage:
                playerStats.UpgradeDamage(valueToAdd);
                break;

            case UpgradeType.DamageReduction:
                playerStats.UpgradeDamageReduction(valueToAdd);
                break;

            //case UpgradeType.ExpGain:
            //    playerStats.UpgradeExpGain(valueToAdd);
            //    break;

            case UpgradeType.HealthRegen:
                Debug.Log("ENTROU1 " + upgrade.currentLevel);
                playerStats.UpgradeHealthRegen(valueToAdd);
                if (upgrade.currentLevel == 1)
                {
                    Debug.Log("ENTROU");
                    StartCoroutine(playerHealthScript.HealthRegen());
                }
                break;

            case UpgradeType.MaxHealth:
                playerStats.UpgradeHealth((int)valueToAdd); 
                break;
            
            case UpgradeType.MoveSpeed:
                playerStats.UpgradeMoveSpeed(valueToAdd);
                break;

            case UpgradeType.ProjectileSpeed:
                playerStats.UpgradeProjectileSpeed(valueToAdd);
                break;

            default:
                Debug.LogWarning($"Tipo de upgrade não implementado: {upgrade.upgradeType}");
                break;
        }
        
        Debug.Log($"Upgrade aplicado: {upgrade.upgradeName} (Nível {upgrade.currentLevel})");
    }
    
    /// <summary>
    /// Fecha o painel de upgrades e despausa o jogo
    /// </summary>
    public void CloseUpgradePanel()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
        
        Time.timeScale = 1f; // Despausa o jogo
    }
    
    /// <summary>
    /// Reseta todos os upgrades (útil ao reiniciar o jogo)
    /// </summary>
    public void ResetAllUpgrades()
    {
        foreach (var upgrade in allUpgrades)
        {
            upgrade.Reset();
        }
        currentPlayerLevel = 1;
    }
    
    /// <summary>
    /// Retorna todos os upgrades que o player já pegou
    /// </summary>
    public List<UpgradeData> GetAcquiredUpgrades()
    {
        return allUpgrades.Where(u => u.currentLevel > 0).ToList();
    }
}
