using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    
    [Header("Configuração da Wave")]
    [Tooltip("A linha do tempo manual das ondas, em ordem (0s, 180s, 360s, etc.)")]
    public List<WaveData> timeline;

    [Header("Configuração do 'Endgame'")]
    [Tooltip("A cada X segundos DEPOIS da última onda, aumenta a dificuldade.")]
    public float endgameScalingFrequency = 60f;

    [Tooltip("Multiplicador de stats dos inimigos (ex: 1.1 = +10% por 'tick')")]
    public float endgameStatMultiplier = 1.1f;

    [Tooltip("Multiplicador do cooldown (ex: 0.95 = 5% mais rápido por 'tick')")]
    public float endgameCooldownMultiplier = 0.95f;

    
    //Variáveis Internas
    
    private Transform playerTransform;

    [Header("Variáveis de Estado")]
    [SerializeField] private float spawnDistance = 12f;
    [SerializeField] private WaveData currentWave;
    [SerializeField] private int currentWaveIndex=0;
    [SerializeField] private float currentStatMultiplier;
    [SerializeField] private float currentCooldownMultiplier;
    [SerializeField] private float nextEndgameTickTime;
    [SerializeField] private float finalCooldown;
    
    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;

        if(timeline.Count==0)
        {
            Debug.Log("A timeline do spawner está vazia.");
            return;
        }

        currentWave = timeline[0];
        currentWaveIndex = 0;

        StartCoroutine(SpawnEnemy());
    }
    //-----GETTERS-----
    public float StatMultiplierUI
    {
        get { return currentStatMultiplier; }
    }
    public int WaveLevelUI
    {
        get { return currentWaveIndex; }
    }
    public float CooldownMultiplierUI
    {
        get { return currentCooldownMultiplier; }
    }
    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            float gameTime = Time.timeSinceLevelLoad;
            
            //Verifica se a wave atual não é a ultima wave
            if(currentWaveIndex<timeline.Count-1)
            {
                WaveData nextWave = timeline[currentWaveIndex+1];
                if(gameTime>= nextWave.timeInSecToStart)
                {
                    currentWaveIndex++;
                    currentWave = nextWave;
                }
            }
            //Verifica se está na ultima wave, para poder começar a lógica de multiplicadores ''Endgame''
            if(currentWaveIndex==timeline.Count-1)
            {
                if (nextEndgameTickTime <= 0)
                {
                    endgameScalingFrequency = 15f;
                }
                if(nextEndgameTickTime==0f)
                {
                    nextEndgameTickTime = currentWave.timeInSecToStart+endgameScalingFrequency;
                }

                if(gameTime>=nextEndgameTickTime)
                {
                    currentStatMultiplier*=endgameStatMultiplier;
                    currentCooldownMultiplier*=endgameCooldownMultiplier;

                    nextEndgameTickTime+=endgameScalingFrequency;
                }
            }

            int index = Random.Range(0, currentWave.enemyPool.Count);
            EnemyData dataToSpawn = currentWave.enemyPool[index];

            Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
            Vector2 spawnPosition = (Vector2)playerTransform.position + (randomDirection * spawnDistance);



            GameObject newEnemy = Instantiate(dataToSpawn.enemyPrefab, spawnPosition, Quaternion.identity);

            newEnemy.GetComponent<EnemyAI>().Initialize(dataToSpawn, currentStatMultiplier);
            newEnemy.GetComponent<EnemyHealth>().Initialize(dataToSpawn,currentStatMultiplier);

            newEnemy.transform.localScale *= 5f;
            newEnemy.GetComponent<BoxCollider2D>().size = new Vector2(0.2f, 0.2f);

            finalCooldown = currentWave.cooldownBetweenSpawn*currentCooldownMultiplier;
            yield return new WaitForSeconds(finalCooldown);
        }

    }
    void OnDrawGizmos()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, spawnDistance);
        }
        
    }
}
