using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject upgradesPanel;
    public GameObject upgradeButtonPrefab;
    public Transform upgradeButtonsContainer;

    void Start()
    {
        PopulateUpgradesUI();
        upgradesPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            ToggleUpgradesPanel();
        }
    }

    public void ToggleUpgradesPanel()
    {
        upgradesPanel.SetActive(!upgradesPanel.activeSelf);
    }

    void PopulateUpgradesUI()
    {
        foreach (Transform child in upgradeButtonsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Upgrade upgrade in UpgradeSystem.Instance.availableUpgrades)
        {
            GameObject buttonObj = Instantiate(upgradeButtonPrefab, upgradeButtonsContainer);
            UpgradeButton upgradeButton = buttonObj.GetComponent<UpgradeButton>();

            if (upgradeButton != null)
            {
                upgradeButton.SetupButton(upgrade);
            }
        }
    }

    public void RefreshUpgradesUI()
    {
        PopulateUpgradesUI();
    }
}
