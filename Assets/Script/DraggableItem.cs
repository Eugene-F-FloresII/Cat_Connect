using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Item Data")]
    public OrderType orderType;

    [Header("UI References")]
    public TextMeshProUGUI itemText;
    public Image itemImage;
    public Image foodimage;
    public Sprite[] orders;
    public AudioSource musicAudio;

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector3 originalPosition;


    private void Update()
    {
        if (orderType == OrderType.Coffee)
        {
            foodimage.sprite = orders[0];
        }
        else if (orderType == OrderType.Food) {

            foodimage.sprite = orders[1];
        }
    }
    void Awake()
    {
        foodimage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        musicAudio.Play();
    }

    public void SetOrderType(OrderType type)
    {
        orderType = type;

        if (itemText != null)
        {
            itemText.text = (orderType == OrderType.Coffee) ? " " : " ";
            itemText.fontSize = 18;
            itemText.fontStyle = FontStyles.Bold;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;

        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        bool delivered = TryDeliverToCustomer(eventData);

        if (!delivered)
        {
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;
        }
        else
        {
            TrayManager.Instance.RemoveItemFromTray(gameObject, orderType);
            Destroy(gameObject);
        }
    }

    bool TryDeliverToCustomer(PointerEventData eventData)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            CustomerAI customer = hit.collider.GetComponent<CustomerAI>();

            if (customer != null && customer.currentState == CustomerState.Seated)
            {
                customer.DeliverOrder(orderType);
                return true;
            }
        }

        return false;
    }
}
