using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float lifetime = 1f;

    [Header("Configurações Visuais")]
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.5f);
    [SerializeField] private Vector2 randomDirection = new Vector2(0.3f, 0.3f);
    [SerializeField] private Color simpleColor;
    [SerializeField] private Color criticalColor;

    private TextMeshPro textMesh;
    private Vector3 moveDirection;
    private float timer;
    
    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            simpleColor = textMesh.color;
        }
    }
    
    public void Initialize(int damage, bool isCritical)
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();
            
        textMesh.text = damage.ToString();

        if (isCritical)
        {
            textMesh.color = criticalColor;
        } else {
            textMesh.color = simpleColor;
        }
        
        // Adiciona uma direção aleatória para o movimento
        moveDirection = new Vector3(
            Random.Range(-randomDirection.x, randomDirection.x),
            1f,
            0f
        ).normalized;
    }
    
    private void Update()
    {
        timer += Time.deltaTime;
        
        // Movimento para cima com variação
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        
        // Escala animada
        float normalizedTime = timer / lifetime;
        transform.localScale = Vector3.one * scaleCurve.Evaluate(normalizedTime);
        
        // Fade out
        if (timer > lifetime * 0.5f)
        {
            Color newColor = textMesh.color;
            newColor.a = Mathf.Lerp(simpleColor.a, 0f, (timer - lifetime * 0.5f) / (lifetime * 0.5f));
            textMesh.color = newColor;
        }
        
        // Destroi após o tempo de vida
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
