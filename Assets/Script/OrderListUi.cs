using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderListUi : MonoBehaviour
{
    [Header("UI References")]
    public Transform orderListContainer;
    public GameObject orderButtonPrefab;

    private List<GameObject> orderButtons = new List<GameObject>();
    private int lastOrderCount = -1;

    void Update()
    {
        if (OrderManager.Instance == null) return;

        int currentOrderCount = OrderManager.Instance.GetPendingOrders().Count;

        if (currentOrderCount != lastOrderCount)
        {
            lastOrderCount = currentOrderCount;
            RefreshOrderList();
        }
    }

    void RefreshOrderList()
    {
        foreach (GameObject btn in orderButtons)
        {
            if (btn != null)
            {
                Destroy(btn);
            }
        }
        orderButtons.Clear();

        List<CustomerAI> pendingOrders = OrderManager.Instance.GetPendingOrders();

        foreach (CustomerAI customer in pendingOrders)
        {
            if (customer == null) continue;

            GameObject btnObj = Instantiate(orderButtonPrefab, orderListContainer);

            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                string orderName = (customer.currentOrder.orderType == OrderType.Coffee) ? "COFFEE" : "FOOD";
                btnText.text = $"Cook {orderName}";
            }

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                CustomerAI capturedCustomer = customer;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnCookButtonClicked(capturedCustomer));
            }

            orderButtons.Add(btnObj);
        }
    }

    void OnCookButtonClicked(CustomerAI customer)
    {
        if (customer != null && OrderManager.Instance != null)
        {
            bool success = OrderManager.Instance.StartCooking(customer);
            if (success)
            {
                RefreshOrderList();
            }
        }
    }

}
