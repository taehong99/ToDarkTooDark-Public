using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player/StatIncrease", fileName = "StatIncrease")]
public class StatIncreaseSO : ScriptableObject
{
    public float powerUp;
    public float armorUp;
    public float hpUp;
    public float hpRegenUp;
    public float speedUp;
    public float criticalUp;
    public float critMultiplierUp;
}
