using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Tae;
using UnityEngine;
using UnityEngine.Rendering.Universal;


namespace MapGenerator
{
    public class Door : MonoBehaviour , IInteractable// 얘는 PhotonView 사용 X , 어짜피 개폐 상태만 확인하면 되기 때문에 , 나중에 Manager든 뭐든 하나로 관리
    {
        [SerializeField] bool isVertical;
        [SerializeField] bool isOpend;

        [Space(10),Header("Componets")]
        [SerializeField] BoxCollider2D doorCollider;
        [SerializeField] ShadowCaster2D shadowCaster;

        [Space(10), Header("Sprites")]
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Sprite spriteOpen;
        [SerializeField] Sprite spriteClose;

        [Space(10), Header("SFX")]
        [SerializeField] AudioClip openSFX;
        [SerializeField] AudioClip closeSFX;
 
        //Offset Value Field

        Vector3 yPosOffset = new Vector3(0, 0.9f);
        Vector3 xPosOffset = new Vector3(0.5f,0);

        Vector3 colliderVerticalOffset = new Vector3(0.02f, 0);
        Vector3 colliderVerticalSize = new Vector3(0.5f, 1.5f);

        Vector3[] verticalShadowShape = new Vector3[4]
        {
            new Vector3(-0.15f,-0.5f),
            new Vector3(-0.15f,0.5f),
            new Vector3(0.20f , 0.5f),
            new Vector3(0.20f, -0.5f)
        };

        public int id;

        //Legacy
        public void SetVertical()
        {
            isVertical = true;
            //spriteRenderer.sprite = spriteOpen;
            /*
            for(int i = 0; i<verticalShadowShape.Length; i++)
            {
                shadowCaster.shapePath.SetValue(verticalShadowShape[i], i);
            }

            doorCollider.offset = colliderVerticalOffset;
            doorCollider.size = colliderVerticalSize;*/
        }

        void IInteractable.HideUI()
        {
            //throw new System.NotImplementedException();
        }

        void IInteractable.OnInteract(Interactor interactor)
        {
            if (!PhotonNetwork.IsConnected)
                InteractDoor();
            
            //Manager를 통한 동기화 작업
            // Event 발생  or Property 변경
            // Event 발생시 해당 포지션을 인수로? <- MapGenerator에서 Vector3 를 Key로 하는 Door Dictionary 가지고 있음
            Manager.Event.DoorInteraction(id);
        }

        void IInteractable.ShowUI()
        {
            //throw new System.NotImplementedException();
        }

        [ContextMenu("InteractDoor")]
        public void InteractDoor()
        {
            isOpend = !isOpend;
            if(openSFX !=null && closeSFX != null)
            {
                Manager.Sound.PlaySFX(isOpend? openSFX:closeSFX);
            }
            doorCollider.enabled = !isOpend;
            shadowCaster.castsShadows = !isOpend;

            if (isVertical)
            {
                if (isOpend)
                {
                    spriteRenderer.sprite = spriteOpen;
                    transform.position += yPosOffset + xPosOffset;
                }
                else
                {
                    spriteRenderer.sprite = spriteClose;
                    transform.position -= yPosOffset + xPosOffset;
                }
            }
            else
            {
                if (isOpend)
                {
                    spriteRenderer.sprite = spriteOpen;
                    transform.position -= xPosOffset;
                }
                else
                {
                    spriteRenderer.sprite = spriteClose;
                    transform.position += xPosOffset;
                }
            }
        }

        public void DestroyDoor()
        {
            Destroy(gameObject);
        }


        [ContextMenu("DebuggingShadowShape")]
        public void DebuggingShadowShape()
        {
            foreach(var item in shadowCaster.shapePath)
            {
                Debug.Log(item);
            }
        }
    }
}