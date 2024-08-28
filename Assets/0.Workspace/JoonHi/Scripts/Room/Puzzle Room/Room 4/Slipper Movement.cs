using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JH
{
    public class SlipperMovement : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] GameObject Player;

        [Header("Anker")]
        [SerializeField] Transform PuzzleMidPosition;

        [Header("Components")]
        // [SerializeField] PlayerMover playerMover;
        [SerializeField] PlayerController playerController;
        [SerializeField] Collider2D playerCollider;

        [Header("LayerMask")]
        [SerializeField] LayerMask iceLayer;
        [SerializeField] LayerMask groundLayer;
        [SerializeField] LayerMask wallLayer;

        [Header("Stat")]
        [SerializeField] float slideSpeed;

        private Vector2 slideDir;
        private bool isSliding;
        private bool OnIce;
        private bool isStablising;
        private bool isSafeGround;

        private IEnumerator Slide;

        private void Start()
        {
            OnIce = false;
            isSliding = false;
            isStablising = false;
            isSafeGround = false;
        }

        private void OnDisable()
        {
            isSliding = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (iceLayer.Contain(collision.gameObject.layer))
            {
                // playerMover.enabled = false;
                playerController.enabled = false;
                PlayerPositionStablelize();
                OnIce = true;
            }
            if (groundLayer.Contain(collision.gameObject.layer))
            {
                isSliding = false;
                OnIce = false;
                isSafeGround = true;
                // playerMover.enabled = true;
                playerController.enabled = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (iceLayer.Contain(collision.gameObject.layer))
            {
                isSliding = false;
                OnIce = false;
                //playerMover.enabled = true;
                playerController.enabled = true;
            }
            if (groundLayer.Contain(collision.gameObject.layer))
            {
                // playerMover.enabled = false;
                playerController.enabled = false;
                PlayerPositionStablelize();
                OnIce = true;
                isSafeGround = false;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (OnIce && wallLayer.Contain(collision.gameObject.layer))
            {
                Debug.Log($"collision: {transform.position}");
                isSliding = false;
                if (Slide != null)
                    StopCoroutine(Slide);
                PlayerPositionStablelize();
            }
        }

        private bool CheckNextMove(Vector2 direction)
        {
            if (OnIce)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f, wallLayer);
                if (hit.collider != null)
                    return true;
            }
            return false;
        }

        private void PlayerPositionStablelize()
        {
            Debug.Log("/////////////////");
            Debug.Log($"Player Position: {transform.position}");
            Debug.Log($"Puzzle Position: {transform.position}");
            isStablising = true;
            Vector2 playerPosition = (Vector2) Player.transform.position - (Vector2) PuzzleMidPosition.position + new Vector2(0, playerCollider.offset.y);
            Debug.Log($"Player Position(MID): {playerPosition}");
            switch (playerPosition.x >= 0)
            {
                case true:
                    playerPosition.x = Mathf.Floor(playerPosition.x);
                    // playerPosition.x -= 0.5f;
                    break;
                case false:
                    playerPosition.x = Mathf.Ceil(playerPosition.x);
                    // playerPosition.x += 0.5f;
                    break;
            }

            switch (playerPosition.y >= 0)
            {
                case true:
                    playerPosition.y = Mathf.Floor(playerPosition.y);
                    break;
                case false:
                    playerPosition.y = Mathf.Floor(playerPosition.y);
                    break;
            }
            playerPosition.y += 0.5f;


            Debug.Log($"Stable Position(MID): {playerPosition}");
            Player.transform.position = playerPosition + (Vector2) PuzzleMidPosition.position;
            isStablising = false;
            Debug.Log($"Stable Position: {transform.position}");
            Debug.Log("/////////////////");
        }

        private void OnMove(InputValue value)
        {
            Vector2 inputDir = value.Get<Vector2>();
            slideDir.x = inputDir.x;
            slideDir.y = inputDir.y;
            if (OnIce == true && isSliding == false && isStablising == false && slideDir.magnitude != 0 && slideDir.magnitude <= 1)
            {
                if (!CheckNextMove(slideDir))
                {
                    isSliding = true;
                    Slide = slide(slideDir);
                    StartCoroutine(Slide);
                }
            }
        }

        private IEnumerator slide(Vector2 Direction)
        {
            Vector2 SlideDirection = Direction;
            while (isSliding == true)
            {
                Player.transform.position = (Vector2) Player.transform.position + (SlideDirection * slideSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}