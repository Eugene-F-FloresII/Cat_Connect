using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverLeft : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Camera mainCamera;
    public float moveSpeed = 5f;
    public float targetX = 4f;

    private bool isHovering = false;

    void Update()
    {
        if (isHovering && mainCamera != null)
        {
            Vector3 camPos = mainCamera.transform.position;
            if (camPos.x > targetX)
            {
                camPos.x -= moveSpeed * Time.deltaTime;
                camPos.x = Mathf.Max(camPos.x, targetX); // clamp
                mainCamera.transform.position = camPos;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}
