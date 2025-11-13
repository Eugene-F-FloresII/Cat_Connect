using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayManager : MonoBehaviour
{
    public static TrayManager Instance { get; private set; }

    [Header("Tray References")]
    public Transform coffeeTraySlotsContainer;
    public Transform foodTraySlotsContainer;

    [Header("Prefabs")]
    public GameObject draggableItemPrefab;

    private List<GameObject> coffeeItems = new List<GameObject>();
    private List<GameObject> foodItems = new List<GameObject>();

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

    public void AddItemToTray(OrderType orderType)
    {
        Transform targetContainer = (orderType == OrderType.Coffee) ? coffeeTraySlotsContainer : foodTraySlotsContainer;

        if (targetContainer.childCount >= 5)
        {
            Debug.LogWarning($"{orderType} tray is full!");
            return;
        }

        GameObject newItem = Instantiate(draggableItemPrefab, targetContainer);
        DraggableItem draggable = newItem.GetComponent<DraggableItem>();

        if (draggable != null)
        {
            draggable.SetOrderType(orderType);
        }

        if (orderType == OrderType.Coffee)
        {
            coffeeItems.Add(newItem);
        }
        else
        {
            foodItems.Add(newItem);
        }

        Debug.Log($"{orderType} added to tray!");
    }

    public void RemoveItemFromTray(GameObject item, OrderType orderType)
    {
        if (orderType == OrderType.Coffee)
        {
            coffeeItems.Remove(item);
        }
        else
        {
            foodItems.Remove(item);
        }
    }
}
