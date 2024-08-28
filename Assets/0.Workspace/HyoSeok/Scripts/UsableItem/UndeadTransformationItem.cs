using System.Collections;
using System.Collections.Generic;
using Tae;
using UnityEngine;
public class UndeadTransformationItem : BaseUseableItem
{
    GameObject player;
    private void Start()
    {
        player = Manager.Game.MyPlayer;
    }
    // 회복 대신 데미지를 줘야되는데 어케해야될까.... 흡...
    [ContextMenu("Test")]
    public override void UseItem()
    {
        // 테스트용
        player = Manager.Game.MyPlayer;

        if (isCool)
        {
            Debug.Log("발동");
            Buff buff = new Buff("언데드화", usableItemSO.effectDuration, StatType.Power, 0.1f, true);
            player.GetComponent<StatusEffectManager>().AddEffect(buff);
            StartCoroutine(DurationRoutine());
            base.UseItem();
        }
    }

    IEnumerator DurationRoutine()
    {
        player.GetComponent<PlayerHealth>().IsUndead = true;
        particle.SetActive(true);
        yield return new WaitForSeconds(usableItemSO.effectDuration);
        particle.SetActive(false);
        player.GetComponent<PlayerHealth>().IsUndead = false;
    }
}
