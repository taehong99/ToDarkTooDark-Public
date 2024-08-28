using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragableWindow : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] RectTransform window; 
    private Vector2 downPosition;

    void Start()
    {
        if(window == null)
        {
            window = GetComponent<RectTransform>();
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        downPosition = data.position;
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 offset = data.position - downPosition;
        downPosition = data.position;

        window.anchoredPosition += offset;
    }
 }