using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("UI Reference")]
    public HealthBar healthBar; 

    [Header("Atributos de Vida")]
    public int maxHealth;
    public int currentHealth;

    [Header("Atributos de Movimento")]
    public float moveSpeed;

    [Header("Atributos de Combate")]
    public int damage;
    public int damageMultiplier;
    public float projectileSpeed;
    public float attackSpeed;
    public float cooldownReductionMultiplier;
    public int criticalChance;
    public float criticalMultiplier;

    [Header("Estado de Buffs")]
    public bool isShielded = false;
    [Range(0f, 1f)] public float shieldReduction = 0.5f;


    private PlayerHealth playerHealthScript;

    void Awake()
    {
        playerHealthScript = GetComponent<PlayerHealth>();
        currentHealth = maxHealth;
    }

    void Start()
    {
        // --- CORREÇÃO AQUI ---
        // Se a healthBar não foi atribuída neste script, tenta pegar a do PlayerHealth
        if (healthBar == null && playerHealthScript != null)
        {
            healthBar = playerHealthScript.healthBar;
        }

        currentHealth = maxHealth;
        
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth, false); 
            healthBar.SetHealth(currentHealth);       
        }
    }

    public void ActivateLightBuff(float duration, float speedMult, GameObject visualPrefab)
    {
        StartCoroutine(LightBuffRoutine(duration, speedMult, visualPrefab));
    }

    private IEnumerator LightBuffRoutine(float duration, float speedMult, GameObject visualPrefab)
    {
        isShielded = true;
    
        PlayerMovement movement = GetComponent<PlayerMovement>();
        float originalTerminalVelocity = moveSpeed; 

        if (movement != null)
        {
            originalTerminalVelocity = movement.terminalVelocity; 
            movement.terminalVelocity *= speedMult;               
        }
        else
        {
            moveSpeed *= speedMult; 
        }
        
        Debug.Log("Luz Ativada: Escudo + Velocidade!");

        GameObject visualInstance = null;
        if (visualPrefab != null)
        {
            visualInstance = Instantiate(visualPrefab, transform.position, Quaternion.identity, transform);
        }

        yield return new WaitForSeconds(duration);

        isShielded = false; 
        
        if (movement != null)
        {
            movement.terminalVelocity = originalTerminalVelocity;
        }
        else
        {
            moveSpeed /= speedMult;
        }
        
        if (visualInstance != null) Destroy(visualInstance);
        
        Debug.Log("Luz Desativada.");
    }

    //-----Funções de Modificação (Dano/Cura)-----
    
    public void ModifyHealth(int amount)
    {
        if (amount < 0 && isShielded)
        {
            float reducedAmount = amount * (1.0f - shieldReduction);
            amount = Mathf.RoundToInt(reducedAmount);
        }
        currentHealth += amount;
        
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Garante que temos a referência antes de atualizar
        if(healthBar == null && playerHealthScript != null) healthBar = playerHealthScript.healthBar;
        if(healthBar != null) healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0 && playerHealthScript != null)
        {
            playerHealthScript.PlayerDeath();
        }
    }

    //-----Funções de Upgrade-----
    public void UpgradeHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount; // Cura o valor aumentado

        // --- CORREÇÃO AQUI TAMBÉM ---
        // Garante a referência antes de usar
        if (healthBar == null && playerHealthScript != null)
        {
            healthBar = playerHealthScript.healthBar;
        }

        if (healthBar != null)
        {
            // O true ou false no segundo parâmetro depende se você quer somar a vida ou não na lógica da barra
            // Como já somamos no currentHealth acima, usei false para apenas reajustar o visual
            healthBar.SetMaxHealth(maxHealth, false); 
            healthBar.SetHealth(currentHealth);
        }
        
        Debug.Log("Vida Aumentada! Max: " + maxHealth);
    }

    public void UpgradeSpeed(float amount)
    {
        moveSpeed += amount;
        Debug.Log("Velocidade Aumentada! Nova: " + moveSpeed);
    }

    public void UpgradeDamage(float percentage)
    {
        damage += (int)percentage;
        Debug.Log("Dano Multiplicado! Novo: " + damage);
    }
    public void UpgradeCooldown(float percentage)
    {
        cooldownReductionMultiplier -= percentage;
        Debug.Log("Cooldown reduzido! Novo: " + cooldownReductionMultiplier);
    }
}