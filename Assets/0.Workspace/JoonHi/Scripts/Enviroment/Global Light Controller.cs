using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace JH
{
    public class GlobalLightController : MonoBehaviour
    {
        [Header("Global Light")]
        [SerializeField] Light2D GlobalLight;

        [Header("intensity")]
        [Range(0.0f, 1.0f)]
        // 처음 실행 됬을때 이 변수로 빛의 세기가 정해지며 이후에는 현제 GlobalLight 세기 표시
        // MIN 0.0f MAX 1.0f
        public float globalLightIntensity = 0f;

        private void Awake()
        {
            GlobalLight = gameObject.GetComponent<Light2D>();
            if (GlobalLight == null)
                gameObject.SetActive(false);
            SetGlobalIntensity(globalLightIntensity);
        }

        /// <summary>현제 전역 빛 설정(GlobalLight)의 빛 강도를 성정합니다.</summary>
        /// <param name="intensity">intensity는 설정할 전역 빛 설정(GlobalLight)의 빛 강도입니다.
        /// (MIN 0.0f | MAX 1.0f / 이보다 높거나 낮은 숫자를 넣으면 자동으로 최대 최소값으로 조정됩니다.)</param>
        public void SetGlobalIntensity(float intensity)
        {
            if (intensity > 1.0f)
                intensity = 1.0f;
            else if (intensity < 0.0f)
                intensity = 0.0f;

            globalLightIntensity = intensity;
            GlobalLight.intensity = globalLightIntensity;
        }
    }
}