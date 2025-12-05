using UnityEngine;

public enum SkillBehaviorType
{
    projectile,
    orbiting,
    groundArea,
    aoe,
    chain,
    summonMinion,
}
//para adicionar a opção no menu Assets> Create> Game> ComboRecipe
[CreateAssetMenu(fileName = "NewSkill", menuName = "Game/SimpleSkill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public GameObject effectPrefab;
    public float cooldown;
    public int maxCharges;
    [Header("Visual")]
    public Sprite icon;
    
    [Header("Configurações Específicas")]
    public int damage;
    public int bounceCount;
    public float bounceRange;

    public SkillBehaviorType behaviorType;
    //
}