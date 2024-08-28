using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tae
{
    public class GuideArrow : MonoBehaviour
    {
        private Transform movingTarget;
        private float angle;
        private Vector2 direction;
        private Vector3 rotation;
        private Coroutine navigationRoutine;

        public void StartNavigation(Transform target)
        {
            if (navigationRoutine != null)
                StopCoroutine(navigationRoutine);

            gameObject.SetActive(true);
            movingTarget = target;
            navigationRoutine = StartCoroutine(NavigationRoutine());
        }

        public void ChangeTarget(Transform target)
        {
            movingTarget = target;
        }

        public void StopNavigation()
        {
            gameObject.SetActive(false);
            StopCoroutine(navigationRoutine);
        }

        private IEnumerator NavigationRoutine()
        {
            while (true)
            {
                PointToTarget();
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void PointToTarget()
        {
            if (movingTarget == null)
                return;

            direction = movingTarget.position - transform.position;
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rotation.z = angle - 90;

            transform.rotation = Quaternion.Euler(rotation);
        }
    }
}