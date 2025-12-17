using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // --- REFERÊNCIAS ---
    private PlayerStats stats;
    private SpriteRenderer spriteRenderer;
    public GameObject gameOverScreen;
    [Header("Audio")]
    public AudioSource soundtrack;
    public AudioClip deathAudio;
    
    [Header("Interface (UI)")]
    public HealthBar healthBar; // <--- ARRASTE A BARRA DE VIDA AQUI NO INSPECTOR

    // --- CONFIGURAÇÕES DE IMPACTO ---
    [Header("Efeitos Visuais")]
    public Material flashMaterial; 
    private Material originalMaterial;
    public GameObject deathDimmer;
    public int dimmerLayer = 50;
    public int playerDeathLayer = 51;
    public float deathTimeScale = 0.2f;

    [Tooltip("Tempo congelado (Hit Stop)")]
    public float hitStopDuration = 0.15f; 

    // --- CONFIGURAÇÕES DE DEFESA ---
    [Header("Invulnerabilidade")]
    public float iFrameDuration = 1.0f; 
    public int numberOfFlashes = 5;     
    private bool isInvulnerable = false; 

    [Header("Knockback")]
    public float knockbackForce = 10f; 
    public float knockbackRadius = 3f; 

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); 
        originalMaterial = spriteRenderer.material;
    }

    void Start()
    {
        // Garante que a barra de vida comece sincronizada com o status atual do player
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(stats.maxHealth, false);
            healthBar.SetHealth(stats.currentHealth);
        }
    }
    
    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        stats.currentHealth = (int)(stats.currentHealth - (damage * stats.damageReduction));
        
        if (stats.currentHealth > 0 && animator != null)
        {
            animator.SetTrigger("Hurt");
        }
        
        if (stats != null)
        {
            stats.ModifyHealth(-damage);
        }

        if (stats.currentHealth <= 0)
        {
            PlayerDeath();
            return;
        }
        
        //Efeitos de Impacto
        StartCoroutine(HitStopRoutine());
        StartCoroutine(InvulnerabilityRoutine());
        ApplyAreaKnockback();
    }

    private IEnumerator HitStopRoutine()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f;
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        
        float flashInterval = iFrameDuration / (numberOfFlashes * 2);

        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRenderer.material = flashMaterial;
            yield return new WaitForSeconds(flashInterval);
            
            spriteRenderer.material = originalMaterial;
            yield return new WaitForSeconds(flashInterval);
        }

        spriteRenderer.material = originalMaterial;
        isInvulnerable = false;
    }

    private void ApplyAreaKnockback()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, knockbackRadius);
        foreach (Collider2D target in colliders)
        {
            if (target.CompareTag("Enemy"))
            {
                EnemyAI enemyAI = target.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    Vector2 direction = (target.transform.position - transform.position).normalized;
                    enemyAI.ApplyKnockback(direction, knockbackForce);
                }
            }
        }
    }
    
    public void Heal(int amount)
    {
        stats.currentHealth += amount;
        if(stats.currentHealth > stats.maxHealth) stats.currentHealth = stats.maxHealth;
        
        // Atualiza a UI ao curar também!
        if (healthBar != null)
        {
            healthBar.Heal(amount);
        }
    }

    // Adicione essa variável no topo
    private Animator animator;
    
    public void PlayerDeath()
    {
        if(animator != null)
        {
            soundtrack.Stop();
            SoundEffectsManager.Instance.PlaySFXClip(deathAudio, transform, 0.3f);
            animator.SetTrigger("Die");
            animator.speed = 1f / deathTimeScale; 
        }
        
        Time.timeScale = deathTimeScale; 

        if(deathDimmer != null)
        {
            deathDimmer.SetActive(true);
            
            SpriteRenderer dimmerRenderer = deathDimmer.GetComponent<SpriteRenderer>();
            if(dimmerRenderer != null) dimmerRenderer.sortingOrder = dimmerLayer;
            
            spriteRenderer.sortingOrder = playerDeathLayer;
            
            Transform shadow = transform.Find("Shadow");
            if(shadow != null) 
            {
                shadow.gameObject.SetActive(false); 
            }
        }
        
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<Collider2D>().enabled = false; 
        
        StartCoroutine(ShowGameOverDelay());
    }

    private IEnumerator ShowGameOverDelay()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
        animator.speed = 0;

        yield return new WaitForSecondsRealtime(2.0f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, knockbackRadius);
    }
}