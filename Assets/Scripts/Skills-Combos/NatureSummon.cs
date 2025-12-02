using UnityEngine;

public class NatureSummon : MonoBehaviour
{
    [Header("Atributos do Arauto")]
    public float moveSpeed = 4f;
    public int damage = 15;
    public float lifeTime = 5f; 
    public float attackRate = 1.0f; 

    private Transform currentTarget;
    private float nextAttackTime = 0f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (currentTarget == null)
        {
            FindNearestEnemy();
        }

        if (currentTarget != null)
        {
            MoveTowardsTarget();
        }
    }

    void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
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

    void MoveTowardsTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (Time.time >= nextAttackTime)
            {
                EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                    nextAttackTime = Time.time + attackRate;
                }
            }
        }
    }
}