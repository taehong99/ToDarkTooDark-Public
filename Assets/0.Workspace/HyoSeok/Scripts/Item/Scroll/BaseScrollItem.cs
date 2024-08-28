using UnityEngine;

public class BaseScrollItem : Tae.Inventory.ConsumableItem
{
    public int ID;
    protected override void UseItem(GameObject user)
    {
        UseScroll(user);
    }
    protected virtual void UseScroll(GameObject user) { }

}
