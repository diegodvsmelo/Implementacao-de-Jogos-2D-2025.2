using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class SkillHUD : MonoBehaviour
{
    [Header("Skills")]
    public Skill[] skills = new Skill[4];

    [Header("UI References")]
    public GameObject skillSlotPrefab;
    public Transform skillContainer;

    private SkillSlot[] skillSlots;

    void Start()
    {
        InitializeSkillSlots();
    }

    void Update()
    {
        UpdateSkills();
        UpdateUI();
    }

    void InitializeSkillSlots()
    {
        skillSlots = new SkillSlot[skills.Length];

        for (int i = 0; i < skills.Length; i++)
        {
            GameObject slotObj = Instantiate(skillSlotPrefab, skillContainer);
            skillSlots[i] = slotObj.GetComponent<SkillSlot>();
            skillSlots[i].Initialize(skills[i]);
        }
    }

    void UpdateSkills()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].currentCooldown > 0)
            {
                skills[i].currentCooldown -= Time.deltaTime;
                if (skills[i].currentCooldown < 0)
                    skills[i].currentCooldown = 0;
            }

            // Detecta input da skill
            if (Input.GetKeyDown(skills[i].keyBind) && !skills[i].IsOnCooldown())
            {
                UseSkill(i);
            }
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            skillSlots[i].UpdateCooldown(skills[i].currentCooldown, skills[i].cooldown);
        }
    }

    void UseSkill(int index)
    {
        skills[index].currentCooldown = skills[index].cooldown;

        // Aqui você adiciona a lógica da skill
        Debug.Log($"Skill {skills[index].skillName} usada!");

        // Exemplo: chamar um evento ou método do player
        // PlayerController.instance.CastSkill(index);
    }
}