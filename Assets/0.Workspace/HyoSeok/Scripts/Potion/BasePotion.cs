using System.Collections;
using UnityEngine;

public class BasePotion : MonoBehaviour, IUsableItem
{
    [SerializeField] protected PotionSO potionSO;
    protected bool isCool = true;
    protected bool isOpen = false;

    public bool IsOpen { get { return isOpen; } set { isOpen = value; } }

    public bool IsCool { get { return isCool; } set { isCool = value; } }

    public virtual void UseItem()
    {
        StartCoroutine(CoolTime());
    }

    protected virtual IEnumerator CoolTime()
    {
        isCool = false;
        yield return new WaitForSeconds(potionSO.coolTime);
        isCool = true;
    }
}