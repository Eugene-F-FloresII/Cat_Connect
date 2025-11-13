using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;

    private List<Vector2Int> currentPath;
    private int currentWaypointIndex = 0;
    private bool isMoving = false;

    public bool IsMoving => isMoving;

    public void MoveToPosition(Vector2Int targetGridPos)
    {
        if (isMoving)
        {
            StopMoving();
        }

        Vector3Int currentCell = GridManager.Instance.WorldToCell(transform.position);
        Vector2Int startPos = new Vector2Int(currentCell.x, currentCell.y);

        currentPath = Pathfinding.Instance.FindPath(startPos, targetGridPos);

        if (currentPath != null && currentPath.Count > 0)
        {
            currentWaypointIndex = 0;
            isMoving = true;
            StartCoroutine(FollowPath());
        }
        else
        {
            OnPathFailed();
        }
    }

    IEnumerator FollowPath()
    {
        while (isMoving && currentWaypointIndex < currentPath.Count)
        {
            Vector2Int targetGridPos = currentPath[currentWaypointIndex];
            Vector3 targetWorldPos = GridManager.Instance.CellToWorld(new Vector3Int(targetGridPos.x, targetGridPos.y, 0));
            targetWorldPos += new Vector3(0.09f, 0.175f, 0);

            while (Vector3.Distance(transform.position, targetWorldPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentWaypointIndex++;
        }

        isMoving = false;
        OnReachedDestination();
    }

    public void StopMoving()
    {
        isMoving = false;
        StopAllCoroutines();
    }

    protected virtual void OnReachedDestination()
    {
    }

    protected virtual void OnPathFailed()
    {
    }
}
