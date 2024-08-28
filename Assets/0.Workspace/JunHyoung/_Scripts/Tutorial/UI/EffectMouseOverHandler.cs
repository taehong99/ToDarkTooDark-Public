using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EffectMouseOverHandler : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] WaveTextEffect effect;

    public void OnPointerEnter(PointerEventData eventData)
    {
        effect.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        effect.enabled = false;
    }
}
