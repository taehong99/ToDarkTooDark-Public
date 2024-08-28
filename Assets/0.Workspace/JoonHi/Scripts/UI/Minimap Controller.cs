using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace JH
{
    public class MinimapController : MonoBehaviour
    {
        [Header("Minimap Camera")]
        [SerializeField] Camera minimapCamera;
        [SerializeField] Light2D minimapLight;

        [Header("Buttons")]
        [SerializeField] Button zoomInButton;
        [SerializeField] Button zoomOutButton;

        [Header("State")]
        [SerializeField] int zoomInMagnif;
        [SerializeField] int zoomOutMagnif;

        [Header("UI")]
        [SerializeField] Image MinimapPanel;
        [SerializeField] RawImage Minimap;
        [SerializeField] Slider minimapAlphaSlider;

        public List<Image> images = new List<Image>();

        private void Awake()
        {
            // 이후 Manager에게서 현제 자신의 player의 minimapCamera를 받아올 수 있도록 구현
            // minimapCamera.gameObject.SetActive(true);

            zoomInButton.onClick.AddListener(ZoomIn);
            zoomOutButton.onClick.AddListener(ZoomOut);

            minimapAlphaSlider.onValueChanged.AddListener(ChangeAlpha);
            minimapAlphaSlider.minValue = 0.0f;
            minimapAlphaSlider.maxValue = 1.0f;
        }

        private void Start()
        {
            // 미니맵 설정 초기화
            InitMinimap();
        }

        /// <summary>현제 미니맵 빛 설정(GlobalLight)의 빛 범위를 성정합니다.</summary>
        /// <param name="Radius">intensity는 설정할 미니맵 빛 설정(GlobalLight)의 빛 범위입니다.</param>
        public void SetminimapLightIng(float Radius)
        {
            minimapLight.pointLightOuterRadius = Radius;
        }

        public void InitMinimap()
        {
            ZoomIn();
        }

        private void ChangeAlpha(float miniMapAlpha)
        {
            MinimapPanel.color = new Color(1,1,1,miniMapAlpha/5);
            Minimap.color = new Color(1, 1, 1, miniMapAlpha);
            foreach(Image image in images)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, miniMapAlpha);
            }
        }

        /// <summary>미니맵 배율 확대 함수</summary>
        private void ZoomIn()
        {
            minimapCamera.orthographicSize = zoomInMagnif;
            zoomInButton.interactable = false;
            zoomOutButton.interactable = true;
        }
        /// <summary>미니맵 배율 축소 함수</summary>
        private void ZoomOut()
        {
            minimapCamera.orthographicSize = zoomOutMagnif;
            zoomInButton.interactable = true;
            zoomOutButton.interactable = false;
        }
    }
}
