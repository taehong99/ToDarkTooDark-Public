using UnityEngine;

namespace JH
{
    public class SpriteMaskInteractionChanger : MonoBehaviour
    {
        [Header("Init")]
        [SerializeField] SpriteMaskInteraction initSpriteMaskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        private void Awake()
        {
            // SpriteRenderer가 있는지 확인하고 없으면 경고 Log를 띄우도록 작동
            SpriteRenderer spriteRend = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRend == null)
            {
                Debug.LogWarning($"SpriteMaskInteractionChanger: {gameObject.name} don't have SpriteMaskInteractionChanger. Please Check the GameObject");
                return;
            }
            spriteRend.maskInteraction = initSpriteMaskInteraction;
        }
    }
}
