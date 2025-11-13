using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurniturePlacementController : MonoBehaviour
{
    public static FurniturePlacementController Instance { get; private set; }

    [Header("Placement Settings")]
    public LayerMask furnitureLayer;

    private Furniture currentFurniture;
    private bool isPlacementMode = false;
    private Camera mainCamera;

    public AudioSource mainAudioSource;

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

        mainCamera = Camera.main;
    }

    void Update()
    {
        if (isPlacementMode && currentFurniture != null)
        {
            HandlePlacementMode();
        }
        else
        {
            HandleSelectionMode();
        }
    }

    void HandlePlacementMode()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = GridManager.Instance.WorldToCell(mouseWorldPos);
        Vector2Int gridPos = new Vector2Int(cellPos.x, cellPos.y);

        Vector3 worldPos = GridManager.Instance.CellToWorld(cellPos);
        currentFurniture.transform.position = worldPos;

        bool canPlace = GridManager.Instance.CanPlaceFurniture(gridPos, currentFurniture.size);
        currentFurniture.SetPreviewMode(canPlace);

        mainAudioSource.Play();

        if (Input.GetMouseButtonDown(0))
        {
            if (canPlace && GameManager.Instance.CanAfford(currentFurniture.purchasePrice))
            {
                GameManager.Instance.SpendMoney(currentFurniture.purchasePrice);
                currentFurniture.Place(gridPos);
                currentFurniture.SetPlacedMode();
                ExitPlacementMode();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    void HandleSelectionMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, Mathf.Infinity, furnitureLayer);
            if (hit.collider != null)
            {
                Furniture furniture = hit.collider.GetComponent<Furniture>();
                if (furniture != null && furniture.IsPlaced)
                {
                    SelectFurniture(furniture);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, Mathf.Infinity, furnitureLayer);

            if (hit.collider != null)
            {
                Furniture furniture = hit.collider.GetComponent<Furniture>();
                if (furniture != null && furniture.IsPlaced)
                {
                    SellFurniture(furniture);
                }
            }
        }
    }

    void SellFurniture(Furniture furniture)
    {
        Debug.Log($"Sold {furniture.furnitureName} for ${furniture.sellPrice}");
        furniture.Sell();
    }

    public void StartPlacement(GameObject furniturePrefab)
    {
        if (currentFurniture != null)
        {
            Destroy(currentFurniture.gameObject);
        }

        GameObject newFurniture = Instantiate(furniturePrefab);
        currentFurniture = newFurniture.GetComponent<Furniture>();
        isPlacementMode = true;


    }

    void ExitPlacementMode()
    {
        currentFurniture = null;
        isPlacementMode = false;
    }

    void CancelPlacement()
    {
        if (currentFurniture != null)
        {
            Destroy(currentFurniture.gameObject);
        }
        ExitPlacementMode();
    }

    void SelectFurniture(Furniture furniture)
    {
        Debug.Log($"Selected: {furniture.furnitureName}");
    }
}
