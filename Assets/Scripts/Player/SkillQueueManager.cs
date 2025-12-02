using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillQueueManager : MonoBehaviour
{
    [Header("Configuração Inicial")]
    public SkillData startingSkill; 

    public SkillState[] activeSlots = new SkillState[4]; 
    

    public Queue<SkillState> waitingQueue = new Queue<SkillState>(); 

    private InputActions controls;


    private Vector2 worldPosition;
    private PlayerStats stats;
    

    [Header("Configurações das skills")]
    public float waterSkillOffset = 1f;
    public float natureSpawnRadius = 2f;
    public int natureSpawnNumber = 3;
    void Awake()
    {
        controls = new InputActions();
        stats = GetComponent<PlayerStats>();

        for(int i=0; i<4; i++) activeSlots[i] = null;

        if (startingSkill != null)
        {
            LearnNewSkill(startingSkill);
        }
    }

    void OnEnable()
    {
        controls.PlayerControls.Enable();
        controls.PlayerControls.Q.performed += ctx => TryUseSkill(0);
        controls.PlayerControls.W.performed += ctx => TryUseSkill(1);
        controls.PlayerControls.E.performed += ctx => TryUseSkill(2);
        controls.PlayerControls.R.performed += ctx => TryUseSkill(3);
    }

    void OnDisable()
    {
        controls.PlayerControls.Disable();
        // Lembre-se de remover os listeners aqui
    }

    // --- SISTEMA DE APRENDIZADO ---
    
    // Esta função será chamada pelo seu UpgradeManager/PlayerStats
    public void LearnNewSkill(SkillData newSkillData)
    {
        SkillState newState = new SkillState(newSkillData);

        // 1. Tenta encontrar um slot vazio nos ativos (Q, W, E, R)
        for (int i = 0; i < activeSlots.Length; i++)
        {
            if (activeSlots[i] == null)
            {
                activeSlots[i] = newState;
                Debug.Log($"Aprendeu {newSkillData.skillName}! Colocada no Slot {i}.");
                return; // Encontrou lugar, termina aqui
            }
        }

        // 2. Se todos os slots estiverem cheios, vai para a fila
        waitingQueue.Enqueue(newState);
        Debug.Log($"Aprendeu {newSkillData.skillName}! Colocada na Fila de Espera.");
    }

    // --- LÓGICA DE USO ---

    private void TryUseSkill(int slotIndex)
    {
        // VERIFICAÇÃO DE SEGURANÇA: O slot tem skill?
        if (activeSlots[slotIndex] == null)
        {
            Debug.Log("Slot vazio! Você ainda não aprendeu uma skill para esta tecla.");
            return;
        }

        SkillState skillState = activeSlots[slotIndex];

        if (Time.time - skillState.lastUsedTime < skillState.data.cooldown)
        {
            return; // Em cooldown
        }

        if (skillState.currentCharges <= 0)
        {
            RotateSkill(slotIndex);
            return;
        }

        CastSkill(skillState.data);
        
        skillState.currentCharges--;
        skillState.lastUsedTime = Time.time;

        if (skillState.currentCharges <= 0)
        {
            RotateSkill(slotIndex);
        }
    }

    private void RotateSkill(int slotIndex)
    {
        SkillState exhaustedSkill = activeSlots[slotIndex];
        exhaustedSkill.ResetCharges();

        if (waitingQueue.Count > 0)
        {
            //Envia a esgotada para o fim da fila
            waitingQueue.Enqueue(exhaustedSkill);

            //Pega a nova da fila
            SkillState nextSkill = waitingQueue.Dequeue();
            activeSlots[slotIndex] = nextSkill;
            
            Debug.Log($"ROTAÇÃO: {exhaustedSkill.data.skillName} saiu. {nextSkill.data.skillName} entrou.");
        }
        else
        {
            // Se não tem ninguém na fila (player tem 4 skills ou menos), a skill apenas recarrega e fica no slot.
            Debug.Log($"Recarregou {exhaustedSkill.data.skillName} (sem rotação disponível).");
        }
    }


    private void CastSkill(SkillData skill)
    {
        switch (skill.behaviorType)
        {
            case SkillBehaviorType.projectile:
                CastProjectile(skill);
                break;
            case SkillBehaviorType.aoe:
                CastAOE(skill);
                break;

            case SkillBehaviorType.orbiting:
                CastOrbiting(skill);
                break;

            case SkillBehaviorType.groundArea:
                CastGroundArea(skill);
                break;
            case SkillBehaviorType.summonMinion:
                CastSummon(skill);
                break;
        }
    }

    private void CastProjectile(SkillData skill)
    {


        Vector2 direction = (Vector2)MousePosition() - (Vector2)transform.position;
        Vector2 directionNormalized = direction.normalized;
        float angle = Mathf.Atan2(directionNormalized.y, directionNormalized.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle + 90f);

        if (skill.effectPrefab == null)
        {
            Debug.LogError($"ERRO: A skill '{skill.skillName}' não tem um Prefab atribuído!");
            return;
        }
        GameObject newProjectile = Instantiate(skill.effectPrefab, transform.position, rotation);
        ProjectileMovement projectileScript = newProjectile.GetComponent<ProjectileMovement>();
        projectileScript.Setup(directionNormalized);
        projectileScript.damage = Mathf.RoundToInt(projectileScript.damage*stats.globalDamageMultiplier);
    }
    private void CastAOE(SkillData skill)
    {
        Vector2 direction = (Vector2)MousePosition() - (Vector2)transform.position;
        Vector2 directionNormalized = direction.normalized;
        Vector2 spawnPosition = (Vector2)transform.position + (directionNormalized * waterSkillOffset);

        //para que o projetil fique direcionado à posição do mouse
        float angle = Mathf.Atan2(directionNormalized.y, directionNormalized.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle - 15f);

        GameObject splash = Instantiate(skill.effectPrefab, spawnPosition, rotation);
        splash.transform.SetParent(this.transform);
    }
    private void CastOrbiting(SkillData skill)
    {
        Instantiate(skill.effectPrefab, transform.position, Quaternion.identity, this.transform);
    }
    private void CastGroundArea(SkillData skill)
    {
        Instantiate(skill.effectPrefab, (Vector2)MousePosition(), Quaternion.identity);
    }
    private void CastSummon(SkillData skill)
    {
    for(int i = 0; i < natureSpawnNumber; i++)
    {
        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * natureSpawnRadius;
        Vector2 spawnPosition = (Vector2)transform.position + randomOffset;
        Instantiate(skill.effectPrefab, spawnPosition, Quaternion.identity);
    }
    }

    private Vector2 MousePosition()
    {
        Vector2 screenPosition = Input.mousePosition;
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
}