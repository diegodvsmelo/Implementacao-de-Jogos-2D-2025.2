using UnityEngine;

public class NatureSummon : MonoBehaviour
{
    [Header("Atributos do Arauto")]
    public float moveSpeed = 4f;
    public int damage = 15;
    public float lifeTime = 6f; 
    public float attackRate = 1.0f;
    public AudioClip audioClip;

    private Transform currentTarget;
    private float nextAttackTime = 0f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (currentTarget == null)
        {
            FindNearestEnemy();
        }

    }
    void FixedUpdate()
    {
        if (currentTarget == null)
        {
            rb.linearVelocity = Vector2.zero; 
        }
        else
        {
            MoveToTarget();
        }
    }
    void MoveToTarget()
    {
        Vector2 direction = ((Vector2)currentTarget.position - rb.position).normalized;

        Vector2 newPos = rb.position + (direction * moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }
    void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            if(enemy == null) continue;

            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
        {
            currentTarget = nearestEnemy.transform;
        }
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            
            if (Time.time >= nextAttackTime)
            {
                EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
                
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                    
                    nextAttackTime = Time.time + attackRate;
                    SoundEffectsManager.Instance.PlaySFXClip(audioClip, transform, 0.4f);
                }
            }
        }
    }
}