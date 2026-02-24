using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class EventTriggerListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action<GameObject> onPointerDown;
    public Action<GameObject> onPointerUp;

    public static EventTriggerListener Get(GameObject obj)
    {
        EventTriggerListener listener = obj.GetComponent<EventTriggerListener>();
        if (listener == null) listener = obj.AddComponent<EventTriggerListener>();
        return listener;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke(gameObject);
    }
}
