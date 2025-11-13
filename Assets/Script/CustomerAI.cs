using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CustomerAI : AIMovement
{
    [Header("Customer Stats")]
    public float moneyPerSecond = 1f;
    public float stayDuration = 40f;
    public float satisfactionCheckInterval = 10f;

    [Header("State")]
    public CustomerState currentState = CustomerState.EnteringCafe;

    [Header("Order System")]
    public Order currentOrder;
    public GameObject orderBubblePrefab;
    private GameObject orderBubbleInstance;
    public Sprite[] customerorder;
    public AudioSource musicAudio;

    private Furniture currentTable = null;
    private float timeAtTable = 0f;
    private bool isLeaving = false;
    private bool orderDelivered = false;

    void Start()
    {
        StartCoroutine(CustomerLifecycle());
        StartCoroutine(MoneyGeneration());
        StartCoroutine(SatisfactionCheck());
    }

    IEnumerator CustomerLifecycle()
    {
        yield return new WaitForSeconds(0.5f);

        currentState = CustomerState.FindingTable;
        Furniture table = FindAvailableTable();

        if (table != null)
        {
            currentTable = table;

            TableOccupancy occupancy = currentTable.GetComponent<TableOccupancy>();
            if (occupancy == null)
            {
                occupancy = currentTable.gameObject.AddComponent<TableOccupancy>();
                Debug.LogWarning($"TableOccupancy missing on {currentTable.furnitureName}! Added automatically.");
            }

            occupancy.OccupyTable(this);
            MoveToPosition(table.GridPosition);
        }
        else
        {
            Debug.Log("Customer leaving - no available tables!");
            LeaveAndDestroy();
        }
    }

    IEnumerator MoneyGeneration()
    {
        while (!isLeaving)
        {
            yield return new WaitForSeconds(1f);

            if (currentState == CustomerState.Seated)
            {
                timeAtTable += 1f;

                int moneyAmount = Mathf.FloorToInt(moneyPerSecond);
                if (moneyAmount > 0)
                {
                    GameManager.Instance.AddMoney(moneyAmount);
                    Debug.Log($"Customer paid ${moneyAmount}!");
                }

                if (timeAtTable >= stayDuration)
                {
                    Debug.Log("Customer finished their stay.");
                    LeaveAndDestroy();
                }
            }
        }
    }

    IEnumerator SatisfactionCheck()
    {
        while (!isLeaving)
        {
            yield return new WaitForSeconds(satisfactionCheckInterval);

            if (currentState == CustomerState.Seated)
            {
                int poopCount = GameManager.Instance.GetPoopCount();
                float satisfactionLoss = poopCount * 0.5f;
                GameManager.Instance.ModifyCustomerSatisfaction(-satisfactionLoss);
            }
        }
    }

    Furniture FindAvailableTable()
    {
        Furniture[] allFurniture = FindObjectsOfType<Furniture>();
        foreach (Furniture furniture in allFurniture)
        {
            if (furniture.furnitureType == FurnitureType.Table && furniture.IsPlaced)
            {
                TableOccupancy occupancy = furniture.GetComponent<TableOccupancy>();

                if (occupancy == null)
                {
                    occupancy = furniture.gameObject.AddComponent<TableOccupancy>();
                    Debug.LogWarning($"TableOccupancy missing! Added to {furniture.furnitureName}");
                }

                if (!occupancy.IsOccupied)
                {
                    return furniture;
                }
            }
        }
        return null;
    }

    protected override void OnReachedDestination()
    {
        base.OnReachedDestination();

        if (currentState == CustomerState.FindingTable)
        {
            currentState = CustomerState.Seated;
            Debug.Log("Customer seated at table.");

            PlaceOrder();
        }
        else if (currentState == CustomerState.Leaving)
        {
            Destroy(gameObject);
        }
    }

    void PlaceOrder()
    {
        OrderType randomOrder = (Random.value > 0.5f) ? OrderType.Coffee : OrderType.Food;
        currentOrder = new Order(randomOrder);

        ShowOrderBubble();
        OrderManager.Instance.RegisterOrder(this);

        Debug.Log($"Customer ordered: {currentOrder.orderType}");
    }

    void ShowOrderBubble()
    {
        if (orderBubblePrefab != null)
        {
            orderBubbleInstance = Instantiate(orderBubblePrefab, transform);
            orderBubbleInstance.transform.localPosition = new Vector3(0, 0.8f, 0);

            OrderBubble bubble = orderBubbleInstance.GetComponent<OrderBubble>();
            if (bubble != null)
            {
                bubble.SetOrder(currentOrder.orderType);
            }
        }
    }

    public void DeliverOrder(OrderType deliveredType)
    {
        if (currentOrder == null || orderDelivered)
        {
            Debug.LogWarning("Customer has no active order or already received it!");
            return;
        }

        if (deliveredType == currentOrder.orderType)
        {
            orderDelivered = true;
            GameManager.Instance.AddMoney(currentOrder.bonusMoney);
            GameManager.Instance.ModifyCustomerSatisfaction(5f);

            if (musicAudio != null)
            {
                musicAudio.Play();
            }

            if (orderBubbleInstance != null)
            {
                Destroy(orderBubbleInstance);
            }

            Debug.Log($"Order delivered correctly! Bonus: ${currentOrder.bonusMoney}");
        }
        else
        {
            GameManager.Instance.ModifyCustomerSatisfaction(-10f);
            Debug.LogWarning("Wrong order delivered!");
        }
    }

    protected override void OnPathFailed()
    {
        base.OnPathFailed();
        Debug.LogWarning("Customer couldn't reach destination!");
        LeaveAndDestroy();
    }

    void LeaveAndDestroy()
    {
        if (isLeaving) return;

        isLeaving = true;
        currentState = CustomerState.Leaving;

        if (currentTable != null)
        {
            TableOccupancy occupancy = currentTable.GetComponent<TableOccupancy>();
            if (occupancy != null)
            {
                occupancy.ReleaseTable();
            }
            else
            {
                Debug.LogError($"Table {currentTable.furnitureName} has no TableOccupancy component!");
            }
        }

        GameManager.Instance.RemoveCustomer();

        if (orderBubbleInstance != null)
        {
            Destroy(orderBubbleInstance);
        }

        Vector2Int exitPos = new Vector2Int(0, GridManager.Instance.gridSize.y / 2);
        MoveToPosition(exitPos);

        Destroy(gameObject, 5f);
    }
}

public enum CustomerState
{
    EnteringCafe,
    FindingTable,
    Seated,
    Leaving,
}