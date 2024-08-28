using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player/Passive", fileName = "Passive")]
public class Passive : ScriptableObject
{
    public string passiveName;
    public int passiveLevel;
    public float[] passiveMultiplier;
}
