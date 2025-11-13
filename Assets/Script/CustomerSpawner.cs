using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public static CustomerSpawner Instance { get; private set; }

    [Header("Spawning Settings")]
    public GameObject[] customerPrefabs;
    public Vector2Int spawnPosition = new Vector2Int(4, 0);
    public float spawnInterval = 15f;
    public int maxCustomers = 5;

    public AudioSource musicaudio;

    [Header("Satisfaction Influence")]
    public bool spawnBasedOnSatisfaction = true;

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

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (CanSpawnCustomer())
            {
                SpawnRandomCustomer();
            }
        }
    }

    bool CanSpawnCustomer()
    {
        if (GameManager.Instance.customerCount >= maxCustomers)
        {
            return false;
        }

        int availableTables = CountAvailableTables();
        if (availableTables <= 0)
        {
            Debug.Log("No available tables for new customers.");
            return false;
        }

        if (spawnBasedOnSatisfaction)
        {
            float satisfaction = GameManager.Instance.customerSatisfaction;
            float spawnChance = satisfaction / 100f;

            if (Random.value > spawnChance)
            {
                Debug.Log($"Low satisfaction ({satisfaction}%) prevented customer spawn.");
                return false;
            }
        }

        return true;
    }

    int CountAvailableTables()
    {
        int count = 0;
        Furniture[] allFurniture = FindObjectsOfType<Furniture>();

        foreach (Furniture furniture in allFurniture)
        {
            if (furniture.furnitureType == FurnitureType.Table && furniture.IsPlaced)
            {
                TableOccupancy occupancy = furniture.GetComponent<TableOccupancy>();
                if (occupancy != null && !occupancy.IsOccupied)
                {
                    count++;
                }
            }
        }

        return count;
    }

    void SpawnRandomCustomer()
    {
        if (customerPrefabs == null || customerPrefabs.Length == 0)
        {
            Debug.LogError("No customer prefabs assigned!");
            return;
        }

        GameObject randomCustomerPrefab = customerPrefabs[Random.Range(0, customerPrefabs.Length)];

        if (randomCustomerPrefab == null)
        {
            Debug.LogError("Selected customer prefab is null!");
            return;
        }

        Vector3 spawnWorldPos = GridManager.Instance.CellToWorld(new Vector3Int(spawnPosition.x, spawnPosition.y, 0));
        spawnWorldPos += new Vector3(0.5f, 0.5f, 0);

        GameObject customer = Instantiate(randomCustomerPrefab, spawnWorldPos, Quaternion.identity);
        GameManager.Instance.AddCustomer();

        Debug.Log($"Customer spawned ({randomCustomerPrefab.name})! Total: {GameManager.Instance.customerCount}");
        musicaudio.Play();
    }
}
