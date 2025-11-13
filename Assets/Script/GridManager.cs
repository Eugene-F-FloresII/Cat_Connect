using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public Grid grid;
    public Vector2Int gridSize = new Vector2Int(10, 4);

    private Dictionary<Vector2Int, GridCell> cells = new Dictionary<Vector2Int, GridCell>();

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

        InitializeGrid();
    }

    void InitializeGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                cells[gridPos] = new GridCell(gridPos);
            }
        }
    }

    public void ExpandGrid(Vector2Int newSize)
    {
        if (newSize.x < gridSize.x || newSize.y < gridSize.y)
        {
            Debug.LogWarning("New grid size must be larger than current size!");
            return;
        }

        Vector2Int oldSize = gridSize;
        gridSize = newSize;

        for (int x = 0; x < newSize.x; x++)
        {
            for (int y = 0; y < newSize.y; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);

                if (!cells.ContainsKey(gridPos))
                {
                    cells[gridPos] = new GridCell(gridPos);
                }
            }
        }

        Debug.Log($"Grid expanded from {oldSize} to {newSize}. Added {(newSize.x * newSize.y) - (oldSize.x * oldSize.y)} new cells.");
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return grid.WorldToCell(worldPosition);
    }

    public Vector3 CellToWorld(Vector3Int cellPosition)
    {
        return grid.CellToWorld(cellPosition);
    }

    public bool IsCellWalkable(Vector2Int cellPos)
    {
        if (!cells.ContainsKey(cellPos)) return false;
        return cells[cellPos].isWalkable;
    }

    public bool CanPlaceFurniture(Vector2Int cellPos, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int checkPos = cellPos + new Vector2Int(x, y);
                if (!cells.ContainsKey(checkPos) || cells[checkPos].furniture != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void PlaceFurniture(Vector2Int cellPos, Vector2Int size, Furniture furniture)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int checkPos = cellPos + new Vector2Int(x, y);
                if (cells.ContainsKey(checkPos))
                {
                    cells[checkPos].furniture = furniture;
                    cells[checkPos].isWalkable = furniture.isWalkable;
                }
            }
        }
    }

    public void RemoveFurniture(Vector2Int cellPos, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int checkPos = cellPos + new Vector2Int(x, y);
                if (cells.ContainsKey(checkPos))
                {
                    cells[checkPos].furniture = null;
                    cells[checkPos].isWalkable = true;
                }
            }
        }
    }

    public GridCell GetCell(Vector2Int cellPos)
    {
        return cells.ContainsKey(cellPos) ? cells[cellPos] : null;
    }
}

public class GridCell
{
    public Vector2Int gridPosition;
    public bool isWalkable = true;
    public Furniture furniture = null;

    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }
    public GridCell parent;

    public GridCell(Vector2Int position)
    {
        this.gridPosition = position;
    }
}
