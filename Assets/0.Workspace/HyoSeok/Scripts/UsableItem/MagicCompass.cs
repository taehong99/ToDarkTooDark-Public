using System.Collections;
using System.Collections.Generic;
using Tae.Inventory;
using Tae;
using UnityEngine;

public class MagicCompass : BaseUseableItem
{
    [SerializeField] PlayerNavigation navigation;
    int usesLeft = 3;
    private void Start()
    {
        navigation = GetComponentInParent<PlayerNavigation>();
    }
    [ContextMenu("UseItem")]
    public override void UseItem()
    {
        if (isCool && usesLeft >= 1)
        {
            base.UseItem();
            navigation.MagicCompassScroll();

        }
    }
    
}
