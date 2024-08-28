using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPotion : BasePotion
{
    float PowerPrecent;
    [SerializeField] GameObject particle;
    void Start()
    {
        PowerPrecent = 0.05f;
    }
    [ContextMenu("Use Potion")]
    public override void UseItem()
    {
        if (isCool)
        {
        base.UseItem();
            Buff buff = new Buff("힘 포션", potionSO.duration, StatType.Power, PowerPrecent, true);
            gameObject.GetComponentInParent<StatusEffectManager>().AddEffect(buff);
            particle.SetActive(true);
            StartCoroutine(ParticleStart());
        }
    }
    IEnumerator ParticleStart()
    {
        particle.SetActive(true);
        yield return new WaitForSeconds(1f);
        particle.SetActive(false);
    }

}
