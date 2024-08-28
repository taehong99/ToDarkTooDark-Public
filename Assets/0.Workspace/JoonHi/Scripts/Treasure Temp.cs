using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JH
{
    public class TreasureTemp : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] List<Sprite> sprites;

        public UnityEvent Opened = new UnityEvent();

        public bool isOpen;

        private void Awake()
        {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            isOpen = false;
            spriteRenderer.sprite = sprites[0];
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                isOpen = true;
                spriteRenderer.sprite = sprites[1];
                Opened.Invoke();
                gameObject.GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}