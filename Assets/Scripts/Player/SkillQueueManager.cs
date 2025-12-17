using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillQueueManager : MonoBehaviour
{
    //Helpers variable
    Vector2 worldPosition;
    public AudioClip fireball;
    [SerializeField] private Animator animator;

    [Header("Configuração Inicial")]
    public SkillData startingSkill; 

    public SkillState[] activeSlots = new SkillState[4]; 
    
    public Queue<SkillState> waitingQueue = new Queue<SkillState>(); 

    private InputActions controls;
    private PlayerStats playerStats;
    private static readonly int Attack = Animator.StringToHash("attack");

    [Header("Configurações das skills")]
    public float waterSkillOffset = 1f;
    public float natureSpawnRadius = 2f;
    public int natureSpawnNumber = 3; // <--- Importante estar > 0

    void Awake()
    {
        controls = new InputActions();
        playerStats = GetComponent<PlayerStats>();

        // Limpa os slots
        for(int i=0; i<4; i++) activeSlots[i] = null;

        // Aprende a skill inicial se houver
        if (startingSkill != null)
        {
            LearnNewSkill(startingSkill);
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    // --- INPUT SYSTEM (Lógica Segura) ---
    void OnEnable()
    {
        controls.PlayerControls.Enable();
        controls.PlayerControls.Q.performed += OnSkillQ;
        controls.PlayerControls.W.performed += OnSkillW;
        controls.PlayerControls.E.performed += OnSkillE;
        controls.PlayerControls.R.performed += OnSkillR;
    }

    void OnDisable()
    {
        controls.PlayerControls.Disable();
        controls.PlayerControls.Q.performed -= OnSkillQ;
        controls.PlayerControls.W.performed -= OnSkillW;
        controls.PlayerControls.E.performed -= OnSkillE;
        controls.PlayerControls.R.performed -= OnSkillR;
    }

    private void OnSkillQ(InputAction.CallbackContext ctx) => TryUseSkill(0);
    private void OnSkillW(InputAction.CallbackContext ctx) => TryUseSkill(1);
    private void OnSkillE(InputAction.CallbackContext ctx) => TryUseSkill(2);
    private void OnSkillR(InputAction.CallbackContext ctx) => TryUseSkill(3);

    // --- SISTEMA DE USO ---
    private void TryUseSkill(int slotIndex)
    {
        // Verifica se o slot é válido
        if (activeSlots[slotIndex] == null || activeSlots[slotIndex].data == null)
        {
            Debug.Log($"Slot {slotIndex} vazio.");
            return;
        }

        SkillState skillState = activeSlots[slotIndex];

        // Verifica Cooldown
        if (Time.time - skillState.lastUsedTime < skillState.data.cooldown) return;

        // Verifica Cargas
        if (skillState.currentCharges <= 0)
        {
            RotateSkill(slotIndex);
            return;
        }

        // DISPARA A SKILL
        CastSkill(skillState.data);
        
        // Consome carga
        skillState.currentCharges--;
        skillState.lastUsedTime = Time.time;

        if (skillState.currentCharges <= 0)
        {
            RotateSkill(slotIndex);
        }
    }

    private void CastSkill(SkillData skill)
    {
        animator.SetTrigger(Attack);
        Debug.Log(Attack);

        switch (skill.behaviorType)
        {
            case SkillBehaviorType.projectile:
                CastProjectile(skill);
                Debug.Log("Cast Projectile (Fogo)");
                break;
            
            case SkillBehaviorType.summonMinion:
                CastSummon(skill);
                break;
            case SkillBehaviorType.chain:
                CastChainLightning(skill);
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
            case SkillBehaviorType.selfBuff:
                CastBuff(skill);
                break;
            case SkillBehaviorType.blackHole:
                CastBlackHole(skill);
                break;
        }
    }
    private void CastBlackHole(SkillData skill)
    {
        if (skill.effectPrefab == null) return;

        GameObject blackHole = Instantiate(skill.effectPrefab, MousePosition(), Quaternion.identity);

        BlackHoleSkill script = blackHole.GetComponent<BlackHoleSkill>();
        if (script != null)
        {
            // Calcula dano (se houver)
            int finalDamage = Mathf.RoundToInt(skill.damage * playerStats.damage);

            // AQUI USAMOS A SUA VARIÁVEL "durationOnGround"
            script.Setup(
                skill.durationOnGround, 
                skill.pullForce, 
                skill.pullRadius,
                finalDamage
            );
        }
    }
    private void CastBuff(SkillData skill)
    {
        if (playerStats != null)
        {
            playerStats.ActivateLightBuff(skill.buffDuration, skill.moveSpeedMultiplier, skill.effectPrefab);
        }
    }
    private void CastOrbiting(SkillData skill)
    {
        Instantiate(skill.effectPrefab, transform.position, Quaternion.identity, this.transform);
    }
    private void CastGroundArea(SkillData skill)
    {
        Instantiate(skill.effectPrefab, (Vector2)MousePosition(), Quaternion.identity);
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
    private void CastChainLightning(SkillData skill)
    {
        Vector2 mousePos = MousePosition();
        
        GameObject lightningObj = Instantiate(skill.effectPrefab, transform.position, Quaternion.identity);
        
        ChainLightningBehavior script = lightningObj.GetComponent<ChainLightningBehavior>();
        if (script != null)
        {
            int finalDamage = Mathf.RoundToInt(skill.damage * playerStats.damage);
            script.Setup(finalDamage, skill.bounceCount, skill.bounceRange, transform.position);
        }
    }
    private void CastProjectile(SkillData skill)
    {
        if (skill.effectPrefab == null) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;

        GameObject proj = Instantiate(skill.effectPrefab, transform.position, Quaternion.identity);
        SoundEffectsManager.Instance.PlaySFXClip(fireball,transform,0.4f);

        ProjectileMovement script = proj.GetComponent<ProjectileMovement>();
        
        if (script != null)
        {
            float projectileSpeed = 10f; 

            script.Setup(
                direction, 
                projectileSpeed,
                skill.damage,      
                skill.bounceCount, 
                skill.bounceRange  
            );
        }
    }
    private void CastSummon(SkillData skill)
    {
        if (skill.effectPrefab == null)
        {
            Debug.LogError("ERRO: Skill Natureza sem Prefab!");
            return;
        }

        NatureSummon herald = skill.effectPrefab.GetComponent<NatureSummon>();
        herald.damage = skill.damage;

        for(int i = 0; i < natureSpawnNumber; i++)
        {
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * natureSpawnRadius;
            Vector2 spawnPosition = (Vector2)transform.position + randomOffset;
            Instantiate(skill.effectPrefab, spawnPosition, Quaternion.identity);
        }
        Debug.Log($"Natureza: Spawnou {natureSpawnNumber} arautos.");
    }

    public void LearnNewSkill(SkillData newSkillData)
    {
        SkillState newState = new SkillState(newSkillData);

        for (int i = 0; i < activeSlots.Length; i++)
        {
            if (activeSlots[i] == null || activeSlots[i].data == null)
            {
                activeSlots[i] = newState;
                Debug.Log($"Aprendeu {newSkillData.skillName} no Slot {i}");
                return;
            }
        }
        waitingQueue.Enqueue(newState);
        Debug.Log($"Aprendeu {newSkillData.skillName} (Fila)");
    }

    public bool HasLearnedSkill(SkillData skillToCheck)
    {
        foreach (var slot in activeSlots)
            if (slot != null && slot.data == skillToCheck) return true;
        
        foreach (var qSkill in waitingQueue)
            if (qSkill.data == skillToCheck) return true;

        return false;
    }

    //-----------Helpers------------

    private void RotateSkill(int slotIndex)
    {
        SkillState oldSkill = activeSlots[slotIndex];
        oldSkill.ResetCharges();

        if (waitingQueue.Count > 0)
        {
            waitingQueue.Enqueue(oldSkill);
            activeSlots[slotIndex] = waitingQueue.Dequeue();
            Debug.Log("Rotação realizada.");
        }
        else
        {
            Debug.Log("Sem rotação disponível (recarregado).");
        }
    }
    
    private Vector2 MousePosition()
    {
        Vector2 screenPosition = Input.mousePosition;
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
}



