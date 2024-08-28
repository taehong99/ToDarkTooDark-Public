using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JH
{
    public class PlayerCameraView : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] GameObject PlayerCamera;
        [SerializeField] CinemachineVirtualCamera PlayerCinemachineCamera;

        [Header("boundarys")]
        [SerializeField] PolygonCollider2D FullBound;
        [SerializeField] PolygonCollider2D LookAroundBound;

        [Header("State")]
        [SerializeField] float minFOV = 4f;
        [SerializeField] float maxFOV = 8f;
        [SerializeField] float zoomSpeed = 0.5f;
        [Space]
        [SerializeField] float lookAroundSensitivity = 0.5f;

        private float curFOV;
        private IEnumerator CameraMove;

        private void Awake()
        {
            curFOV = minFOV;
            PlayerCinemachineCamera.m_Lens.OrthographicSize = curFOV;
            PlayerCinemachineCamera.gameObject.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = FullBound;
            PlayerCinemachineCamera.gameObject.GetComponent<CinemachineConfiner>().m_BoundingShape2D = LookAroundBound;
        }

        private void OnCameraZoom(InputValue value)
        {
            Vector2 scroll = value.Get<Vector2>();
            if (scroll.y != 0)
                ChangeMainCamFOV(scroll.y);
        }

        private void OnCameraView(InputValue value)
        {
            if (value.isPressed)
            {
                Vector3 mousePivot = Input.mousePosition;
                mousePivot.z = PlayerCinemachineCamera.gameObject.transform.position.z;
                CameraMove = CameraMovement(mousePivot);
                StartCoroutine(CameraMove);
            }
            else
            {
                StopCoroutine(CameraMove);
                PlayerCinemachineCamera.gameObject.transform.localPosition = new Vector3(0, 0, PlayerCinemachineCamera.gameObject.transform.position.z);
            }
        }

        private void ChangeMainCamFOV(float value)
        {
            switch (value < 0)
            {
                case true:
                    curFOV += zoomSpeed;
                    break;
                case false:
                    curFOV -= zoomSpeed;
                    break;
            }
            if (maxFOV < curFOV)
                curFOV = maxFOV;
            else if (minFOV > curFOV)
                curFOV = minFOV;
            PlayerCinemachineCamera.m_Lens.OrthographicSize = curFOV;
        }

        IEnumerator CameraMovement(Vector3 mousePivot)
        {
            Vector3 interval;
            Vector3 CamMoveinterval;
            while (true)
            {
                interval = (mousePivot - Input.mousePosition) * lookAroundSensitivity;
                CamMoveinterval = new Vector3(interval.x, interval.y, PlayerCinemachineCamera.gameObject.transform.position.z);
                PlayerCinemachineCamera.gameObject.transform.localPosition = CamMoveinterval;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}