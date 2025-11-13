using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Cooking Settings")]
    public int maxCookingSlots = 3;

    private List<CustomerAI> pendingOrders = new List<CustomerAI>();
    private List<Order> cookingOrders = new List<Order>();

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
        UpdateCookingOrders();
    }

    public void RegisterOrder(CustomerAI customer)
    {
        if (!pendingOrders.Contains(customer))
        {
            pendingOrders.Add(customer);
            Debug.Log($"Order registered: {customer.currentOrder.orderType}");
        }
    }

    public bool StartCooking(CustomerAI customer)
    {
        if (cookingOrders.Count >= maxCookingSlots)
        {
            Debug.LogWarning("All cooking slots are full!");
            return false;
        }

        if (!pendingOrders.Contains(customer))
        {
            Debug.LogWarning("Customer not found in pending orders!");
            return false;
        }

        pendingOrders.Remove(customer);
        cookingOrders.Add(customer.currentOrder);

        Debug.Log($"Started cooking {customer.currentOrder.orderType}");
        return true;
    }

    void UpdateCookingOrders()
    {
        for (int i = cookingOrders.Count - 1; i >= 0; i--)
        {
            Order order = cookingOrders[i];

            if (!order.isReady)
            {
                order.cookTimer += Time.deltaTime;

                if (order.cookTimer >= order.cookTime)
                {
                    order.isReady = true;
                    OnOrderReady(order);
                }
            }
        }
    }

    void OnOrderReady(Order order)
    {
        TrayManager.Instance.AddItemToTray(order.orderType);
        cookingOrders.Remove(order);
        Debug.Log($"{order.orderType} is ready!");
    }

    public List<CustomerAI> GetPendingOrders()
    {
        pendingOrders.RemoveAll(c => c == null);
        return pendingOrders;
    }

    public int GetCookingCount()
    {
        return cookingOrders.Count;
    }

    public List<Order> GetCookingOrders()
    {
        return cookingOrders;
    }
}
