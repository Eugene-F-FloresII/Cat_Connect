using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CatData
{
    public string catName;
    public string description;
    public int hireCost;
    public GameObject catPrefab;
    public Sprite catIcon;

    public float moveSpeedMultiplier = 1f;
    public float satisfactionBoostMultiplier = 1f;
    public float energyDrainMultiplier = 1f;
    public Color catColor = Color.white;
}
