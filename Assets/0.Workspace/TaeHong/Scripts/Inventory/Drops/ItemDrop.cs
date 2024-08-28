using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Tae.Inventory
{
    [RequireComponent(typeof(Collider2D))]
    public class ItemDrop : MonoBehaviour, IInteractable, ICollectable
    {
        [SerializeField] GameObject interactionUI;
        [SerializeField] float floatLength;
        [SerializeField] Transform floatingSprite;
        private Sequence bounceSequence;
        protected bool canInteract = true;

        protected virtual void Start()
        {
            Bounce();
        }

        private void OnDestroy()
        {
            bounceSequence.Kill();
        }

        private void Bounce()
        {
            Vector3 minYPosition = floatingSprite.position;
            Vector3 maxYPosition = floatingSprite.position + Vector3.up * floatLength;

            bounceSequence = DOTween.Sequence()
                .Append(floatingSprite.DOMove(maxYPosition, 0.5f).SetEase(Ease.InOutSine)) // Bounce up with ease out
                .Append(floatingSprite.DOMove(minYPosition, 0.5f).SetEase(Ease.InOutSine)) // Fall back to ground
                .SetLoops(-1); // Loop infinitely

            bounceSequence.Play();
        }
         
        public void OnInteract(Interactor interactor)
        {
            OnCollect(interactor.collector);
        }

        public virtual void OnCollect(Collector collector) { }

        public void ShowUI()
        {
            if (!canInteract)
                return;
            interactionUI.SetActive(true);
        }

        public void HideUI()
        {
            interactionUI.SetActive(false);
        }
    }
}