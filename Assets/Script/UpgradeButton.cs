using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText;
    public Button purchaseButton;
    public GameObject purchasedOverlay;

    private Upgrade upgrade;

    public void SetupButton(Upgrade upgradeData)
    {
        upgrade = upgradeData;

        nameText.text = upgrade.upgradeName;
        descriptionText.text = upgrade.description;
        costText.text = $"${upgrade.cost}";

        purchaseButton.onClick.AddListener(OnPurchaseClicked);

        UpdateButtonState();
    }

    void OnPurchaseClicked()
    {
        bool success = UpgradeSystem.Instance.PurchaseUpgrade(upgrade);

        if (success)
        {
            UpdateButtonState();
            FindObjectOfType<UpgradesUIManager>()?.RefreshUpgradesUI();
        }
    }

    void UpdateButtonState()
    {
        if (upgrade.isPurchased)
        {
            purchaseButton.interactable = false;
            costText.text = "OWNED";
            costText.color = Color.green;

            if (purchasedOverlay != null)
            {
                purchasedOverlay.SetActive(true);
            }
        }
        else
        {
            bool canAfford = GameManager.Instance.money >= upgrade.cost;
            purchaseButton.interactable = canAfford;
            costText.color = canAfford ? Color.white : Color.red;
        }
    }
}
