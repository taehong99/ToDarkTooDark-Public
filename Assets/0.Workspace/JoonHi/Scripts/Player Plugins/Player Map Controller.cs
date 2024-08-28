using Photon.Pun;
using UnityEngine;

namespace JH
{
    public class PlayerMapController : MonoBehaviour
    {
        [SerializeField] GameObject FullMap;
        [SerializeField] GameObject PlayerMap;

        [SerializeField] GameObject minimapCamera;
        [SerializeField] GameObject MinimapCanvas;

        private void Start()
        {
            if(!transform.root.GetComponent<PhotonView>().IsMine)
            {
                return;
            }

            // 임시 (나중에 메니저에서 읽음)
            if (FullMap == null || PlayerMap == null)
                this.enabled = false;
            FullMap.SetActive(false);
            PlayerMap.SetActive(true);

            minimapCamera.transform.localPosition = new Vector3(0, 0, -30);
        }

        // MapSwap message가 오면 반응. 전체맵과 플레이어맵 ON/OFF 교체
        private void OnMapSwap()
        {
            FullMap.SetActive(!FullMap.activeSelf);
            PlayerMap.SetActive(!PlayerMap.activeSelf);
        }
    }
}