using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Upgrade
{
    public string upgradeName;
    public string description;
    public int cost;
    public bool isPurchased = false;
    public UpgradeType upgradeType;
}

public enum UpgradeType
{
    FasterCats,
    HappierCats,
    PremiumTables,
    AutoCleaner,
    SatisfactionBoost,
    LongerCustomerStay,
    MoreCustomerSpawns,
    CheaperFurniture,
    BiggerTips
}
public class UpgradeSystem : MonoBehaviour
{
    public static UpgradeSystem Instance { get; private set; }

    [Header("Available Upgrades")]
    public Upgrade[] availableUpgrades;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeUpgrades();
    }

    void InitializeUpgrades()
    {
        availableUpgrades = new Upgrade[]
        {
            new Upgrade
            {
                upgradeName = "Faster Cats",
                description = "Cats move 50% faster",
                cost = 100,
                upgradeType = UpgradeType.FasterCats
            },
            new Upgrade
            {
                upgradeName = "Happy Cats",
                description = "Cats boost satisfaction 2x more",
                cost = 150,
                upgradeType = UpgradeType.HappierCats
            },
            new Upgrade
            {
                upgradeName = "Premium Tables",
                description = "Tables earn +$2 per minute",
                cost = 200,
                upgradeType = UpgradeType.PremiumTables
            },
            new Upgrade
            {
                upgradeName = "Auto Cleaner",
                description = "Poop auto-cleans after 10 seconds",
                cost = 300,
                upgradeType = UpgradeType.AutoCleaner
            },
            new Upgrade
            {
                upgradeName = "Cozy Atmosphere",
                description = "+20% base satisfaction",
                cost = 250,
                upgradeType = UpgradeType.SatisfactionBoost
            },
            new Upgrade
            {
                upgradeName = "Comfy Chairs",
                description = "Customers stay 50% longer",
                cost = 180,
                upgradeType = UpgradeType.LongerCustomerStay
            },
            new Upgrade
            {
                upgradeName = "Advertisement",
                description = "More customers spawn",
                cost = 220,
                upgradeType = UpgradeType.MoreCustomerSpawns
            },
            new Upgrade
            {
                upgradeName = "Bulk Discount",
                description = "Furniture costs 20% less",
                cost = 120,
                upgradeType = UpgradeType.CheaperFurniture
            },
            new Upgrade
            {
                upgradeName = "Tip Jar",
                description = "Customers pay 50% more",
                cost = 350,
                upgradeType = UpgradeType.BiggerTips
            }
        };
    }

    public bool PurchaseUpgrade(Upgrade upgrade)
    {
        if (upgrade.isPurchased)
        {
            Debug.LogWarning($"{upgrade.upgradeName} already purchased!");
            return false;
        }

        if (GameManager.Instance.money < upgrade.cost)
        {
            Debug.LogWarning($"Not enough money for {upgrade.upgradeName}!");
            return false;
        }

        GameManager.Instance.AddMoney(-upgrade.cost);
        upgrade.isPurchased = true;
        ApplyUpgrade(upgrade);

        Debug.Log($"Purchased: {upgrade.upgradeName}!");
        return true;
    }

    void ApplyUpgrade(Upgrade upgrade)
    {
        switch (upgrade.upgradeType)
        {
            case UpgradeType.FasterCats:
                ApplyFasterCats();
                break;
            case UpgradeType.HappierCats:
                ApplyHappierCats();
                break;
            case UpgradeType.PremiumTables:
                ApplyPremiumTables();
                break;
            case UpgradeType.AutoCleaner:
                ApplyAutoCleaner();
                break;
            case UpgradeType.SatisfactionBoost:
                ApplySatisfactionBoost();
                break;
            case UpgradeType.LongerCustomerStay:
                ApplyLongerCustomerStay();
                break;
            case UpgradeType.MoreCustomerSpawns:
                ApplyMoreCustomerSpawns();
                break;
            case UpgradeType.CheaperFurniture:
                ApplyCheaperFurniture();
                break;
            case UpgradeType.BiggerTips:
                ApplyBiggerTips();
                break;
        }
    }

    void ApplyFasterCats()
    {
        CatAI[] cats = FindObjectsOfType<CatAI>();
        foreach (CatAI cat in cats)
        {
            cat.moveSpeed *= 1.5f;
        }
    }

    void ApplyHappierCats()
    {
        CatAI[] cats = FindObjectsOfType<CatAI>();
        foreach (CatAI cat in cats)
        {
            cat.satisfactionBoostPerSecond *= 2f;
        }
    }

    void ApplyPremiumTables()
    {
        CustomerAI[] customers = FindObjectsOfType<CustomerAI>();
        foreach (CustomerAI customer in customers)
        {
            customer.moneyPerSecond += 2f;
        }
    }

    void ApplyAutoCleaner()
    {
        StartCoroutine(AutoCleanRoutine());
    }

    System.Collections.IEnumerator AutoCleanRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);

            Poop[] allPoop = FindObjectsOfType<Poop>();
            foreach (Poop poop in allPoop)
            {
                poop.CleanPoop();
            }
        }
    }

    void ApplySatisfactionBoost()
    {
        GameManager.Instance.ModifyCustomerSatisfaction(20f);
    }

    void ApplyLongerCustomerStay()
    {
        CustomerAI[] customers = FindObjectsOfType<CustomerAI>();
        foreach (CustomerAI customer in customers)
        {
            customer.stayDuration *= 1.5f;
        }
    }

    void ApplyMoreCustomerSpawns()
    {
        if (CustomerSpawner.Instance != null)
        {
            CustomerSpawner.Instance.spawnInterval *= 0.7f;
        }
    }

    void ApplyCheaperFurniture()
    {
        Furniture[] allFurniture = FindObjectsOfType<Furniture>();
        foreach (Furniture furniture in allFurniture)
        {
            furniture.purchasePrice = Mathf.RoundToInt(furniture.purchasePrice * 0.8f);
        }
    }

    void ApplyBiggerTips()
    {
        CustomerAI[] customers = FindObjectsOfType<CustomerAI>();
        foreach (CustomerAI customer in customers)
        {
            customer.moneyPerSecond *= 1.5f;
        }
    }

    public bool IsUpgradePurchased(UpgradeType type)
    {
        foreach (Upgrade upgrade in availableUpgrades)
        {
            if (upgrade.upgradeType == type && upgrade.isPurchased)
            {
                return true;
            }
        }
        return false;
    }

    public Upgrade GetUpgrade(UpgradeType type)
    {
        foreach (Upgrade upgrade in availableUpgrades)
        {
            if (upgrade.upgradeType == type)
            {
                return upgrade;
            }
        }
        return null;
    }
}
