using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Security.Cryptography;
using UnityEngine.Rendering;

public class UiDebug : MonoBehaviour
{
    [Header("WaveInfo")]
    public TextMeshProUGUI leveltxt;
    public TextMeshProUGUI statMultitxt;
    public TextMeshProUGUI cdMultitxt;

    [Header("SkillsInfo")]
    public SkillQueueManager skillManager;
    public GameObject debugUI;
    public bool isDebugUIActive;

    public TextMeshProUGUI cooldownText;

    [Header("PlayerStats")]
    public GameObject player;
    private PlayerStats stats;
    public TextMeshProUGUI currentHealthtxt;
    public TextMeshProUGUI maxHealthtxt;
    void Start()
    {
       stats = player.GetComponent<PlayerStats>();
    }
    void Update()
    {
        
        UpdatePlayerStats();
        if (!isDebugUIActive)
        {
            debugUI.SetActive(false);
        }
        else
        {
            debugUI.SetActive(true);
            UpdateCooldownsText();
            UpdateWaveInfo();
        }
    }
    

    void UpdateCooldownsText()
    {
        string cdString = "Cooldowns Ativos:\n";

        // Agora iteramos pelos 4 SLOTS ATIVOS (Q, W, E, R)
        for (int i = 0; i < skillManager.activeSlots.Length; i++)
        {
            SkillState state = skillManager.activeSlots[i];

            // Se o slot não estiver vazio (tem uma skill aprendida)
            if (state != null)
            {
                string skillName = state.data.skillName;
                float duration = state.data.cooldown;
                float lastUsed = state.lastUsedTime;

                // Matemática do Cooldown
                float timePassed = Time.time - lastUsed;
                float timeRemaining = duration - timePassed;

                string keyName = ""; // Só pra ficar bonito na UI
                if (i == 0) keyName = "[Q] ";
                if (i == 1) keyName = "[W] ";
                if (i == 2) keyName = "[E] ";
                if (i == 3) keyName = "[R] ";

                if (timeRemaining > 0)
                {
                    cdString += keyName + skillName + ": " + timeRemaining.ToString("F1") + "s\n";
                }
                else
                {
                    // Mostra também as cargas restantes!
                    cdString += keyName + skillName + ": Pronto (" + state.currentCharges + "x)\n";
                }
            }
            else
            {
                cdString += $"Slot {i}: Vazio\n";
            }
        }

        cooldownText.text = cdString;
    }
    void UpdatePlayerStats()
    {
        if(player !=null)
        {
            if(stats!=null)
            {
                maxHealthtxt.text = "Max: "+stats.maxHealth.ToString();
                currentHealthtxt.text = "Current: "+stats.currentHealth.ToString();
            }
        }
    }
    void UpdateWaveInfo()
    {
        EnemySpawner enemySpawner = GameObject.FindWithTag("EnemySpawner").GetComponent<EnemySpawner>();
        leveltxt.text = "Wave: "+ enemySpawner.WaveLevelUI;
        statMultitxt.text = "Stat Multiplier: "+ enemySpawner.StatMultiplierUI;
        cdMultitxt.text = "CooldownMulti: "+ enemySpawner.CooldownMultiplierUI;
    }
}
