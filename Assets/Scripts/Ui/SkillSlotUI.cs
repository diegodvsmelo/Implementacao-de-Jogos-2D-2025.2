using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSlotUI : MonoBehaviour
{
    [Header("UI Components")]
    public Image skillIcon;       
    public Image cooldownOverlay; 
    public TextMeshProUGUI chargesText; 
    public TextMeshProUGUI cooldownText;

    public void UpdateSlot(SkillState state)
    {
        // 1. Se o slot estiver vazio (nenhuma skill aprendida)
        if (state == null || state.data == null)
        {
            skillIcon.enabled = false;
            skillIcon.sprite = null;
            if(cooldownOverlay) cooldownOverlay.fillAmount = 0;
            if(chargesText) chargesText.text = "";
            if(cooldownText) cooldownText.text = "";
            return;
        }

        skillIcon.enabled = true;
        skillIcon.sprite = state.data.icon;

        float timePassed = Time.time - state.lastUsedTime;
        float cooldownTotal = state.data.cooldown;
        float timeRemaining = cooldownTotal - timePassed;

        if (timeRemaining > 0)
        {
            // Em Cooldown
            if(cooldownOverlay) cooldownOverlay.fillAmount = timeRemaining / cooldownTotal;
            if(cooldownText) cooldownText.text = timeRemaining.ToString("F1");
            if(chargesText) chargesText.text = ""; // Esconde carga enquanto recarrega
        }
        else
        {
            if(cooldownOverlay) cooldownOverlay.fillAmount = 0;
            if(cooldownText) cooldownText.text = "";
            
            if (state.data.maxCharges > 1)
                if(chargesText) chargesText.text = state.currentCharges.ToString();
            else
                if(chargesText) chargesText.text = "";
        }
    }
}