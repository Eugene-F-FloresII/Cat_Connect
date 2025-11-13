using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int money;
    public float customerSatisfaction;
    public bool hasExpanded;
    public Vector2Int gridSize;
    public List<FurnitureData> placedFurniture = new List<FurnitureData>();
    public string saveDate;

    public SaveData()
    {
        saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[Serializable]
public class FurnitureData
{
    public string furnitureName;
    public FurnitureType furnitureType;
    public Vector2Int gridPosition;
    public Vector2Int size;
    public bool isWalkable;
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    private string saveFilePath;
    private const string SAVE_FILE_NAME = "savegame.json";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        saveFilePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
    }

    void Start()
    {
        LoadGame();
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();

        data.money = GameManager.Instance.money;
        data.customerSatisfaction = GameManager.Instance.customerSatisfaction;

        if (CafeExpansion.Instance != null)
        {
            data.hasExpanded = CafeExpansion.Instance.HasExpanded;
        }

        data.gridSize = GridManager.Instance.gridSize;

        Furniture[] allFurniture = FindObjectsOfType<Furniture>();
        foreach (Furniture furniture in allFurniture)
        {
            if (furniture.IsPlaced)
            {
                FurnitureData furnitureData = new FurnitureData
                {
                    furnitureName = furniture.furnitureName,
                    furnitureType = furniture.furnitureType,
                    gridPosition = furniture.GridPosition,
                    size = furniture.size,
                    isWalkable = furniture.isWalkable
                };
                data.placedFurniture.Add(furnitureData);
            }
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log($"Game saved! Location: {saveFilePath}");
        Debug.Log($"Saved {data.placedFurniture.Count} furniture items.");
    }

    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.Log("No save file found. Starting new game.");
            return;
        }

        try
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            GameManager.Instance.money = data.money;
            GameManager.Instance.customerSatisfaction = data.customerSatisfaction;

            if (data.hasExpanded && CafeExpansion.Instance != null)
            {
                CafeExpansion.Instance.LoadExpansionState(data.gridSize);
            }

            ClearAllFurniture();

            foreach (FurnitureData furnitureData in data.placedFurniture)
            {
                LoadFurniture(furnitureData);
            }

            Debug.Log($"Game loaded! Loaded {data.placedFurniture.Count} furniture items.");
            Debug.Log($"Save date: {data.saveDate}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
        }
    }

    void ClearAllFurniture()
    {
        Furniture[] allFurniture = FindObjectsOfType<Furniture>();
        foreach (Furniture furniture in allFurniture)
        {
            if (furniture.IsPlaced)
            {
                furniture.Remove();
                Destroy(furniture.gameObject);
            }
        }
    }

    void LoadFurniture(FurnitureData data)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Furniture/{data.furnitureName}");

        if (prefab == null)
        {
            Debug.LogWarning($"Could not find furniture prefab: {data.furnitureName}");
            return;
        }

        GameObject furnitureObj = Instantiate(prefab);
        Furniture furniture = furnitureObj.GetComponent<Furniture>();

        if (furniture != null)
        {
            furniture.Place(data.gridPosition);
            furniture.SetPlacedMode();
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted.");
        }
    }

    public bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGame();
        }
    }
}