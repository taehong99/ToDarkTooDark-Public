using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Tae
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;

        [SerializeField] float beginMultiple = 2.0f;
        [SerializeField] float critMultiple = 4.0f;

        private void Awake()
        {
            transform.position = Vector3.zero;
        }

        public void Spawn(Color color, int value, Transform parent)
        {
            text.color = color;
            text.text = value.ToString();
            transform.SetParent(parent, false);
            StartCoroutine(FloatRoutine());
        }

        public void SpawnMultiple(Color color, int value, Transform parent, bool isCrit = false)
        {
            text.color = color;
            text.text = value.ToString();
            transform.SetParent(parent, false);

            if (isCrit)
                transform.localScale = Vector3.one * critMultiple;
            else
                transform.localScale = Vector3.one * beginMultiple;

            transform.DOScale(1f, 1f).SetEase(Ease.OutBounce);
            StartCoroutine(FloatRoutine());
        }

        private IEnumerator FloatRoutine()
        {
            transform.DOMoveY(transform.position.y + 1, 1f);

            // Fade out the text
            text.DOFade(0, 1f);
            
            // Wait for the duration of the animation
            yield return new WaitForSeconds(1f);


            Destroy(gameObject);
        }
    }
}