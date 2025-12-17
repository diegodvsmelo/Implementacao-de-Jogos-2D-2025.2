using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpBar : MonoBehaviour
{
    [Header("Referências da UI")]
    public Image expBarFill;
    public TextMeshProUGUI expText;

    [Header("Configurações")]
    public float maxExp;
    private float currentExp;

    [Header("Animação (Opcional)")]
    public bool smoothTransition = true;
    public float transitionSpeed = 5f;

    private float targetFillAmount;

    void Start()
    {
        targetFillAmount = 0f;
        UpdateExpBar();
    }

    void Update()
    {
        if (smoothTransition && expBarFill != null)
        {
            expBarFill.fillAmount = Mathf.Lerp(
                expBarFill.fillAmount,
                targetFillAmount,
                Time.deltaTime * transitionSpeed
            );
        }
    }

    public void SetMaxExp(float newMax)
    {
        maxExp = Mathf.Max(1f, newMax);
        currentExp = Mathf.Clamp(currentExp, 0, maxExp);
        UpdateExpBar();
    }

    public void SetExp(float exp)
    {
        currentExp = Mathf.Clamp(exp, 0, maxExp);
        UpdateExpBar();
    }

    public void AddExp(float amount)
    {
        currentExp += amount;
        currentExp = Mathf.Clamp(currentExp, 0, maxExp);
        UpdateExpBar();
    }

    void UpdateExpBar()
    {
        float fillAmount = 0f;
        if (maxExp > 0) fillAmount = currentExp / maxExp;

        if (smoothTransition)
        {
            targetFillAmount = fillAmount;
        }
        else if (expBarFill != null)
        {
            expBarFill.fillAmount = fillAmount;
        }

        if (expText != null)
        {
            expText.text = $"{Mathf.FloorToInt(currentExp)} / {Mathf.FloorToInt(maxExp)}";
        }
    }

    public float GetCurrentExp()
    {
        return currentExp;
    }

    public float GetMaxExp()
    {
        return maxExp;
    }
}