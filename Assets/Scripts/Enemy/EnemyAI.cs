using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private int damage;
    [SerializeField] private float exp;
    public Transform target;
    public GameObject player;
    private Rigidbody2D rb;
    private Vector2 normalizedDirection;
    private bool isKnockedBack = false;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public void Initialize(EnemyData data, float statMultiplier)
    {
        this.moveSpeed = data.moveSpeed * statMultiplier;
        this.damage = (int)(data.damage * statMultiplier);
        this.exp = (float)(data.exp * statMultiplier);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    void OnEnable()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.RegisterEnemy(this);
        }
    }

    void OnDisable()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.UnregisterEnemy(this);
        }
    }

    void Update()
    {
        if (target != null)
        {
            Vector2 direction = target.position - transform.position;
            normalizedDirection = direction.normalized;
        }
        UpdateAnimator();
        FlipSprite();
    }

    void FixedUpdate()
    {
        if (isKnockedBack)
        {
            return;
        }
        rb.linearVelocity = normalizedDirection * moveSpeed;
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (isKnockedBack) return;

        StartCoroutine(KnockbackStun(direction, force));
    }

    private IEnumerator KnockbackStun(Vector2 direction, float force)
    {
        isKnockedBack = true;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.2f);
        isKnockedBack = false;
    }

    private void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", normalizedDirection.magnitude);
        }
    }

    private void FlipSprite()
    {
        if (spriteRenderer == null) return;

        if (normalizedDirection.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (normalizedDirection.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void OnDestroy()
    {
        if (!this.gameObject.scene.isLoaded) return;

        if (player != null)
        {
            var playerExp = player.GetComponent<PlayerExp>();
            if (playerExp != null)
            {
                playerExp.AddExp(this.exp);
            }
        }

        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.UnregisterEnemy(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        
        var playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }
}