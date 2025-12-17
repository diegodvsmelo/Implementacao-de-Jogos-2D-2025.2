using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    private float speed;
    private int damage;
    
    // Variáveis de comportamento
    private int bouncesRemaining;
    private float bounceRange;

    private Vector2 moveDirection;
    private bool isSetup = false;
    [Header("Audio")]
    [SerializeField] private AudioClip[] hitSound;
    public void Setup(Vector2 dir, float spd, int dmg, int bounces, float range)
    {
        speed = spd;
        damage = dmg;
        bouncesRemaining = bounces;
        bounceRange = range;

        moveDirection = dir;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle+90f);

        isSetup = true;
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (!isSetup) return;

        transform.position += (Vector3)moveDirection * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            if (hitSound != null && SoundEffectsManager.Instance != null)
            {
                // Usamos 'transform' para o som sair da posição onde o projétil morreu
                SoundEffectsManager.Instance.PlayRandomSFXClip(hitSound, transform, 0.4f);
            }

            if (bouncesRemaining > 0)
            {
                HandleBounce(other.transform.position, other.gameObject);
            }
            else
            {
                Destroy(gameObject); 
            }
        }
    }

    void HandleBounce(Vector2 hitPosition, GameObject currentTarget)
    {
        bouncesRemaining--;

        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPosition, bounceRange);
        Transform bestTarget = null;
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy") && hit.gameObject != currentTarget)
            {
                float dist = Vector2.Distance(hitPosition, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    bestTarget = hit.transform;
                }
            }
        }

        if (bestTarget != null)
        {
            moveDirection = (bestTarget.position - transform.position).normalized;
            
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}