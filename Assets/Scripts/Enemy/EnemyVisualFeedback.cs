using UnityEngine;
using System.Collections;

public class EnemyVisualFeedback : MonoBehaviour
{
    [Header("Damage Numbers")]
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private Vector3 damageNumberOffset = new Vector3(0, 0.5f, 0);
    
    [Header("Hit Flash")]
    [SerializeField] private bool useFlashEffect = true;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;
    
    [Header("Shake Effect")]
    [SerializeField] private bool useShakeEffect = true;
    [SerializeField] private float shakeMagnitude = 0.1f;
    [SerializeField] private float shakeDuration = 0.1f;
    
    [Header("Knockback")]
    [SerializeField] private bool useKnockback = false;
    [SerializeField] private float knockbackForce = 2f;
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector3 originalPosition;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        rb = GetComponent<Rigidbody2D>();
        originalPosition = transform.localPosition;
    }
    
    /// <summary>
    /// Chame este método quando o inimigo receber dano
    /// </summary>
    public void ShowDamage(int damageAmount, Vector2 damageSourcePosition = default)
    {
        // Exibe o número de dano
        if (damageNumberPrefab != null)
        {
            SpawnDamageNumber(damageAmount);
        }
        
        // Efeito de flash
        if (useFlashEffect && spriteRenderer != null)
        {
            StartCoroutine(FlashEffect());
        }
        
        // Efeito de shake
        if (useShakeEffect)
        {
            StartCoroutine(ShakeEffect());
        }
        
        // Knockback
        if (useKnockback && rb != null && damageSourcePosition != default)
        {
            ApplyKnockback(damageSourcePosition);
        }
    }
    
    private void SpawnDamageNumber(int damage)
    {
        GameObject damageNumber = Instantiate(
            damageNumberPrefab,
            transform.position + damageNumberOffset,
            Quaternion.identity
        );
        
        DamageNumber damageScript = damageNumber.GetComponent<DamageNumber>();
        if (damageScript != null)
        {
            // Pode definir cores diferentes para tipos de dano
            Color damageColor = Color.white;
            
            // Exemplo: dano crítico em amarelo
            if (damage > 50) // Ajuste este valor conforme seu jogo
            {
                damageColor = Color.yellow;
            }
            
            damageScript.Initialize(damage, damageColor);
        }
    }
    
    private IEnumerator FlashEffect()
    {
        if (spriteRenderer == null) yield break;
        
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
    
    private IEnumerator ShakeEffect()
    {
        float elapsed = 0f;
        
        while (elapsed < shakeDuration)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-shakeMagnitude, shakeMagnitude),
                Random.Range(-shakeMagnitude, shakeMagnitude),
                0f
            );
            
            transform.localPosition = originalPosition + randomOffset;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.localPosition = originalPosition;
    }
    
    private void ApplyKnockback(Vector2 sourcePosition)
    {
        Vector2 knockbackDirection = ((Vector2)transform.position - sourcePosition).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
    }
}
