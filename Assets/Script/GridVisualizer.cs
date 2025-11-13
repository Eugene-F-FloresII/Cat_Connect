using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    public GridManager gridManager;
    public Color gridColor = new Color(1, 1, 1, 0.2f);

    void OnDrawGizmos()
    {
        if (gridManager == null) return;

        Gizmos.color = gridColor;

        for (int x = 0; x <= gridManager.gridSize.x; x++)
        {
            Vector3 start = gridManager.grid.CellToWorld(new Vector3Int(x, 0, 0));
            Vector3 end = gridManager.grid.CellToWorld(new Vector3Int(x, gridManager.gridSize.y, 0));
            Gizmos.DrawLine(start, end);
        }

        for (int y = 0; y <= gridManager.gridSize.y; y++)
        {
            Vector3 start = gridManager.grid.CellToWorld(new Vector3Int(0, y, 0));
            Vector3 end = gridManager.grid.CellToWorld(new Vector3Int(gridManager.gridSize.x, y, 0));
            Gizmos.DrawLine(start, end);
        }
    }

}
