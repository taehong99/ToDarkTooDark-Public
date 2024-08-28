using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace AudioResponseUI
{
    [RequireComponent(typeof(AudioResponse))]
    public class EnhanceResponsePower : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] AudioResponse responseUI;
        [Range(1,10)]
        [SerializeField] float enhancePower = 2f;
 
        float defaultPower;

        private void Awake()
        {
            if (responseUI == null)
                responseUI = GetComponent<AudioResponse>();

            if (responseUI != null)
                defaultPower = responseUI.SizePower;
        }
        

        public void OnPointerExit(PointerEventData eventData)
        {
            if(responseUI != null)  
                responseUI.SizePower = defaultPower;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (responseUI != null)
                responseUI.SizePower = enhancePower * defaultPower;
        }
    }
}