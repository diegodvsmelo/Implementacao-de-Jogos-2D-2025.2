using UnityEngine;

public enum SkillBehaviorType
{
    projectile,
    orbiting,
    groundArea,
    aoe,
    chain
}
//para adicionar a opção no menu Assets> Create> Game> ComboRecipe
[CreateAssetMenu(fileName = "NewSkill", menuName = "Game/SimpleSkill")]
public class SkillData : ScriptableObject
{
    public string key1;
    public GameObject effectPrefab;
    public float coolDown;
    
    [Header("Configurações Específicas")]
    public int damage = 10;
    public int bounceCount = 3;
    public float bounceRange = 5f;

    public SkillBehaviorType behaviorType;
}