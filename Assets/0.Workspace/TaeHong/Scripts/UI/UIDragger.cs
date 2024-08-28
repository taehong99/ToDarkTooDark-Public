using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach this to a UI Element with raycast enabled to enable UI dragging.
/// Requires a reference to the desired RectTransform to drag.
/// </summary>
public class UIDragger : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [SerializeField] RectTransform rectTransform;
    Vector2 offset;
    Vector2 lastPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        offset = eventData.position - lastPosition;
        rectTransform.position = rectTransform.position + (Vector3)offset;
        lastPosition = eventData.position;
    }
}
