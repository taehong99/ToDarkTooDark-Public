using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class SilencePotion : BasePotion
{
    [SerializeField] GameObject particle;
    [ContextMenu("Use Potion")]
    public override void UseItem()
    {
        if (isCool)
        {
            base.UseItem();
            SilenceDebuff blackOut = new SilenceDebuff("침묵 포션", potionSO.duration);
            gameObject.GetComponentInParent<StatusEffectManager>().AddEffect(blackOut);
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

