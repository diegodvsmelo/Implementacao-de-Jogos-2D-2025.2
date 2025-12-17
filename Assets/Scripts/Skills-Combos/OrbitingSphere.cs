using UnityEngine;

public class OrbitingSphere : MonoBehaviour
{
    public int damage;
    public AudioClip audioClip;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            SoundEffectsManager.Instance.PlaySFXClip(audioClip, transform, 0.8f);
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            Destroy(this.gameObject);
        }
    }
}
