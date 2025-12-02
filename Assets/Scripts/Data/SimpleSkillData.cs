using UnityEngine;

public enum SkillBehaviorType
{
    projectile,
    orbiting,
    groundArea,
    aoe,
    summonMinion
}
//para adicionar a opção no menu Assets> Create> Game> ComboRecipe
[CreateAssetMenu(fileName = "NewSkill", menuName = "Game/SimpleSkill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public GameObject effectPrefab;
    public float cooldown;
    public SkillBehaviorType behaviorType;

    [Header("Sistema de Fila")]
    public int maxCharges = 5;
}