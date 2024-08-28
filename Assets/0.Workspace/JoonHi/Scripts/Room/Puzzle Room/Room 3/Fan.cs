using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class Fan : MonoBehaviour
    {
        [SerializeField] BoxCollider2D collid;
        [SerializeField] LayerMask layerMask;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (layerMask.Contain(collision.gameObject.layer))
            {
                // collision.gameObject.GetComponent<PlayerController>().enabled = false;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (layerMask.Contain(collision.gameObject.layer))
            {
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                // collision.gameObject.GetComponent<PlayerController>().enabled = true;
            }
        }
    }
}