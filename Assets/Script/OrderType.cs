using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum OrderType
{
    None,
    Coffee,
    Food
}

[System.Serializable]
public class Order
{
    public OrderType orderType;
    public float cookTime;
    public int bonusMoney;
    public bool isReady;
    public float cookTimer;

    public Order(OrderType type)
    {
        orderType = type;

        switch (type)
        {
            case OrderType.Coffee:
                cookTime = 5f;
                bonusMoney = 5;
                break;
            case OrderType.Food:
                cookTime = 10f;
                bonusMoney = 10;
                break;
            default:
                cookTime = 0f;
                bonusMoney = 0;
                break;
        }

        isReady = false;
        cookTimer = 0f;
    }
}
