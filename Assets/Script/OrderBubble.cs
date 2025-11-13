using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderBubble : MonoBehaviour
{
    public TextMeshPro orderText;
    public SpriteRenderer bubbleRenderer;
    public SpriteRenderer orderimage;
    public Sprite[] customerOrder;
    public AudioSource musicAudio;


    public void SetOrder(OrderType orderType)
    {
        if (orderText != null)
        {
            musicAudio.Play();
            if (orderType == OrderType.Coffee)
            {
                orderText.text = "C";
                orderText.color = new Color(1f, 1f, 1f);
                orderimage.sprite = customerOrder[0];
            }
            else
            {
                orderText.text = "F";
                orderText.color = new Color(1f, 1f, 1f);
                orderimage.sprite = customerOrder[1];
            }

            orderText.fontSize = 4;
            orderText.alignment = TextAlignmentOptions.Center;
            orderText.fontStyle = FontStyles.Bold;
        }

        if (bubbleRenderer != null)
        {
            bubbleRenderer.color = (orderType == OrderType.Coffee)
                ? new Color(1f, 1f, 1f, 1f)
                : new Color(1f, 1f, 1f, 1f);
        }
    }
}
