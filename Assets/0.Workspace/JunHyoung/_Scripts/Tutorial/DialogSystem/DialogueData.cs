using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue")]
public class DialogueData : ScriptableObject
{
    public int ID;
    [Multiline]
    public string script;
    public Teller teller;
    public Sprite sprite; // Emote;
}

[Serializable]
public enum Teller
{
    척준렬 = 0,
    크로커스 = 1,
}