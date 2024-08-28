using UnityEngine;
using UnityEngine.InputSystem;

namespace JH
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] float moveSpeed;
        [SerializeField] GameObject lightGameObject;

        private float angle;
        private Vector2 target, mouse;
        private Vector2 moveDir;

        // Start is called before the first frame update
        void Start()
        {
            target = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = (Vector2) gameObject.transform.position + (moveDir * moveSpeed * Time.deltaTime);
        }

        /*
        private void FixedUpdate()
        {
            mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            angle = Mathf.Atan2(mouse.y - target.y, mouse.x - target.x) * Mathf.Rad2Deg;
            lightGameObject.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
        */

        public GameObject PlayerLight()
        {
            return lightGameObject;
        }

        private void OnMove(InputValue value)
        {
            Vector2 inputDir = value.Get<Vector2>();
            moveDir.x = inputDir.x;
            moveDir.y = inputDir.y;
        }
    }
}