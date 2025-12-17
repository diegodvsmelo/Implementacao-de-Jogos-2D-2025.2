using System.Collections;
using UnityEngine;

public class BlackHoleSkill : MonoBehaviour
{
    [Header("Configurações")]
    private float pullForce;
    private float radius;
    private int damage;
    private float duration;

    private float damageTickRate = 0.5f; 

    [Header("Áudio")]
    [SerializeField] private AudioClip tickSound;
    [Range(0f, 1f)] [SerializeField] private float tickVolume = 0.4f;

    public void Setup(float durationTime, float force, float range, int dmg)
    {
        this.duration = durationTime;
        this.pullForce = Mathf.Abs(force);
        this.radius = range;
        this.damage = dmg;

        transform.localScale = Vector3.one * (range / 2f); 
    }

    void Start()
    {
        StartCoroutine(DamageAndSoundRoutine());
        
        Destroy(gameObject, duration);
    }

    void FixedUpdate()
    {
        PullEnemies();
    }

    private void PullEnemies()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        
        foreach (Collider2D target in colliders)
        {
            if (target.CompareTag("Enemy"))
            {

                Vector2 direction = (transform.position - target.transform.position).normalized;
                
                Rigidbody2D enemyRb = target.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyRb.AddForce(direction * pullForce * Time.fixedDeltaTime, ForceMode2D.Force);
                }
            }
        }
    }

    private IEnumerator DamageAndSoundRoutine()
    {
        float timer = 0f;
        
        while (timer < duration)
        {
            ApplyAreaDamage();
            PlayTickSound();

            yield return new WaitForSeconds(damageTickRate);
            timer += damageTickRate;
        }
    }

    private void ApplyAreaDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                var enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }
            }
        }
    }

    private void PlayTickSound()
    {
        if (tickSound != null && SoundEffectsManager.Instance != null)
        {
            SoundEffectsManager.Instance.PlaySFXClip(tickSound, transform, tickVolume);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}