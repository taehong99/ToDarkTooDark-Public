using System.Collections;
using System.Collections.Generic;
using Tae;
using UnityEngine;

public class LavaArea : MonoBehaviour
{
    [SerializeField] GameObject player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.GetComponent<PlayerHealth>().photonView.IsMine)
        {
            if(player == null)
            {
                player = collision.gameObject;
                collision.GetComponent<DebufController>().OnLava();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.GetComponent<PlayerHealth>().photonView.IsMine)
        {
            if (player != null)
            {
                player = null;
                collision.GetComponent<DebufController>().LavaTimerStart();
            }
        }
    }
}
