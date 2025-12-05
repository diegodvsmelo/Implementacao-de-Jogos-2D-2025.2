using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ComboManager : MonoBehaviour
{
    private PlayerStats stats;
    private InputActions controls;
    private Vector2 worldPosition;

    public List<ComboData> comboRecipes;
    public List<SkillData> skillRecipes;
    private Dictionary<string, float> cooldownTracker;
    private List<ComboInput> inputBuffer;
    public float comboWindow = 300f;
    private Coroutine clearBuffer;

    [Header("Configurações das skills")]
    public float waterSkillOffset = 1f;

    public struct ComboInput
    {
        public string key;
        public float time;
    }
    void Awake()
    {
        cooldownTracker = new Dictionary<string, float>();
        controls = new InputActions();
        inputBuffer = new List<ComboInput>();
        stats = GetComponent<PlayerStats>();
    }
    void OnEnable()
    {
        controls.PlayerControls.Enable();
        controls.PlayerControls.Fire.performed += FireHandler;
        controls.PlayerControls.Water.performed += WaterHandler;
        controls.PlayerControls.Air.performed += AirHandler;
        controls.PlayerControls.Earth.performed += EarthHandler;
    }
    void OnDisable()
    {
        controls.PlayerControls.Disable();
        controls.PlayerControls.Fire.performed -= FireHandler;
        controls.PlayerControls.Water.performed -= WaterHandler;
        controls.PlayerControls.Air.performed -= AirHandler;
        controls.PlayerControls.Earth.performed -= EarthHandler;
    }
    //----------GETTERS----------
    public List<ComboInput> CurrentInputBuffer
    {
        get { return inputBuffer; }
    }
    public Dictionary<string, float> CooldownTracker
    {
        get { return cooldownTracker; }
    }
    public string LastUsedComboName { get; private set; } = "";


    private void FireHandler(InputAction.CallbackContext context)
    {
        ComboInput input = new ComboInput { key = "Fire", time = Time.time };
        SkillData fireSkill = FindSkill("Fire");

        if (fireSkill == null)
        {
            return;
        }
        if (IsOnCoolDown(fireSkill.key1, fireSkill.coolDown))
        {
            return;
        }

        if (inputBuffer.Count > 0 && (Time.time - inputBuffer[0].time < comboWindow))
        {
            if (clearBuffer != null)
            {
                StopCoroutine(clearBuffer);
            }
            inputBuffer.Add(input);
            CheckForCombo();
            inputBuffer.Clear();
        }
        else
        {
            CastSimpleSkill(fireSkill);

            if (clearBuffer != null)
            {
                StopCoroutine(clearBuffer);
            }

            inputBuffer.Clear();
            inputBuffer.Add(input);
            clearBuffer = StartCoroutine(ClearBufferAfterTime());
        }

    }
    private void WaterHandler(InputAction.CallbackContext context)
    {
        ComboInput input = new ComboInput { key = "Water", time = Time.time };
        SkillData waterSkill = FindSkill("Water");


        if (waterSkill == null)
        {
            return;
        }
        if (IsOnCoolDown(waterSkill.key1, waterSkill.coolDown))
        {
            return;
        }
        if (inputBuffer.Count > 0 && (Time.time - inputBuffer[0].time < comboWindow))
        {
            if (clearBuffer != null)
            {
                StopCoroutine(clearBuffer);
            }
            inputBuffer.Add(input);
            CheckForCombo();
            inputBuffer.Clear();
        }
        else
        {
            CastSimpleSkill(waterSkill);

            if (clearBuffer != null)
            {
                StopCoroutine(clearBuffer);
            }

            inputBuffer.Clear();
            inputBuffer.Add(input);
            clearBuffer = StartCoroutine(ClearBufferAfterTime());
        }
    }
    private void AirHandler(InputAction.CallbackContext context)
    {
        ComboInput input = new ComboInput { key = "Air", time = Time.time };
        SkillData airSkill = FindSkill("Air");

        if (airSkill == null)
        {

            return;
        }
        if (IsOnCoolDown(airSkill.key1, airSkill.coolDown))
        {
            return;
        }
        //Logica principal
        if (inputBuffer.Count > 0 && (Time.time - inputBuffer[0].time < comboWindow))
        {
            if (clearBuffer != null)
            {
                StopCoroutine(clearBuffer);
            }
            inputBuffer.Add(input);
            CheckForCombo();
            inputBuffer.Clear();
        }
        else
        {
            CastSimpleSkill(airSkill);

            if (clearBuffer != null)
            {
                StopCoroutine(clearBuffer);
            }

            inputBuffer.Clear();
            inputBuffer.Add(input);
            clearBuffer = StartCoroutine(ClearBufferAfterTime());
        }
    }
    private void EarthHandler(InputAction.CallbackContext context)
    {
        ComboInput input = new ComboInput { key = "Earth", time = Time.time };
        SkillData earthSkill = FindSkill("Earth");

        if (earthSkill == null)
        {
            return;
        }
        if (IsOnCoolDown(earthSkill.key1, earthSkill.coolDown))
        {
            return;
        }
        //Logica principal
        if (inputBuffer.Count > 0 && (Time.time - inputBuffer[0].time < comboWindow))
        {
            if (clearBuffer != null)
            {
                StopCoroutine(clearBuffer);
            }
            inputBuffer.Add(input);
            CheckForCombo();
            inputBuffer.Clear();
        }
        else
        {
            CastSimpleSkill(earthSkill);

            if (clearBuffer != null)
            {
                StopCoroutine(clearBuffer);
            }

            inputBuffer.Clear();
            inputBuffer.Add(input);
            clearBuffer = StartCoroutine(ClearBufferAfterTime());
        }
    }
    private void CastSimpleSkill(SkillData skill)
    {
        cooldownTracker[skill.key1] = Time.time;
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
            
            case SkillBehaviorType.chain:
                CastChainLightning(skill);
                break;
        }   
    }

    private void CastProjectile(SkillData skill)
    {


        Vector2 direction = (Vector2)MousePosition() - (Vector2)transform.position;
        Vector2 directionNormalized = direction.normalized;

        //para que o projetil fique direcionado à posição do mouse
        float angle = Mathf.Atan2(directionNormalized.y, directionNormalized.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle + 90f);

        GameObject newProjectile = Instantiate(skill.effectPrefab, transform.position, rotation);
        var projectileScript = newProjectile.GetComponent<ProjectileMovement>();
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
    
    private void CastChainLightning(SkillData skill)
    {
        Vector2 mousePos = MousePosition();
        
        GameObject lightningObj = Instantiate(skill.effectPrefab, transform.position, Quaternion.identity);
        
        ChainLightningBehavior script = lightningObj.GetComponent<ChainLightningBehavior>();
        if (script != null)
        {
            int finalDamage = Mathf.RoundToInt(skill.damage * stats.globalDamageMultiplier);
            script.Setup(finalDamage, skill.bounceCount, skill.bounceRange, transform.position);
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
    public void CheckForCombo()
    {

        if (inputBuffer.Count >= 2)
        {
            ComboInput input1 = inputBuffer[inputBuffer.Count - 1];
            ComboInput input2 = inputBuffer[inputBuffer.Count - 2];
            if (input1.time - input2.time < comboWindow)
            {
                ComboData combo = FindComboRecipe(inputBuffer[0].key, inputBuffer[1].key);
                if(combo.comboEffectPrefab != null)
                {
                    Instantiate(combo.comboEffectPrefab, MousePosition(), Quaternion.identity);
                }
                
            }
            else if (input1.time - input2.time > comboWindow)
            {
            }
            inputBuffer.Clear();
        }
    }
    private IEnumerator ClearBufferAfterTime()
    {
        yield return new WaitForSeconds(comboWindow);
        inputBuffer.Clear();
    }
    private SkillData FindSkill(string key1)
    {
        foreach (var recipe in skillRecipes)
        {      
            if (key1 == recipe.key1)
            {
                return recipe;
            }
        }
        return null;
    }
    public ComboData FindComboRecipe(string key1, string key2)
    {
        foreach (ComboData recipe in comboRecipes)
        {
            //para testar comutatividade, verifico ambas as condições
            bool match1 = (key1 == recipe.key1 && key2 == recipe.key2);
            bool match2 = (key1 == recipe.key2 && key2 == recipe.key1);
            if (match1 || match2)
            {
                LastUsedComboName = recipe.name;
                cooldownTracker[recipe.name] = Time.time;
                return recipe;
            }
        }
        return null;
    }
    private Vector2 MousePosition()
    {
        Vector2 screenPosition = Input.mousePosition;
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }

    private bool IsOnCoolDown(string skillName, float cooldownDuration)
    {
        if (cooldownTracker.ContainsKey(skillName))
        {
            if (Time.time - cooldownTracker[skillName] < cooldownDuration)
            {
                return true;
            }
        }
        return false;
    }

}
