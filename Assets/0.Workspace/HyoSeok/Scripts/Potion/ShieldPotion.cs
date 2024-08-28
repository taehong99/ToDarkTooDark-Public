using System.Collections;
using UnityEngine;

//[CreateAssetMenu(menuName = "Item/ShieldPotion")]
public class ShieldPotion : BasePotion
{
    float shieldPrecent;
    [SerializeField] GameObject particle;
    private void Start()
    {
        shieldPrecent = 0.2f;
    }
    [ContextMenu("Use Potion")]
    public override void UseItem()
    {
        if (isCool)
        {
        base.UseItem();
            Shield shield = new Shield("실드 포션", potionSO.duration, shieldPrecent);
            gameObject.GetComponentInParent<StatusEffectManager>().AddEffect(shield);
            StartCoroutine(ParticleStart());
        }
    }
    IEnumerator ParticleStart()
    {
        particle.SetActive(true);
        yield return new WaitForSeconds(potionSO.duration);
        particle.SetActive(false);

    }
}
