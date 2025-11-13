using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }

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

    public List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int targetPos)
    {
        GridCell startCell = GridManager.Instance.GetCell(startPos);
        GridCell targetCell = GridManager.Instance.GetCell(targetPos);

        if (startCell == null || targetCell == null || !targetCell.isWalkable)
        {
            return null;
        }

        List<GridCell> openList = new List<GridCell>();
        HashSet<GridCell> closedList = new HashSet<GridCell>();
        openList.Add(startCell);

        while (openList.Count > 0)
        {
            GridCell currentCell = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentCell.fCost ||
                    (openList[i].fCost == currentCell.fCost && openList[i].hCost < currentCell.hCost))
                {
                    currentCell = openList[i];
                }
            }

            openList.Remove(currentCell);
            closedList.Add(currentCell);

            if (currentCell == targetCell)
            {
                return RetracePath(startCell, targetCell);
            }

            foreach (GridCell neighbor in GetNeighbors(currentCell))
            {
                if (!neighbor.isWalkable || closedList.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCost = currentCell.gCost + GetDistance(currentCell, neighbor);
                if (newMovementCost < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCost;
                    neighbor.hCost = GetDistance(neighbor, targetCell);
                    neighbor.parent = currentCell;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    List<Vector2Int> RetracePath(GridCell startCell, GridCell endCell)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        GridCell currentCell = endCell;

        while (currentCell != startCell)
        {
            path.Add(currentCell.gridPosition);
            currentCell = currentCell.parent;
        }

        path.Reverse();
        return path;
    }

    List<GridCell> GetNeighbors(GridCell cell)
    {
        List<GridCell> neighbors = new List<GridCell>();
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0)
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighborPos = cell.gridPosition + dir;
            GridCell neighbor = GridManager.Instance.GetCell(neighborPos);
            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    int GetDistance(GridCell cellA, GridCell cellB)
    {
        int distX = Mathf.Abs(cellA.gridPosition.x - cellB.gridPosition.x);
        int distY = Mathf.Abs(cellA.gridPosition.y - cellB.gridPosition.y);
        return distX + distY;
    }
}
