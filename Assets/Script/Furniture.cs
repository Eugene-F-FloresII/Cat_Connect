using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FurnitureType
{
    Table,
    Chair,
    Counter,
    CatBed,
    Decoration
}

public class Furniture : MonoBehaviour
{
    [Header("Furniture Properties")]
    public string furnitureName;
    public FurnitureType furnitureType;
    public Vector2Int size = Vector2Int.one;
    public bool isWalkable = false;
    public int purchasePrice = 100;
    public int sellPrice = 50;

    [Header("References")]
    public SpriteRenderer spriteRenderer;

    private Vector2Int gridPosition;
    private bool isPlaced = false;

    public Vector2Int GridPosition => gridPosition;
    public bool IsPlaced => isPlaced;

    void Awake()
    {
        EnsureTableOccupancy();
    }

    void EnsureTableOccupancy()
    {
        if (furnitureType == FurnitureType.Table)
        {
            TableOccupancy occupancy = GetComponent<TableOccupancy>();
            if (occupancy == null)
            {
                gameObject.AddComponent<TableOccupancy>();
                Debug.Log($"Added TableOccupancy to {furnitureName}");
            }
        }
    }

    public void Place(Vector2Int position)
    {
        gridPosition = position;
        isPlaced = true;
        Vector3 worldPos = GridManager.Instance.CellToWorld(new Vector3Int(position.x, position.y, 0));
        transform.position = worldPos;

        GridManager.Instance.PlaceFurniture(gridPosition, size, this);

        EnsureTableOccupancy();
    }

    public void Remove()
    {
        if (isPlaced)
        {
            GridManager.Instance.RemoveFurniture(gridPosition, size);
            isPlaced = false;
        }
    }

    public void Sell()
    {
        Remove();
        GameManager.Instance.AddMoney(sellPrice);
        Destroy(gameObject);
    }

    public void SetPreviewMode(bool isValid)
    {
        Color previewColor = isValid ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
        spriteRenderer.color = previewColor;
    }

    public void SetPlacedMode()
    {
        spriteRenderer.color = Color.white;
    }
}
