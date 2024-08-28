using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindnessPotion : BasePotion
{
    [SerializeField] GameObject particle;
    public override void UseItem()
    {
        if (isCool)
        {
        base.UseItem();
            BlindnessEffect blindness = new BlindnessEffect("암흑 포션", potionSO.duration);
            gameObject.GetComponentInParent<StatusEffectManager>().AddEffect(blindness);
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
