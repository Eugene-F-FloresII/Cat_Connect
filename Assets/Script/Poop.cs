using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poop : MonoBehaviour
{
    [Header("Poop Settings")]
    public float satisfactionDrainRate = 0.5f;
    public LayerMask poopLayer;

    private SpriteRenderer spriteRenderer;
    private bool isBeingCleaned = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPoop(gameObject);
        }
    }

    void Update()
    {
        if (!isBeingCleaned)
        {
            GameManager.Instance?.ModifyCustomerSatisfaction(-satisfactionDrainRate * Time.deltaTime);
        }
    }

    void OnMouseDown()
    {
        CleanPoop();
    }

    public void CleanPoop()
    {
        if (isBeingCleaned) return;

        isBeingCleaned = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterPoop(gameObject);
            GameManager.Instance.ModifyCustomerSatisfaction(5f);
        }

        Debug.Log("Poop cleaned! +5 satisfaction");
        Destroy(gameObject);
    }
}
