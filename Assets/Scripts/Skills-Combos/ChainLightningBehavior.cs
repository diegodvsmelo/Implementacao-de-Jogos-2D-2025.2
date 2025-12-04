using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChainLightningBehavior : MonoBehaviour
{
    private int damage;
    private int maxBounces;
    private float range;
    
    [Header("Visual")]
    public LineRenderer lineRenderer;
    public float duration = 0.2f; // Tempo que o raio fica na tela

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    // Essa função será chamada pelo ComboManager
    public void Setup(int damage, int bounces, float range, Vector2 startPosition)
    {
        this.damage = damage;
        this.maxBounces = bounces;
        this.range = range;

        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        
        // Começa a lógica
        StartCoroutine(ProcessBounces(startPosition));
    }

    private IEnumerator ProcessBounces(Vector2 startPos)
    {
        Vector2 currentPos = startPos;
        GameObject target = FindClosestEnemy(currentPos);
        int bounces = 0;

        // Lista de pontos para desenhar a linha
        List<Vector3> linePoints = new List<Vector3>();
        linePoints.Add(startPos); // Ponto inicial (jogador/mouse)

        while (bounces < maxBounces && target != null)
        {
            hitEnemies.Add(target.gameObject);
            linePoints.Add(target.transform.position);

            // Aplica dano
            EnemyHealth hp = target.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }

            // Prepara próximo pulo
            currentPos = target.transform.position;
            target = FindClosestEnemy(currentPos);
            bounces++;
        }

        // Desenha o raio
        DrawLightning(linePoints);

        // Espera um pouco e destrói o objeto
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private void DrawLightning(List<Vector3> points)
    {
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    private GameObject FindClosestEnemy(Vector2 center)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, range);
        GameObject bestTarget = null;
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            // Ignora quem já foi atingido e verifica se tem vida
            if (hit.GetComponent<EnemyHealth>() != null && !hitEnemies.Contains(hit.gameObject))
            {
                float d = Vector2.Distance(center, hit.transform.position);
                if (d < closestDist)
                {
                    closestDist = d;
                    bestTarget = hit.gameObject;
                }
            }
        }
        return bestTarget;
    }
}