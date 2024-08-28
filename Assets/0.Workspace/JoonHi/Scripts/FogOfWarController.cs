using UnityEngine;

namespace JH
{
    public class FogOfWarController : MonoBehaviour
    {
        [Header("Fog Of War")]
        [SerializeField] FogOfWar fogOfWar;
        [SerializeField] GameObject fog;

        private void Awake()
        {
            fogOfWar.gameObject.SetActive(true);
            fog.SetActive(true);
        }
    }
}