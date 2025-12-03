using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Searcher.SearcherWindow;

public class SkillSlot : MonoBehaviour
{
    [Header("UI Components")]
    public Image skillIcon;
    public Image cooldownOverlay;
    public TextMeshProUGUI cooldownText;
    public TextMeshProUGUI keybindText;

    private Skill skill;

    public void Initialize(Skill skillData)
    {
        skill = skillData;

        if (skillIcon != null && skill.skillIcon != null)
            skillIcon.sprite = skill.skillIcon;

        if (keybindText != null)
            keybindText.text = skill.keyBind.ToString();

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 0;
    }

    public void UpdateCooldown(float currentCooldown, float maxCooldown)
    {
        bool isOnCooldown = currentCooldown > 0;

        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = isOnCooldown ? currentCooldown / maxCooldown : 0;
        }

        if (cooldownText != null)
        {
            if (isOnCooldown)
            {
                cooldownText.gameObject.SetActive(true);
                cooldownText.text = Mathf.Ceil(currentCooldown).ToString();
            }
            else
            {
                cooldownText.gameObject.SetActive(false);
            }
        }
    }
}