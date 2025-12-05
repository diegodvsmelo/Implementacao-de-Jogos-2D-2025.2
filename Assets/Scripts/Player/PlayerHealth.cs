using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Referências existentes
    private PlayerStats stats;
    public GameObject gameOverScreen;
    private SpriteRenderer spriteRenderer; 

    [Header("Configurações de Invulnerabilidade")]
    public float iFrameDuration = 1.0f; 
    public int numberOfFlashes = 5;      
    private bool isInvulnerable = false;

    [Header("Configurações de Knockback")]
    public float knockbackForce = 5f; 
    public float knockbackRadius = 3f; 
    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        // Passa a responsabilidade da matemática para o Stats
        // Passamos valor negativo pois é dano
        if(stats != null) stats.ModifyHealth(-damage);

        StartCoroutine(InvulnerabilityRoutine());
        ApplyAreaKnockback();
    }

    public void Heal(int amount)
    {
        stats.currentHealth += amount;
        if (stats.currentHealth > stats.maxHealth)
        {
            stats.currentHealth = stats.maxHealth;
        }
    }

    public void PlayerDeath()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
        gameObject.SetActive(false);
    }

    // --- Novas Funcionalidades ---

    private void ApplyAreaKnockback()
    {
        // Busca colisores na área (raio configurável)
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, knockbackRadius);

        foreach (Collider2D target in colliders)
        {
            if (target.CompareTag("Enemy"))
            {
                // Em vez de pegar o Rigidbody, pegamos o script de inteligência do inimigo
                EnemyAI enemyAI = target.GetComponent<EnemyAI>();

                if (enemyAI != null)
                {
                    // Calcula direção: Do Player PARA o Inimigo
                    Vector2 direction = (target.transform.position - transform.position).normalized;

                    // Chama a função que já existe no seu inimigo!
                    // Ela vai pausar o movimento dele e aplicar a força corretamente.
                    enemyAI.ApplyKnockback(direction, knockbackForce);
                }
            }
        }
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;

        // Salva a cor original (geralmente branca)
        Color originalColor = spriteRenderer.color;
        // Cria a cor transparente
        Color flashColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);

        // Loop para piscar
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(iFrameDuration / (numberOfFlashes * 2));

            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(iFrameDuration / (numberOfFlashes * 2));
        }

        // Garante que a cor voltou ao normal e desliga a imunidade
        spriteRenderer.color = originalColor;
        isInvulnerable = false;
    }

    // Função visual para você ver o tamanho do knockback na Scene do Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, knockbackRadius);
    }
}