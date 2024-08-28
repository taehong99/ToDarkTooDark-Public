using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPotion : BasePotion
{
    float ArmorPrecent;
    [SerializeField] GameObject particle;
    void Start()
    {
        ArmorPrecent = 0.06f;
    }
    [ContextMenu("Use Potion")]
    public override void UseItem()
    {
        if (isCool)
        {
            base.UseItem();
            Buff buff = new Buff("방어력 포션", potionSO.duration, StatType.Armor, ArmorPrecent, true);
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
