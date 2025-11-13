using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITestController : MonoBehaviour
{
    [Header("Furniture Prefabs")]
    public GameObject tablePrefab;
    public GameObject chairPrefab;
    public GameObject catBedPrefab;
    public GameObject counterPrefab;

    public void OnPlaceTableClicked()
    {
        if (tablePrefab != null && CanAffordFurniture(100))
        {
            FurniturePlacementController.Instance.StartPlacement(tablePrefab);
        }
        else
        {
            Debug.Log("Not enough money for Table!");
        }
    }

    public void OnPlaceChairClicked()
    {
        if (chairPrefab != null && CanAffordFurniture(50))
        {
            FurniturePlacementController.Instance.StartPlacement(chairPrefab);
        }
        else
        {
            Debug.Log("Not enough money for Chair!");
        }
    }

    public void OnPlaceCatBedClicked()
    {
        if (catBedPrefab != null && CanAffordFurniture(150))
        {
            FurniturePlacementController.Instance.StartPlacement(catBedPrefab);
        }
        else
        {
            Debug.Log("Not enough money for Cat Bed!");
        }
    }

    public void OnPlaceCounterClicked()
    {
        if (counterPrefab != null && CanAffordFurniture(200))
        {
            FurniturePlacementController.Instance.StartPlacement(counterPrefab);
        }
        else
        {
            Debug.Log("Not enough money for Counter!");
        }
    }

    bool CanAffordFurniture(int cost)
    {
        return GameManager.Instance.CanAfford(cost);
    }
}
