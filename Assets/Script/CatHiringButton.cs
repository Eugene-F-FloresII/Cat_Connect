using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatHiringButton : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI statsText;
    public Button hireButton;
    public Image catColorPreview;

    private CatData catData;

    public void SetupButton(CatData data)
    {
        catData = data;

        nameText.text = catData.catName;
        descriptionText.text = catData.description;
        costText.text = $"${catData.hireCost}";

        statsText.text = $"Speed: {catData.moveSpeedMultiplier:F1}x\n" +
                        $"Boost: {catData.satisfactionBoostMultiplier:F1}x\n" +
                        $"Energy: {catData.energyDrainMultiplier:F1}x";

        if (catColorPreview != null)
        {
            catColorPreview.color = catData.catColor;
        }

        hireButton.onClick.AddListener(OnHireClicked);

        UpdateButtonState();
    }

    void OnHireClicked()
    {
        bool success = CatHiringManager.Instance.HireCat(catData);

        if (success)
        {
            FindObjectOfType<CatHiringUIManager>()?.RefreshCatHiringUI();
        }
    }

    public void UpdateButtonState()
    {
        bool canAfford = GameManager.Instance.CanAfford(catData.hireCost);
        bool canHire = GameManager.Instance.CanHireCat();

        hireButton.interactable = canAfford && canHire;

        if (!canHire)
        {
            costText.text = "MAX CATS";
            costText.color = Color.red;
        }
        else if (!canAfford)
        {
            costText.text = $"${catData.hireCost}";
            costText.color = Color.red;
        }
        else
        {
            costText.text = $"${catData.hireCost}";
            costText.color = Color.green;
        }
    }
}
