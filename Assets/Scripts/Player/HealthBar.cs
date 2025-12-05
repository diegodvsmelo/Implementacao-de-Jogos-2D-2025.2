using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [Header("Refer�ncias da UI")]
    public Image healthBarFill;
    public TextMeshProUGUI healthText;

    [Header("Configura��es")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Anima��o (Opcional)")]
    public bool smoothTransition = true;
    public float transitionSpeed = 5f;

    private float targetFillAmount;

    void Start()
    {
        currentHealth = maxHealth;
        targetFillAmount = 1f;
        UpdateHealthBar();
    }

    void Update()
    {
        if (smoothTransition && healthBarFill != null)
        {
            healthBarFill.fillAmount = Mathf.Lerp(
                healthBarFill.fillAmount,
                targetFillAmount,
                Time.deltaTime * transitionSpeed
            );
        }
    }

    public void SetMaxHealth(float newMax,bool healDifference = true)
    {
        if (healDifference)
        {
            // Se eu tinha 100/100 e fui pra 120, agora tenho 120/120
            // Ou se eu tinha 50/100 e fui pra 120, ganho +20 e fico com 70/120
            float difference = newMax - maxHealth;
            if(difference > 0)
            {
                currentHealth += difference;
            }
        }

        maxHealth = newMax;
        
        // Garante que a vida atual não ultrapasse o novo máximo
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        UpdateHealthBar();
    }

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        float fillAmount = 0f;
        if (maxHealth > 0) fillAmount = currentHealth / maxHealth;

        if (smoothTransition)
        {
            targetFillAmount = fillAmount;
        }
        else if (healthBarFill != null)
        {
            healthBarFill.fillAmount = fillAmount;
        }


        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
}