using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private EnemyVisualFeedback visualFeedback;
    private static PlayerStats playerStats;

    public int maxHealth;
    [SerializeField] private int currentHealth;

    private void Awake()
    {
        visualFeedback = GetComponent<EnemyVisualFeedback>();

        if (playerStats == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerStats = player.GetComponent<PlayerStats>();
            }
        }
    }

    public void Initialize(EnemyData data, float statMultiplier)
    {
        this.maxHealth = (int)(data.maxHealth * statMultiplier);
        currentHealth = maxHealth;
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        bool isCrit = false;
        float finalDamage = damage;

        // Verifica se encontrou o PlayerStats
        if (playerStats != null)
        {
            isCrit = Random.Range(0f, 1f) <= playerStats.criticalChance;
            finalDamage = isCrit ? damage * playerStats.criticalMultiplier : damage;
        }
        else
        {
            // Fallback: se n�o encontrou PlayerStats, usa valores padr�o
            Debug.LogWarning("PlayerStats n�o encontrado! Usando dano padr�o.");
        }

        currentHealth -= (int)finalDamage;

        // Exibe o feedback visual
        if (visualFeedback != null)
        {
            visualFeedback.ShowDamage((int)finalDamage, isCrit);
        }

        // Verifica se morreu
        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}