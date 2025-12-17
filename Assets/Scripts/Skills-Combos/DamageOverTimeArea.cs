using System;
using System.Collections;
using UnityEngine;

public class DamageOverTimeArea : MonoBehaviour
{
    [Header("Configuração do Dano")]
    public int damagePerTick;
    public float timeBetweenTicks;
    public float duration;
    public AudioClip[] damageAudio;

    private BoxCollider2D areaCollider;

    void Start()
    {
        areaCollider = GetComponent<BoxCollider2D>();

        StartCoroutine(DamagePulse());
        
        Destroy(gameObject, duration);
    }

    private IEnumerator DamagePulse()
    {
        while(true)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(transform.position, areaCollider.size, transform.eulerAngles.z);

            foreach (Collider2D enemy in hitEnemies)
            {
                EnemyHealth health = enemy.GetComponent<EnemyHealth>();
                if (health != null)
                {
                    health.TakeDamage(damagePerTick);
                    SoundEffectsManager.Instance.PlayRandomSFXClip(damageAudio, transform, 0.4f);
                }
            }
            yield return new WaitForSeconds(timeBetweenTicks);
        }
    }
}
