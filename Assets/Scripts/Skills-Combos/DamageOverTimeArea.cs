using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTimeArea : MonoBehaviour
{
    [Header("Configuração do Dano")]
    public int damagePerTick = 10;
    public float timeBetweenTicks = 0.5f;
    public float duration = 3f;

    [Header("Áudio")]
    [SerializeField] private AudioClip[] tickSounds; 
    [Range(0f, 1f)] [SerializeField] private float tickVolume = 0.5f;

    private void Start()
    {
        StartCoroutine(DamageRoutine());
        
        Destroy(gameObject, duration);
    }

    private IEnumerator DamageRoutine()
    {

        float timer = 0f;
        
        while (timer < duration)
        {
            ApplyAreaDamage();
            PlayTickSound();

            yield return new WaitForSeconds(timeBetweenTicks);
            timer += timeBetweenTicks;
        }
    }

    private void ApplyAreaDamage()
    {
        Vector2 center = transform.position;
        Vector2 size = Vector2.one;

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            center = (Vector2)transform.position + collider.offset;
            size = collider.size;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(center, size, 0f);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                var enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damagePerTick);
                }
            }
        }
    }

    private void PlayTickSound()
    {
        if (tickSounds != null && SoundEffectsManager.Instance != null)
        {
            SoundEffectsManager.Instance.PlayRandomSFXClip(tickSounds, transform, tickVolume);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            Gizmos.DrawWireCube((Vector2)transform.position + collider.offset, collider.size);
        }
    }
}