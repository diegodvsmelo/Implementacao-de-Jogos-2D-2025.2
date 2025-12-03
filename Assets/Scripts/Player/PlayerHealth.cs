using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PlayerStats stats;
    public GameObject gameOverScreen;
    public HealthBar healthBar;

    void Start()
    {        
        stats = GetComponent<PlayerStats>();
        healthBar.SetMaxHealth(stats.maxHealth);
    }
    public void TakeDamage(int damage)
    {
        healthBar.TakeDamage(damage);
        stats.currentHealth -= damage;
        if (stats.currentHealth <= 0)
        {
            PlayerDeath();
        }
    }

    public void Heal(int amount)
    {
        stats.currentHealth += amount;
        healthBar.Heal(amount);
        if (stats.currentHealth > stats.maxHealth)
        {
            stats.currentHealth = stats.maxHealth;
        }
    }

    public void PlayerDeath()
    {
        Destroy(this.gameObject);
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
    }
}
