using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPotion : BasePotion
{
    float SpeedPrecent;
    [SerializeField] GameObject particle;
    void Start()
    {
        SpeedPrecent = 0.1f;
    }
    [ContextMenu("Use Potion")]
    public override void UseItem()
    {
        if (isCool)
        {
        base.UseItem();
            Buff buff = new Buff("스피드 포션", potionSO.duration, StatType.Speed, SpeedPrecent, true);
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
