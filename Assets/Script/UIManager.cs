using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD References")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI satisfactionText;
    public TextMeshProUGUI customerCountText;
    public TextMeshProUGUI taxTimerText;
    public TextMeshProUGUI poopCountText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        UpdateHUD();
    }

    void UpdateHUD()
    {
        if (GameManager.Instance != null)
        {
            moneyText.text = $"Money: ${GameManager.Instance.money}";

            float satisfaction = GameManager.Instance.customerSatisfaction;
            satisfactionText.text = $"Satisfaction: {satisfaction:F0}%";
            satisfactionText.color = GetSatisfactionColor(satisfaction);

            customerCountText.text = $"Customers: {GameManager.Instance.customerCount}";

            float timeUntilTax = GameManager.Instance.taxInterval - GameManager.Instance.GetTaxTimer();
            taxTimerText.text = $"Next Tax: {timeUntilTax:F0}s";

            int poopCount = GameManager.Instance.GetPoopCount();
            poopCountText.text = $"Poop: {poopCount}";
            
        }
    }

    Color GetSatisfactionColor(float satisfaction)
    {
        if (satisfaction >= 75f) return Color.green;
        if (satisfaction >= 50f) return Color.yellow;
        if (satisfaction >= 25f) return new Color(1f, 0.5f, 0f);
        return Color.red;
    }
}
