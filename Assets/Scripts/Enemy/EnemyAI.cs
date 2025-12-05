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
        this.moveSpeed = data.moveSpeed* statMultiplier;
        this.damage = (int)(data.damage*statMultiplier);
        this.exp = (float)(data.exp*statMultiplier);
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
        target = player.transform;

        //adicionando o inimigo no array de inimigos no mapa para definir qual inimigo será alvo do player
        EnemyManager.allEnemies.Add(this);
    }
    void Update()
    {
        if (target != null)
        {
            Vector2 direction = target.position - transform.position;
            //a magnitude do vetor direction será maior quanto mais longe estiver do player, fazendo ele se mover mais rapido  quando longe e mais lento quando perto, para evitar isso, basta normalizar o valor
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
        animator.SetFloat("Speed", normalizedDirection.magnitude);
        //altera a variavel speed dentro do animator com base na magnitude do moveinput, caso o player gere qualquer movimentação no personagem, a magnitude será maior que zero, e dentro do animator, a animação de andar precisa apenas q speed seja maior q 0.1
    }
    private void FlipSprite()
    {
        if (normalizedDirection.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (normalizedDirection.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }
    //a função OnDestroy é chamada em todos os scripts de objetos quando são destruidos, como o script enemyHealth destroi o inimigo qnd a vida chega a zero, o script EnemyAI dele vai chamar a função OnDestroy abaixo, que vai tirar ele do array de inimigos no mapa
    private void OnDestroy()
    {
        EnemyManager.allEnemies.Remove(this);
        var playerExp = player.GetComponent<PlayerExp>();
        if (playerExp != null)
        {
            playerExp.AddExp(this.exp); 
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
