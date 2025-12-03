using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [Header("Referências da UI")]
    public Image healthBarFill;
    public TextMeshProUGUI healthText;

    [Header("Configurações")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Animação (Opcional)")]
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

    public void SetMaxHealth(float max)
    {
        maxHealth = max;
        currentHealth = max;
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
        float fillAmount = currentHealth / maxHealth;

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
            healthText.text = $"{Mathf.Ceil(currentHealth)}/{maxHealth}";
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