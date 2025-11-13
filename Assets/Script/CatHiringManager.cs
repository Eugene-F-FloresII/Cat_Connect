using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatHiringManager : MonoBehaviour
{
    public static CatHiringManager Instance { get; private set; }

    [Header("Cat Types")]
    public CatData[] availableCats;

    [Header("Spawn Settings")]
    public Vector2Int defaultSpawnPosition = new Vector2Int(5, 5);

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeCatTypes();
    }

    void InitializeCatTypes()
    {
        availableCats = new CatData[5];

        availableCats[0] = new CatData
        {
            catName = "Stray Cat",
            description = "A basic street cat. Slow but cheap.",
            hireCost = 50,
            moveSpeedMultiplier = 0.8f,
            satisfactionBoostMultiplier = 0.8f,
            energyDrainMultiplier = 0.9f,
            catColor = new Color(0.7f, 0.7f, 0.7f)
        };

        availableCats[1] = new CatData
        {
            catName = "House Cat",
            description = "A friendly domestic cat. Balanced stats.",
            hireCost = 100,
            moveSpeedMultiplier = 1f,
            satisfactionBoostMultiplier = 1f,
            energyDrainMultiplier = 1f,
            catColor = new Color(1f, 0.8f, 0.6f)
        };

        availableCats[2] = new CatData
        {
            catName = "Siamese Cat",
            description = "Energetic and loves customers!",
            hireCost = 200,
            moveSpeedMultiplier = 1.2f,
            satisfactionBoostMultiplier = 1.5f,
            energyDrainMultiplier = 1.3f,
            catColor = new Color(0.9f, 0.85f, 0.7f)
        };

        availableCats[3] = new CatData
        {
            catName = "Persian Cat",
            description = "Elegant and efficient. Low energy drain.",
            hireCost = 350,
            moveSpeedMultiplier = 0.9f,
            satisfactionBoostMultiplier = 1.3f,
            energyDrainMultiplier = 0.7f,
            catColor = new Color(1f, 1f, 1f)
        };

        availableCats[4] = new CatData
        {
            catName = "Maine Coon",
            description = "The ultimate cafe cat! Premium stats.",
            hireCost = 500,
            moveSpeedMultiplier = 1.3f,
            satisfactionBoostMultiplier = 2f,
            energyDrainMultiplier = 0.8f,
            catColor = new Color(0.6f, 0.4f, 0.3f)
        };
    }

    public bool HireCat(CatData catData)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null!");
            return false;
        }

        if (!GameManager.Instance.CanHireCat())
        {
            Debug.LogWarning("Cannot hire more cats! Maximum limit reached.");
            return false;
        }

        if (!GameManager.Instance.CanAfford(catData.hireCost))
        {
            Debug.LogWarning($"Not enough money to hire {catData.catName}!");
            return false;
        }

        Vector2Int spawnPos = FindValidSpawnPosition();
        if (spawnPos == new Vector2Int(-1, -1))
        {
            Debug.LogWarning("No valid spawn position found for cat!");
            return false;
        }

        GameManager.Instance.SpendMoney(catData.hireCost);

        GameObject catPrefab = Resources.Load<GameObject>("Prefabs/Cat");
        if (catPrefab == null)
        {
            Debug.LogError("Cat prefab not found! Make sure Cat.prefab is in Assets/Resources/Prefabs/Cat.prefab");
            return false;
        }

        Vector3 worldPos = GridManager.Instance.CellToWorld(new Vector3Int(spawnPos.x, spawnPos.y, 0));
        GameObject newCat = Instantiate(catPrefab, worldPos, Quaternion.identity);
        newCat.name = catData.catName;

        CatAI catAI = newCat.GetComponent<CatAI>();
        if (catAI != null)
        {
            ApplyCatStats(catAI, catData);
        }
        else
        {
            Debug.LogWarning("CatAI component not found on spawned cat!");
        }

        SpriteRenderer spriteRenderer = newCat.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = catData.catColor;
        }

        GameManager.Instance.RegisterCat(newCat);

        Debug.Log($"Hired {catData.catName} for ${catData.hireCost}!");
        return true;
    }

    void ApplyCatStats(CatAI catAI, CatData data)
    {
        catAI.moveSpeed *= data.moveSpeedMultiplier;
        catAI.satisfactionBoostPerSecond *= data.satisfactionBoostMultiplier;
        catAI.energyDrainRate *= data.energyDrainMultiplier;
    }

    Vector2Int FindValidSpawnPosition()
    {
        if (GridManager.Instance == null)
        {
            Debug.LogError("GridManager.Instance is null!");
            return new Vector2Int(-1, -1);
        }

        for (int attempt = 0; attempt < 20; attempt++)
        {
            Vector2Int testPos = new Vector2Int(
                Random.Range(2, GridManager.Instance.gridSize.x - 2),
                Random.Range(2, GridManager.Instance.gridSize.y - 2)
            );

            if (GridManager.Instance.IsCellWalkable(testPos))
            {
                return testPos;
            }
        }

        if (GridManager.Instance.IsCellWalkable(defaultSpawnPosition))
        {
            return defaultSpawnPosition;
        }

        return new Vector2Int(-1, -1);
    }

    public CatData GetCatData(int index)
    {
        if (index >= 0 && index < availableCats.Length)
        {
            return availableCats[index];
        }
        return null;
    }
}
