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
    public float globalDamageMultiplier;
    public float cooldownReductionMultiplier;


    private PlayerHealth playerHealthScript;

    void Awake()
    {
        playerHealthScript = GetComponent<PlayerHealth>();
        currentHealth = maxHealth;
    }
    void Start()
    {
        // Inicialização Segura
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth, false); 
            healthBar.SetHealth(currentHealth);       
        }
    }

    //-----Funções de Modificação (Dano/Cura)-----
    
    // Agora o PlayerHealth chama esta função, em vez de diminuir a vida direto
    public void ModifyHealth(int amount)
    {
        currentHealth += amount;
        
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

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
        currentHealth += amount;
        if (healthBar != null)
        {
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
        globalDamageMultiplier += percentage;
        Debug.Log("Dano Multiplicado! Novo: " + globalDamageMultiplier);
    }
    public void UpgradeCooldown(float percentage)
    {
        cooldownReductionMultiplier -= percentage;
        Debug.Log("Dano Multiplicado! Novo: " + cooldownReductionMultiplier);
    }
}
