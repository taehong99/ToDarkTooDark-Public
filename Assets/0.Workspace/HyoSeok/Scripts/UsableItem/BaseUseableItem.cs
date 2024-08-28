using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 사용형 아이템이 작동하는 클래스
public class BaseUseableItem : MonoBehaviour,IUsableItem
{
    [SerializeField] protected UsableItemSO usableItemSO;
    [SerializeField] protected GameObject particle;
    protected bool isCool = true;

    public bool IsCool { get { return isCool; } set { isCool = value; } }

    public virtual void UseItem() { StartCoroutine(CoolTiem()); }
    IEnumerator CoolTiem()
    {
        isCool = false;
        yield return new WaitForSeconds(usableItemSO.cooldown);
        isCool = true;
    }
}
