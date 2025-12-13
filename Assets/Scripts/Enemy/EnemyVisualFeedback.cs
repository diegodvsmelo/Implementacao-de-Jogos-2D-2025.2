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
    [SerializeField] private Color criticalFlashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

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

    public void ShowDamage(int damageAmount, bool isCritical)
    {
        // Exibe o número de dano
        if (damageNumberPrefab != null)
        {
            SpawnDamageNumber(damageAmount, isCritical);
        }

        // Efeito de flash (cor diferente para crítico)
        if (useFlashEffect && spriteRenderer != null)
        {
            StartCoroutine(FlashEffect(isCritical));
        }

        // Efeitos especiais de crítico
        //if (isCritical)
        //{
        //    ShowCriticalEffects();
        //}
    }

    private void SpawnDamageNumber(int damage, bool isCritical)
    {
        GameObject damageNumber = Instantiate(
            damageNumberPrefab,
            transform.position + damageNumberOffset,
            Quaternion.identity
        );

        DamageNumber damageScript = damageNumber.GetComponent<DamageNumber>();
        if (damageScript != null)
        {
            // Usa o método com suporte a críticos
            damageScript.Initialize(damage, isCritical);
        }
    }

    private IEnumerator FlashEffect(bool isCritical)
    {
        if (spriteRenderer == null) yield break;

        // Usa cor diferente para crítico
        Color flashCol = isCritical ? criticalFlashColor : flashColor;

        spriteRenderer.color = flashCol;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
}