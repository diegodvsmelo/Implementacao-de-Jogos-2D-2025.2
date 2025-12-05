using UnityEngine;

[System.Serializable]
public class Skill
{
    public string skillName;
    public Sprite skillIcon;
    public KeyCode keyBind;
    public float cooldown;
    [HideInInspector] public float currentCooldown;

    public bool IsOnCooldown()
    {
        return currentCooldown > 0;
    }
}