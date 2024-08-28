using MapGenerator;
using Tae;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Lever : RoomEventTrigger, IInteractable
{
    private void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = inactiveSprite;
    }

    void IInteractable.HideUI()
    {
        // throw new System.NotImplementedException();
    }

    void IInteractable.OnInteract(Interactor interactor)
    {
        Debug.Log(gameObject.name);
        OnTrigger();
    }

    void IInteractable.ShowUI()
    {
        // throw new System.NotImplementedException();
    }

}
