using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawController : MonoBehaviour
{
    public List<SawTrap> saws = new List<SawTrap>();

    private List<GameObject> PlayerList = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (PlayerList.Contains(collision.gameObject) == false)
                PlayerList.Add(collision.gameObject);
            foreach (SawTrap saw in saws)
            {
                saw.SawStart();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerList.Remove(collision.gameObject);
            if (PlayerList.Count == 0)
            {
                foreach (SawTrap saw in saws)
                {
                    saw.SawStop();
                }
            }
        }
    }
}
