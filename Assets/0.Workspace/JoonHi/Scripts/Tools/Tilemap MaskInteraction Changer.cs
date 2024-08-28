using UnityEngine;
using UnityEngine.Tilemaps;

namespace JH
{
    public class TilemapMaskInteractionChanger : MonoBehaviour
    {
        [Header("Init")]
        [SerializeField] SpriteMaskInteraction initSpriteMaskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        private void Awake()
        {
            // TilemapRenderer가 있는지 확인하고 없으면 경고 Log를 띄우도록 작동
            TilemapRenderer tilemapRend = gameObject.GetComponent<TilemapRenderer>();
            if (tilemapRend == null)
            {
                Debug.LogWarning($"TilemapMaskInteractionChanger: {gameObject.name} don't have TilemapRenderer. Please Check the GameObject");
                return;
            }
            tilemapRend.maskInteraction = initSpriteMaskInteraction;
        }
    }
}