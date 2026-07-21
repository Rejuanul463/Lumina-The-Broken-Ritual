using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action PointerDown;
    public Action PointerUp;

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerUp?.Invoke();
    }
}