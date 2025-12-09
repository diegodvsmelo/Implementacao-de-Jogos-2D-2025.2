using UnityEngine;
using TMPro; // Necessário para mexer com texto

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private Color textColor;
    
    [Header("Animação")]
    public float moveSpeed = 2f;
    public float disappearSpeed = 3f;
    public float disappearTimer = 0.5f; // Quanto tempo fica parado antes de sumir

    // Função para configurar o texto assim que ele nasce
    public void Setup(int damageAmount, bool isCritical)
    {
        textMesh = GetComponent<TextMeshPro>();
        textMesh.text = damageAmount.ToString();

        if (isCritical)
        {
            textMesh.fontSize = 6; // Maior
            textMesh.color = Color.red; // Ou amarelo/laranja
        }
        else
        {
            textMesh.fontSize = 4; // Normal
            textMesh.color = Color.white;
        }

        textColor = textMesh.color;
    }

    void Update()
    {
        // 1. Mover para cima
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);

        // 2. Contagem para começar a sumir
        disappearTimer -= Time.deltaTime;
        
        if (disappearTimer < 0)
        {
            // 3. Fade Out (Desaparecer suavemente)
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;

            // Se ficou totalmente invisível, destrói
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}