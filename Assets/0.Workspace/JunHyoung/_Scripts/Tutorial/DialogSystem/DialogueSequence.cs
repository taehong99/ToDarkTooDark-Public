using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Sequence")]
public class DialogueSequence : ScriptableObject
{
    [SerializeField] List<DialogueData> dialogues = new List<DialogueData>();

    int curIndex = -1;

    [ContextMenu("Init Index")] // Instantiate 안하고 썼다가 index 변경되면 이거 함 호출해주셈
    public void InitIndex()
    {
        curIndex = -1;
    }

    public DialogueData NextDialogue()
    {
        curIndex++;

        if (curIndex >= dialogues.Count)
            return null; //  == 시퀀스 종료

        return dialogues[curIndex];
    }
}
