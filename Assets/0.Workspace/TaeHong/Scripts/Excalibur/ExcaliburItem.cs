using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Tae.Inventory
{
    public class ExcaliburItem : ItemDrop
    {
        [SerializeField] LayerMask obstacleMask;
        [SerializeField] Transform spriteTransform;
        [SerializeField] Transform dropBezierPointsParent;
        [SerializeField] Transform slipBezierPointsParent;
        [SerializeField] float rotateSpeed;
        private Vector2[] dropBezierPoints = new Vector2[4];
        private Vector2[] slipBezierPoints = new Vector2[3];
        
        private bool hitObstacle;
        private PhotonView photonView;
        
        private void Awake()
        {
            photonView = GetComponent<PhotonView>();

            // Get Drop Bezier Points
            for(int i = 0; i < dropBezierPoints.Length; i++)
            {
                dropBezierPoints[i] = dropBezierPointsParent.GetChild(i).localPosition;
            }
            // Get Slip Bezier Points
            for (int i = 0; i < slipBezierPoints.Length; i++)
            {
                slipBezierPoints[i] = slipBezierPointsParent.GetChild(i).localPosition;
            }
        }

        [PunRPC]
        public void DestroyExcalibur(int viewID)
        {
            PhotonView targetView = PhotonView.Find(viewID);
            if(targetView != null && targetView.IsMine)
            {
                PhotonNetwork.Destroy(targetView);
            }
        }

        public override void OnCollect(Collector collector)
        {
            collector.CollectExcalibur();
            if(PhotonNetwork.IsConnected)
                photonView.RPC("DestroyExcalibur", RpcTarget.MasterClient, photonView.ViewID);
            
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!obstacleMask.Contain(collision.gameObject.layer))
                return;

            hitObstacle = true;
        }

        #region Drop/Slip
        public void Drop(bool isLeft)
        {
            Manager.Event.RevealExcaliburLocation(transform);
            photonView.RPC("DropRPC", RpcTarget.AllViaServer, isLeft);
        }

        public void Slip(bool isLeft)
        {
            Manager.Event.RevealExcaliburLocation(transform);
            photonView.RPC("SlipRPC", RpcTarget.AllViaServer, isLeft);
        }

        [PunRPC]
        public void DropRPC(bool dropLeft)
        {
            StartCoroutine(DropExcaliburRoutine(dropLeft));
        }

        [PunRPC]
        public void SlipRPC(bool dropLeft)
        {
            StartCoroutine(SlipExcaliburRoutine(dropLeft));
        }

        private IEnumerator DropExcaliburRoutine(bool isLeft)
        {
            canInteract = false;

            float t = 0;
            Vector3 startPos = transform.position;
            Vector2 p0 = dropBezierPoints[0];
            Vector2 p1 = dropBezierPoints[1];
            Vector2 p2 = dropBezierPoints[2];
            Vector2 p3 = dropBezierPoints[3];
            Vector3 degreesToRotate = -Vector3.forward * rotateSpeed;

            if (isLeft)
            {
                p1.x *= -1;
                p2.x *= -1;
                p3.x *= -1;
                degreesToRotate *= -1;
            }

            while (t < 1 && !hitObstacle)
            {
                spriteTransform.Rotate(degreesToRotate);
                transform.position = startPos + (Vector3) MathUtilities.CubicBezierCurve(p0, p1, p2, p3, t);
                t += Time.deltaTime;
                yield return null;
            }

            canInteract = true;
        }

        private IEnumerator SlipExcaliburRoutine(bool isLeft)
        {
            canInteract = false;

            float t = 0;
            Vector3 startPos = transform.position;
            Vector2 p0 = slipBezierPoints[0];
            Vector2 p1 = slipBezierPoints[1];
            Vector2 p2 = slipBezierPoints[2];
            spriteTransform.Rotate(transform.forward * -90); // sprite | -> ㅡ
            if (isLeft)
            {
                spriteTransform.Rotate(transform.forward * 180);
                p1.x *= -1;
                p2.x *= -1;
            }

            //Drop
            while (t < 1f)
            {
                if (hitObstacle)
                {
                    canInteract = true;
                    yield break;
                }

                transform.position = startPos + (Vector3) MathUtilities.QuadraticBezierCurve(p0, p1, p2, MathUtilities.EaseInQuad(t));
                t += Time.deltaTime * 3;
                yield return null;
            }

            //Slide
            startPos = transform.position;
            Vector3 endPos = startPos + Vector3.right;
            if (isLeft)
            {
                endPos = startPos + Vector3.left;
            }
            t = 0;
            while (t < 1)
            {
                if (hitObstacle)
                {
                    canInteract = true;
                    yield break;
                }

                transform.position = Vector3.Lerp(startPos, endPos, MathUtilities.EaseOutQuad(t));
                t += Time.deltaTime * 2;
                yield return null;
            }

            canInteract = true;
        }
        #endregion

        #region Excalbur Seal
        public void DisableInteraction()
        {
            gameObject.layer = 0;
        }

        public void EnableInteraction()
        {
            gameObject.layer = LayerMask.NameToLayer("Item");
        }
        #endregion
    }
}