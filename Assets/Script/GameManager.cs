using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Economy")]
    public int money = 1000;
    public float taxRate = 0.1f;
    public float taxInterval = 60f;

    [Header("Customer Satisfaction")]
    public float customerSatisfaction = 100f;
    public int customerCount = 0;

    [Header("Poop Management")]
    public int poopCount = 0;
    private List<GameObject> activePoops = new List<GameObject>();

    [Header("Cat Management")]
    public int maxCats = 5;
    private List<GameObject> hiredCats = new List<GameObject>();

    private float taxTimer = 0f;

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
        HandleTaxes();
    }

    void HandleTaxes()
    {
        taxTimer += Time.deltaTime;
        if (taxTimer >= taxInterval)
        {
            taxTimer = 0f;
            int taxAmount = Mathf.FloorToInt(money * taxRate);
            SpendMoney(taxAmount);
            Debug.Log($"Taxes paid: {taxAmount}");
        }
    }

    public bool CanAfford(int amount)
    {
        return money >= amount;
    }

    public void SpendMoney(int amount)
    {
        money -= amount;
        money = Mathf.Max(0, money);
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void ModifyCustomerSatisfaction(float amount)
    {
        customerSatisfaction += amount;
        customerSatisfaction = Mathf.Clamp(customerSatisfaction, 0, 100);
    }

    public void RegisterPoop(GameObject poop)
    {
        if (!activePoops.Contains(poop))
        {
            activePoops.Add(poop);
            poopCount = activePoops.Count;
            Debug.Log($"Poop registered. Total poop: {poopCount}");
        }
    }

    public void UnregisterPoop(GameObject poop)
    {
        if (activePoops.Contains(poop))
        {
            activePoops.Remove(poop);
            poopCount = activePoops.Count;
            Debug.Log($"Poop removed. Total poop: {poopCount}");
        }
    }

    public int GetPoopCount()
    {
        activePoops.RemoveAll(poop => poop == null);
        poopCount = activePoops.Count;
        return poopCount;
    }

    public float GetTaxTimer()
    {
        return taxTimer;
    }

    public void AddCustomer()
    {
        customerCount++;
    }

    public void RemoveCustomer()
    {
        customerCount--;
        customerCount = Mathf.Max(0, customerCount);
    }

    public void RegisterCat(GameObject cat)
    {
        if (!hiredCats.Contains(cat))
        {
            hiredCats.Add(cat);
            Debug.Log($"Cat hired! Total cats: {GetHiredCatsCount()}");
        }
    }

    public void UnregisterCat(GameObject cat)
    {
        if (hiredCats.Contains(cat))
        {
            hiredCats.Remove(cat);
            Debug.Log($"Cat removed. Total cats: {GetHiredCatsCount()}");
        }
    }

    public int GetHiredCatsCount()
    {
        hiredCats.RemoveAll(cat => cat == null);
        return hiredCats.Count;
    }

    public bool CanHireCat()
    {
        return GetHiredCatsCount() < maxCats;
    }
}
