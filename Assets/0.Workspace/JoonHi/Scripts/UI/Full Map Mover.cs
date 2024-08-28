using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

namespace JH
{
    public class FullMapMover : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Fullmap Camera")]
        [SerializeField] Camera fullMapCamera;

        [Header("State")]
        [SerializeField] float minZoom = 4f;
        [SerializeField] float maxZoom = 50f;
        [SerializeField] float zoomSpeed = 1f;
        [Space]
        [SerializeField] float moveAroundSensitivity = 0.5f;

        [Header("UI")]
        [SerializeField] Slider fullmapZoomSlider;

        private Vector3 curLocation;        // 마우스 조작으로 변한 전채맵 카메라 위치 저장
        private Vector3 bufLocation;        // 조작중에 전채맵 카메라 위치
        private Vector3 mousePivot;         // 현제 마우스 조작 시작점
        private Vector3 interval;           // 지정된 마우스 조작 시작점과 현제 마우스 위치의 차이
        private Vector3 CamMoveinterval;    // 현제 전체맵 카메라 위치

        private float initZ;                // 카메라의 초기 z좌표
        private bool isMoveing;             // 현제 마우스로 지도를 조작중인지
        private bool isEnabled;             // 현제 이 코드가 실행되고 있는지
        // private IEnumerator CameraMove;

        private void Awake()
        {
            fullmapZoomSlider.onValueChanged.AddListener(ChangeZoom);
            fullmapZoomSlider.minValue = minZoom;
            fullmapZoomSlider.maxValue = maxZoom;
        }

        private void Start()
        {
            initZ = fullMapCamera.gameObject.transform.position.z;
            // curLocation = new Vector3(0, 0, initZ);
            isMoveing = false;
            isEnabled = true;

            fullmapZoomSlider.value = maxZoom;
        }

        private void Update()
        {
            if (isEnabled && isMoveing)
            {
                interval = (mousePivot - Input.mousePosition) * moveAroundSensitivity;
                CamMoveinterval = curLocation + new Vector3(interval.x, interval.y, initZ);
                fullMapCamera.gameObject.transform.localPosition = CamMoveinterval;
                bufLocation = CamMoveinterval;
            }
        }

        private void OnEnable()
        {
            isEnabled = true;
        }

        private void OnDisable()
        {
            fullMapCamera.gameObject.transform.localPosition = new Vector3(0, 0, initZ);
            curLocation = new Vector3(0, 0, initZ);
            isMoveing = false;
            isEnabled = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == InputButton.Left)
            {
                isMoveing = true;
                mousePivot = Input.mousePosition;
                mousePivot.z = initZ;
                bufLocation = curLocation;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == InputButton.Left)
            {
                isMoveing = false;
                curLocation = bufLocation;
            }
        }

        private void ChangeZoom(float fullMapZoom)
        {
            fullMapCamera.orthographicSize = fullMapZoom;
        }
    }
}