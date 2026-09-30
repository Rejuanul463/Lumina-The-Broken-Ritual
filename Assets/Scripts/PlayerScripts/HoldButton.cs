using UnityEngine;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// UI helper for on-screen (mobile) buttons that must know when they are pressed AND released,
/// which the normal UI Button (click only) can't do. Used for the Block and Run buttons.
/// Other scripts subscribe to PointerDown / PointerUp (see PlayerInputHandler).
/// </summary>
public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Raised when the finger/mouse goes down on the button
    public Action PointerDown;
    // Raised when the finger/mouse is released
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
