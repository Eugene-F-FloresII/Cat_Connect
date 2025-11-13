using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CafeExpansion : MonoBehaviour
{
    public static CafeExpansion Instance { get; private set; }

    [Header("Expansion Settings")]
    [SerializeField] private int expansionCost = 1000;
    [SerializeField] private Vector2Int newGridSize = new Vector2Int(17, 15);

    [Header("References")]
    [SerializeField] private Button expandButton;
    [SerializeField] private GameObject expansionArea;
    [SerializeField] private TextMeshProUGUI buttonText;

    private bool hasExpanded = false;

    public bool HasExpanded => hasExpanded;

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
        if (expandButton != null)
        {
            expandButton.onClick.AddListener(OnExpandButtonClicked);
        }

        if (expansionArea != null && !hasExpanded)
        {
            expansionArea.SetActive(false);
        }

        UpdateButtonText();
    }

    void Update()
    {
        if (expandButton != null && !hasExpanded)
        {
            bool canAfford = GameManager.Instance.CanAfford(expansionCost);
            expandButton.interactable = canAfford;

            UpdateButtonText();
        }
    }

    void UpdateButtonText()
    {
        if (buttonText != null)
        {
            if (hasExpanded)
            {
                buttonText.text = "Expanded!";
            }
            else
            {
                bool canAfford = GameManager.Instance.CanAfford(expansionCost);
                buttonText.text = canAfford
                    ? $"Expand Cafe (${expansionCost})"
                    : $"Need ${expansionCost - GameManager.Instance.money} more";
            }
        }
    }

    void OnExpandButtonClicked()
    {
        if (hasExpanded)
        {
            Debug.Log("Cafe already expanded!");
            return;
        }

        if (!GameManager.Instance.CanAfford(expansionCost))
        {
            Debug.Log($"Not enough money! Need ${expansionCost}");
            return;
        }

        ExpandCafe();
    }

    void ExpandCafe()
    {
        GameManager.Instance.SpendMoney(expansionCost);

        GridManager.Instance.ExpandGrid(newGridSize);

        if (expansionArea != null)
        {
            expansionArea.SetActive(true);
        }

        hasExpanded = true;

        if (expandButton != null)
        {
            expandButton.interactable = false;
        }

        UpdateButtonText();

        Debug.Log($"Cafe expanded! New grid size: {GridManager.Instance.gridSize}");
    }

    public void LoadExpansionState(Vector2Int savedGridSize)
    {
        GridManager.Instance.ExpandGrid(savedGridSize);

        if (savedGridSize.x > 10 || savedGridSize.y > 15)
        {
            hasExpanded = true;

            if (expansionArea != null)
            {
                expansionArea.SetActive(true);
            }

            if (expandButton != null)
            {
                expandButton.interactable = false;
            }

            UpdateButtonText();

            Debug.Log("Expansion state loaded from save.");
        }
    }
}
