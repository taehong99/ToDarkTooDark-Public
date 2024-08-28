using UnityEngine;
using UnityEngine.Events;


namespace MapGenerator
{
    // 레버, 발판등 룸에서 특정 이벤트를 발생시키고자 하는 트리거는 다음 스크립트 상속할것
    public class RoomEventTrigger : MonoBehaviour
    {
        [SerializeField] public bool activeState;
        [SerializeField] bool isOnce;
        // 일회성인지 아닌지도 bool 추가

        [SerializeField] SpriteRenderer spriteRenderer;

        [Header("Sprites")]
        public Sprite activeSprite;
        public Sprite inactiveSprite;

        [Space(10)]
        public UnityEvent<bool> roomEvent;

        

        [ContextMenu("Trigger")]
        public virtual void OnTrigger()
        {
            activeState = !activeState;
            ChangeSprite();
            roomEvent?.Invoke(activeState);
            if(isOnce)
            {
                roomEvent.RemoveAllListeners();
            }
        }

        void ChangeSprite()
        {
            if (spriteRenderer == null || activeSprite == null || inactiveSprite == null)
                return;

            spriteRenderer.sprite = activeState ? activeSprite : inactiveSprite;
        }
    }
}