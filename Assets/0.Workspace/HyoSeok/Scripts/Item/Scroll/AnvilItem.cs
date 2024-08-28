using System.Collections;
using System.Collections.Generic;
using Tae.Inventory;
using UnityEngine;
[CreateAssetMenu(fileName = "AnvilItem", menuName = "Item/AnvilItem")]
public class AnvilItem : BaseScrollItem
{
    protected override void UseScroll(GameObject user)
    {
/*        int random = Random.Range(-2, 4);
        InventoryManager.Instance.equipmentInventory[0].enhanceLevel += random;
*/    }
}
