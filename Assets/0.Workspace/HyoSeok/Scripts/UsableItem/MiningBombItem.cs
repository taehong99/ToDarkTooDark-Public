using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningBombItem : BaseUseableItem
{
    [SerializeField] GameObject bombPrefab;
    Vector3 mousePosition;
    [ContextMenu("TestMod")]
    public override void UseItem()
    {
        mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Instantiate(bombPrefab, mousePosition, Quaternion.identity);
    }
}
