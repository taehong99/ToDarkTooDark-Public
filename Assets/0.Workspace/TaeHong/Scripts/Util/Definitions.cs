using UnityEngine;
using System;

public enum PlayerJob { Swordsman, Archer, Priest, Wizard }


[Serializable]
public struct PlayerJobData
{
    public PlayerJob job;
    public string _name;
    public Sprite sprite;
}