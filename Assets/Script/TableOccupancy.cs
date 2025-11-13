using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableOccupancy : MonoBehaviour
{
    public bool IsOccupied { get; private set; } = false;
    private CustomerAI currentCustomer = null;

    public void OccupyTable(CustomerAI customer)
    {
        if (IsOccupied && currentCustomer != null && currentCustomer != customer)
        {
            Debug.LogWarning($"Table already occupied by another customer! Attempted double-booking prevented.");
            return;
        }

        IsOccupied = true;
        currentCustomer = customer;
        Debug.Log($"Table {gameObject.name} occupied by {customer.gameObject.name}");
    }

    public void ReleaseTable()
    {
        if (!IsOccupied)
        {
            Debug.LogWarning($"Attempted to release table {gameObject.name} that wasn't occupied.");
            return;
        }

        IsOccupied = false;
        currentCustomer = null;
        Debug.Log($"Table {gameObject.name} released and available.");
    }

    void Update()
    {
        if (IsOccupied && currentCustomer == null)
        {
            Debug.LogWarning($"Table {gameObject.name} marked occupied but customer is null! Releasing table.");
            ReleaseTable();
        }
    }
}
