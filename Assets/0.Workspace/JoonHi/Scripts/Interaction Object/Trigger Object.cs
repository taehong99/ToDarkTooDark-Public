using MapGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TriggerObject : RoomEventTrigger
{
    public enum TriggerType { All, One, CertainNum }

    [Header("Option")]
    [SerializeField] TriggerType Type;
    [SerializeField] int CertainNum; // Certain Number일때만 활성화

    public List<RoomEventTrigger> Triggers = new List<RoomEventTrigger>();

    public UnityEvent OnTriggered = new UnityEvent();

    public void UpdateState()
    {
        switch(Type)
        {
            case TriggerType.All:
                bool Alltrigger = true;
                foreach (RoomEventTrigger trigger in Triggers)
                    if (trigger.activeState == false)
                    {
                        Alltrigger = false;
                        break;
                    }
                if(Alltrigger)
                    OnTriggered.Invoke();
                break;
            case TriggerType.One:
                foreach (RoomEventTrigger trigger in Triggers)
                    if (trigger.activeState == true)
                    {
                        OnTriggered.Invoke();
                        break;
                    }
                break;
            case TriggerType.CertainNum:
                int triggernum = 0;
                foreach (RoomEventTrigger trigger in Triggers)
                    if (trigger.activeState == true)
                        triggernum++;
                if(triggernum >= CertainNum)
                    OnTriggered.Invoke();
                break;
        }
    }
}
