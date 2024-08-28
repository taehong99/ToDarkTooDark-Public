using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    // Events
    public event Action PlayerSpawnedEvent;
    public event Action<int> ExcaliburPickUpEvent;
    public event Action<int> DoorInteractionEvent;
    public event Action<Vector3> ObjectBreakEvent;
    public event Action SpawnExcaliburEvent;
    public event Action<Transform> ExcaliburLocationRevealEvent;

    // States
    public bool excaliburPickedUp; 

    // Photon RaiseEvent
    public const byte ExcaliburPickedUpEventCode = 1;
    public const byte GameOverEventCode = 2;

    public void PlayerSpawned()
    {
        Debug.Log("Player Spawned Event Called");
        PlayerSpawnedEvent?.Invoke();
    }

    public void ExcaliburPickedUp(int viewID)
    {
        Debug.Log("Excalibur Picked Up Event Called");
        ExcaliburPickUpEvent?.Invoke(viewID);

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(ExcaliburPickedUpEventCode, viewID, raiseEventOptions, SendOptions.SendReliable);
    }

    public void DoorInteraction(int doorID)
    {
        DoorInteractionEvent?.Invoke(doorID);
    }

    public void GameOver(int actorNum, Team team)
    {
        Debug.Log("Game Over Event Called");
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        object[] content = new object[] { actorNum, team };
        PhotonNetwork.RaiseEvent(GameOverEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void ObjectBreak(Vector3 pos)
    {
        ObjectBreakEvent?.Invoke(pos);
    }

    public void SpawnExcalibur()
    {
        Debug.Log("Spawn Excalibur Event Called");
        SpawnExcaliburEvent?.Invoke();
    }

    public void RevealExcaliburLocation(Transform target)
    {
        Debug.Log("Excalibur Location Reveal Event Called");
        ExcaliburLocationRevealEvent?.Invoke(target);
    }
}