using System.Collections;
using UnityEngine;

namespace JH
{
    public class PlayerFogChecker : MonoBehaviour
    {
        public FogOfWar fogOfWar;
        public Transform secondaryFogOfWar;
        [Range(0, 10)]
        public float sightDistance;
        public float checkInterval;

        [Header("Debug")]
        // 현제 Gizmo를 보여줄지 안보여줄지 여부
        [SerializeField] bool debug;

        void Start()
        {
            StartCoroutine(CheckFogOfWar(checkInterval));
        }

        // 구멍의 크기를 가시화하는 함수
        private void OnDrawGizmosSelected()
        {
            if (debug != true)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightDistance + 1);
        }

        // SpriteMash에 구멍을 뚫는 함수
        private IEnumerator CheckFogOfWar(float checkInterval)
        {
            while (true)
            {
                fogOfWar.MakeHole(transform.position, sightDistance);
                yield return new WaitForSeconds(checkInterval);
            }
        }
    }
}