using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EffectTimer : MonoBehaviour
{
    public event Action effectDamageEvent;


    public void DamageFrame()
    {
        effectDamageEvent?.Invoke();
    }

    public void ClearEvent()
    {
        effectDamageEvent = null;
    }
}
