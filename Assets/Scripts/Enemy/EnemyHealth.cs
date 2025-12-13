using Unity.Mathematics;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private EnemyVisualFeedback visualFeedback;
    public int maxHealth;
    [SerializeField] private int currentHealth;

    private void Awake()
    {
        visualFeedback = GetComponent<EnemyVisualFeedback>();
    }

    public void Initialize(EnemyData data, float statMultiplier)
    {
        this.maxHealth = (int)(data.maxHealth*statMultiplier);
        currentHealth = maxHealth;
    }
    void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (visualFeedback != null)
        {
            visualFeedback.ShowDamage(damage);
        }

        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
