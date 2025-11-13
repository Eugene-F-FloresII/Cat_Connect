using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CatHiringUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject catHiringPanel;
    public GameObject catButtonPrefab;
    public Transform catButtonsContainer;
    public TextMeshProUGUI catCountText;

    void Start()
    {
        PopulateCatHiringUI();
        catHiringPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleCatHiringPanel();
        }

        UpdateCatCountText();
    }

    public void ToggleCatHiringPanel()
    {
        catHiringPanel.SetActive(!catHiringPanel.activeSelf);

        if (catHiringPanel.activeSelf)
        {
            RefreshCatHiringUI();
        }
    }

    void PopulateCatHiringUI()
    {
        foreach (Transform child in catButtonsContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < CatHiringManager.Instance.availableCats.Length; i++)
        {
            CatData catData = CatHiringManager.Instance.availableCats[i];
            GameObject buttonObj = Instantiate(catButtonPrefab, catButtonsContainer);
            CatHiringButton catButton = buttonObj.GetComponent<CatHiringButton>();

            if (catButton != null)
            {
                catButton.SetupButton(catData);
            }
        }
    }

    public void RefreshCatHiringUI()
    {
        CatHiringButton[] buttons = catButtonsContainer.GetComponentsInChildren<CatHiringButton>();
        foreach (CatHiringButton button in buttons)
        {
            button.UpdateButtonState();
        }
    }

    void UpdateCatCountText()
    {
        if (catCountText != null)
        {
            int currentCats = GameManager.Instance.GetHiredCatsCount();
            int maxCats = GameManager.Instance.maxCats;
            catCountText.text = $"Cats: {currentCats}/{maxCats}";
        }
    }
}
