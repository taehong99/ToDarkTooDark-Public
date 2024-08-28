using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUsableItem
{
    bool IsCool { get; set; }
    void UseItem();
}
