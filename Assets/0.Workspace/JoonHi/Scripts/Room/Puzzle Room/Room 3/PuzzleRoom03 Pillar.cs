using System.Collections;
using UnityEngine;

namespace JH
{
    public class PuzzleRoom03Pillar : MonoBehaviour
    {
        [Header("GameObjects")]
        [SerializeField] PuzzleRoom03 puzzleRoom3;

        [Header("Animation")]
        [SerializeField] Animator animator;

        [Header("State")]
        public Element element;

        public bool isDeactivate;

        public enum Element { Water = 0, Wind, Fire, Grass };

        private bool ONOFF;

        private void Awake()
        {
            isDeactivate = false;
            ONOFF = false;
            animator = gameObject.GetComponent<Animator>();
        }

        private void Start()
        {
            puzzleRoom3 = gameObject.transform.parent.gameObject.GetComponent<PuzzleRoom03>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (isDeactivate != true)
            {
                ONOFF = !ONOFF;
                SwitchPillar();
                puzzleRoom3.inputIn(element, ONOFF);
            }
        }

        public void StopPillar()
        {
            ONOFF = true;
            ON();
            isDeactivate = true;
        }

        public void Activate()
        {
            animator.SetBool("Activate", true);
        }
        public void Deactivate()
        {
            animator.SetBool("Activate", false);
        }

        public void SwitchPillar()
        {
            animator.SetBool("On", ONOFF);
        }

        public void ON()
        {
            animator.SetBool("On", true);
        }

        public void OFF()
        {
            animator.SetBool("On", false);
        }
    }
}