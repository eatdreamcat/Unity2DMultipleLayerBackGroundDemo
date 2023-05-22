using System;

using UnityEngine;
using UnityEngine.EventSystems;

public class Mouse : MonoBehaviour, IDragHandler, IPointerDownHandler, IScrollHandler 
{

    public static Action<float, float> Move;
    public static Action<float, float, Vector2> Scroll;

    public void OnDrag(PointerEventData eventData)
    {
        if (Move != null)
        {
            Move(eventData.delta.x, eventData.delta.y);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (Scroll != null)
        {
            Scroll(eventData.scrollDelta.x, eventData.scrollDelta.y, eventData.position - new Vector2(Screen.width / 2, Screen.height / 2));
        }
    }
}
