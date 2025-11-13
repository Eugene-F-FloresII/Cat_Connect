using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAI : AIMovement
{
    [Header("Cat Stats")]
    public float satisfactionBoost = 0.5f;
    public float energyLevel = 100f;
    public float energyDrainRate = 5f;
    public float sleepEnergyRestoreRate = 10f;

    [Header("Behavior Timers")]
    public float wanderInterval = 3f;
    public float poopChance = 0.1f;
    public float poopCheckInterval = 10f;

    [Header("Prefabs")]
    public GameObject poopPrefab;

    private bool isSleeping = false;
    private bool isLookingForBed = false;
    private Furniture currentBed = null;

    [Header("Customer Interaction")]
    public float interactionRadius = 2f;
    public float satisfactionBoostPerSecond = 0.3f;
    public float customerCheckInterval = 2f;

    [Header("Visual Effects")]
    public GameObject happinessParticlePrefab;
    public AudioSource musicSource;
    public AudioClip[] audioClips;


    void Start()
    {
        musicSource.clip = audioClips[1];
        musicSource.Play();
        StartCoroutine(WanderRoutine());
        StartCoroutine(PoopCheckRoutine());
        StartCoroutine(EnergyManagement());
        StartCoroutine(CustomerInteractionRoutine());
    }

    

    IEnumerator WanderRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(wanderInterval);

            if (!isSleeping && !isLookingForBed && !IsMoving)
            {
                WanderToRandomPosition();
            }
        }
    }

    IEnumerator CustomerInteractionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(customerCheckInterval);

            if (!isSleeping)
            {
                BoostNearbyCustomers();
            }
        }
    }

    void BoostNearbyCustomers()
    {
        CustomerAI[] allCustomers = FindObjectsOfType<CustomerAI>();
        int customersHelped = 0;

        foreach (CustomerAI customer in allCustomers)
        {
            if (customer.currentState == CustomerState.Seated)
            {
                float distance = Vector3.Distance(transform.position, customer.transform.position);

                if (distance <= interactionRadius)
                {
                    GameManager.Instance.ModifyCustomerSatisfaction(satisfactionBoostPerSecond * customerCheckInterval);
                    customersHelped++;

                    if (happinessParticlePrefab != null)
                    {
                        Vector3 particlePos = (transform.position + customer.transform.position) / 2f;
                        GameObject particle = Instantiate(happinessParticlePrefab, particlePos, Quaternion.identity);
                        Destroy(particle, 1f);
                        musicSource.clip = audioClips[0];
                        musicSource.Play();
                    }
                }
            }
        }

        if (customersHelped > 0)
        {
            Debug.Log($"Cat made {customersHelped} customers happy!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

    void WanderToRandomPosition()
    {
        int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2Int randomPos = new Vector2Int(
                Random.Range(0, GridManager.Instance.gridSize.x),
                Random.Range(0, GridManager.Instance.gridSize.y)
            );

            if (GridManager.Instance.IsCellWalkable(randomPos))
            {
                MoveToPosition(randomPos);
                return;
            }
        }
    }

    IEnumerator PoopCheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(poopCheckInterval);

            if (!isSleeping && Random.value < poopChance)
            {
                SpawnPoop();
            }
        }
    }

    void SpawnPoop()
    {
        if (poopPrefab != null)
        {
            GameObject poop = Instantiate(poopPrefab, transform.position, Quaternion.identity);
            GameManager.Instance.RegisterPoop(poop);
        }
    }

    IEnumerator EnergyManagement()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (!isSleeping)
            {
                energyLevel -= energyDrainRate;
                Debug.Log($"Cat awake - Energy: {energyLevel:F1}");

                if (energyLevel <= 20f && !isLookingForBed)
                {
                    Debug.Log("Cat is tired, looking for bed!");
                    FindBedAndSleep();
                }
            }
            else
            {
                energyLevel += sleepEnergyRestoreRate;
                Debug.Log($"Cat sleeping - Energy: {energyLevel:F1}");

                GameManager.Instance.ModifyCustomerSatisfaction(-0.2f);

                if (energyLevel >= 100f)
                {
                    Debug.Log("Cat fully rested, waking up!");
                    WakeUp();
                }
            }

            energyLevel = Mathf.Clamp(energyLevel, 0, 100f);
        }
    }

    void FindBedAndSleep()
    {
        isLookingForBed = true;

        Furniture[] allFurniture = FindObjectsOfType<Furniture>();
        Furniture closestBed = null;
        float closestDistance = float.MaxValue;

        foreach (Furniture furniture in allFurniture)
        {
            if (furniture.furnitureType == FurnitureType.CatBed && furniture.IsPlaced)
            {
                float distance = Vector2.Distance(transform.position, furniture.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBed = furniture;
                }
            }
        }

        if (closestBed != null)
        {
            currentBed = closestBed;
            Debug.Log($"Cat found bed at {closestBed.GridPosition}, moving there.");
            MoveToPosition(closestBed.GridPosition);
        }
        else
        {
            Debug.LogWarning("Cat is tired but no cat bed is available!");
            isLookingForBed = false;
        }
    }

    void WakeUp()
    {
        isSleeping = false;
        isLookingForBed = false;
        currentBed = null;
        Debug.Log("Cat woke up from sleep! Energy fully restored.");
        StartCoroutine(WanderRoutine());
        StartCoroutine(PoopCheckRoutine());
        StartCoroutine(CustomerInteractionRoutine());

    }

    protected override void OnReachedDestination()
    {
        base.OnReachedDestination();

        if (isLookingForBed && currentBed != null)
        {
            isSleeping = true;
            isLookingForBed = false;
            Debug.Log("Cat reached bed and is now sleeping.");
            StartCoroutine(EnergyManagement());

        }
    }

    protected override void OnPathFailed()
    {
        base.OnPathFailed();

        if (isLookingForBed)
        {
            Debug.LogWarning("Cat couldn't reach the bed. Path blocked or bed not walkable.");
            isLookingForBed = false;
        }
    }
}
