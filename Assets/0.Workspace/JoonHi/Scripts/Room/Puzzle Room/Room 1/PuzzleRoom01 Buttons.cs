using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class PuzzleRoom01Buttons : MonoBehaviour
    {
        [Header("GameObjects")]
        [SerializeField] Puzzle01Platform platform;

        [Header("Components")]
        [SerializeField] BoxCollider2D buttonCollider;
        [SerializeField] SpriteRenderer spriteRenderer;

        [Header("Sprites")]
        [SerializeField] List<Sprite> buttonStateSprite;

        [Header("Stat")]
        [SerializeField] int waitTime;          // 비활성화 대기시간
        [SerializeField] LayerMask layermask;   // 반응할 오브젝트의 layermask
        public ButtonState state { get; private set; }  // 버튼의 상태

        private List<GameObject> PressingObject = new List<GameObject>();   // 버튼을 누르고 있는 물체의 리스트. 비어있어야 대기시간 시작
        private IEnumerator timer;

        public enum ButtonState { NotPressed = 0, Waiting, Pressed };   // 0: 비활성화 , 1: 비활성화 대기, 2: 활성화

        private void Awake()
        {
            buttonCollider = gameObject.GetComponent<BoxCollider2D>();
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            state = ButtonState.NotPressed;
            changeSprite();
        }

        private void Start()
        {
            platform = gameObject.transform.parent.GetComponent<Puzzle01Platform>();
        }

        public void Deactivate()
        {
            if (timer != null)
                StopCoroutine(timer);
            buttonCollider.enabled = false;
            state = ButtonState.Pressed;
            changeSprite();
        }

        public void Activate()
        {
            buttonCollider.enabled = true;
            state = ButtonState.NotPressed;
            changeSprite();
        }

        // 버튼에서 타겟 오브젝트가 들어왔을때
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (layermask.Contain(collision.gameObject.layer))
            {
                if (PressingObject.Contains(collision.gameObject) == false)
                    PressingObject.Add(collision.gameObject);
                if (state != ButtonState.Pressed)
                {
                    if (timer != null)
                    {
                        StopCoroutine(timer);
                        timer = null;
                    }
                    state = ButtonState.Pressed;
                    changeSprite();
                    platform.PlateActivate();
                }
            }
        }

        // 버튼에서 타겟 오브젝트가 나갔을때
        private void OnTriggerExit2D(Collider2D collision)
        {
            // 만약 반응할 layermask를 가진 물체라면 누르고있는 물체 리스트에서 제거
            if (layermask.Contain(collision.gameObject.layer))
                PressingObject.Remove(collision.gameObject);

            if (buttonCollider.enabled == false)
                return;

            // 만약 눌려저있는 상태에서 아무 물체도 올려져 있지 않은경우
            if (state == ButtonState.Pressed && PressingObject.Count == 0)
            {
                timer = Timer(waitTime);
                StartCoroutine(timer);
                state = ButtonState.Waiting;
                changeSprite();
            }
        }

        // 버튼의 스프라이트 변환
        private void changeSprite()
        {
            spriteRenderer.sprite = buttonStateSprite[(int) state];
        }

        // 버튼 비활성화 시간계산
        private IEnumerator Timer(int time)
        {
            int timeleft = 0;
            while (timeleft < waitTime)
            {
                timeleft += 1;
                yield return new WaitForSeconds(1);
            }
            state = ButtonState.NotPressed;
            changeSprite();
        }
    }
}