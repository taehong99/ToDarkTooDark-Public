using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DolDoriDebugger : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] DolDori dolDori;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            dolDori.ConsumItem();
        }
        else if(eventData.button == PointerEventData.InputButton.Middle)
        {
            dolDori.OnInteract(null);
        }
    }
}
