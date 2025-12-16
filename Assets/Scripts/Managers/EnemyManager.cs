using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public static List<EnemyAI> allEnemies;

    void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            allEnemies = new List<EnemyAI>(); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void RegisterEnemy(EnemyAI enemy)
    {
        if (!allEnemies.Contains(enemy))
        {
            allEnemies.Add(enemy);
        }
    }

    public void UnregisterEnemy(EnemyAI enemy)
    {
        if (allEnemies.Contains(enemy))
        {
            allEnemies.Remove(enemy);
        }
    }
}