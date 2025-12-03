using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerStats stats;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private InputActions controls;
    private SpriteRenderer spriteRenderer;

    private Vector2 targetPosition;
    private Vector2 normalizedDirection;

    public GameObject targetMarker;
    private Coroutine activeMarkerCoroutine;
    [SerializeField] private float markerStopDistance;


    // Awake sempre para inicialização de variaveis proprias
    void Awake()
    {
        controls = new InputActions();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        stats = GetComponent<PlayerStats>();
        
        targetPosition = transform.position;
    }

    private void OnEnable()
    {
        controls.PlayerControls.Enable();
        controls.PlayerControls.Walk.performed += HandleWalkClick;
    }
    private void OnDisable()
    {
        controls.PlayerControls.Disable();
        controls.PlayerControls.Walk.performed -= HandleWalkClick;
    }

    private void HandleWalkClick(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = Input.mousePosition;
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        RaycastHit2D hitInfo = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hitInfo.collider != null)
        {
            targetPosition = hitInfo.point;
            activeMarkerCoroutine = StartCoroutine(ShowMarker());
        }
    }
    
    IEnumerator ShowMarker()
    {
        targetMarker.transform.position = targetPosition;
        targetMarker.gameObject.SetActive(true);

        while(Vector2.Distance(transform.position, targetMarker.transform.position) >0.3)
        {
            yield return null;
        }
        targetMarker.gameObject.SetActive(false);
        activeMarkerCoroutine = null;
    }

    void Update()
    {
        //deixar codigo inativo para possivel adaptação para controle futuramente
        //moveInput = controls.PlayerControls.Move.ReadValue<Vector2>();

        if (Vector2.Distance(targetPosition, (Vector2)transform.position) >0.2)
        {
            Vector2 direction = targetPosition - (Vector2)transform.position;

            normalizedDirection = direction.normalized;
        }
        else
        {
            normalizedDirection = Vector2.zero;
        }

        UpdateAnimator();
        FlipSprite();
    }

    private void FlipSprite()
    {
        if(normalizedDirection.x < 0)
        {
            spriteRenderer.flipX = true;
        }else if(normalizedDirection.x >0)
        {
            spriteRenderer.flipX=false;
        }
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("Speed", normalizedDirection.magnitude);
        //altera a variavel speed dentro do animator com base na magnitude do moveinput, caso o player gere qualquer movimentação no personagem, a magnitude será maior que zero, e dentro do animator, a animação de andar precisa apenas q speed seja maior q 0.1
    }

    void FixedUpdate()
    {
        rb.linearVelocity = normalizedDirection * stats.moveSpeed;
        //rb.linearVelocity = new Vector2(moveInput.x * speed * Time.deltaTime, moveInput.y * speed * Time.deltaTime);
    }

}
