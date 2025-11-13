using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Settings")]
    [SerializeField] private float feedbackDuration = 2f;

    private float feedbackTimer = 0f;

    void Start()
    {
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(OnSaveButtonClicked);
        }

        if (loadButton != null)
        {
            loadButton.onClick.AddListener(OnLoadButtonClicked);
        }

        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (feedbackTimer > 0)
        {
            feedbackTimer -= Time.deltaTime;
            if (feedbackTimer <= 0 && feedbackText != null)
            {
                feedbackText.gameObject.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            OnSaveButtonClicked();
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            OnLoadButtonClicked();
        }
    }

    void OnSaveButtonClicked()
    {
        SaveSystem.Instance.SaveGame();
        ShowFeedback("Game Saved!", Color.green);
    }

    void OnLoadButtonClicked()
    {
        if (SaveSystem.Instance.HasSaveFile())
        {
            SaveSystem.Instance.LoadGame();
            ShowFeedback("Game Loaded!", Color.cyan);
        }
        else
        {
            ShowFeedback("No save file found!", Color.red);
        }
    }

    void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
            feedbackText.gameObject.SetActive(true);
            feedbackTimer = feedbackDuration;
        }

        Debug.Log(message);
    }
}
