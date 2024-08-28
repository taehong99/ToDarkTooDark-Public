using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapGenerator;

[RequireComponent(typeof(Collider2D))]
public class PressurePlate : RoomEventTrigger
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!activeState)
            OnTrigger();
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        //OnTrigger();
    }

}
