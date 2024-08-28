using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace JH
{
    public class PuzzleRoom02 : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] GameObject treasurePrefab;

        [Header("Fans")]
        [SerializeField] List<AreaEffector2D> RowFan = new List<AreaEffector2D>();
        [SerializeField] List<AreaEffector2D> ColumFan = new List<AreaEffector2D>();

        [Header("Fan Number")]
        [SerializeField] int Row;
        [SerializeField] int Colum = 2;

        [Header("Spec")]
        [SerializeField] int fanRoutineTime;
        [SerializeField] int magnitude;

        private IEnumerator fanRouter;
        private TreasureTemp treasureInst;

        private void Start()
        {
            if (gameObject.transform.childCount != Row + Colum)
            {
                Debug.LogWarning($"{gameObject.name} Fan number MISMATCH");
                gameObject.SetActive(false);
                return;
            }
            for (int i = 0; i < Row; i++)
                RowFan.Add(gameObject.transform.GetChild(i).GetComponent<AreaEffector2D>());
            for (int i = Row; i < Colum + Row; i++)
                ColumFan.Add(gameObject.transform.GetChild(i).GetComponent<AreaEffector2D>());

            treasureInst = Instantiate(treasurePrefab, gameObject.transform).GetComponent<TreasureTemp>();
            treasureInst.Opened.AddListener(Deactivate);

            fanRouter = FanRouter();
            StartCoroutine(fanRouter);
        }

        public void Deactivate()
        {
            StopCoroutine(fanRouter);
            fanRouter = null;
            for (int i = 0; i < Row; i++)
                RowFan[i].forceMagnitude = 0;
            for (int i = 0; i < Colum; i++)
                ColumFan[i].forceMagnitude = 0;
        }

        public void ExcaliverMode()
        {
            treasureInst.Opened.RemoveListener(Deactivate);
            if (fanRouter != null)
                StopCoroutine(fanRouter);
            fanRouter = FanRouter();
            StartCoroutine(fanRouter);
        }

        private IEnumerator FanRouter()
        {
            while (true)
            {
                for (int i = 0; i < Row; i++)
                    RowFan[i].forceMagnitude = magnitude;
                for (int i = 0; i < Colum; i++)
                    ColumFan[i].forceMagnitude = magnitude;
                yield return new WaitForSeconds(fanRoutineTime);
                for (int i = 0; i < Row; i++)
                    RowFan[i].forceMagnitude = 0;
                for (int i = 0; i < Colum; i++)
                    ColumFan[i].forceMagnitude = 0;
                yield return new WaitForSeconds(fanRoutineTime);
            }
        }

        #if UNITY_EDITOR
        [ContextMenu("Debug Excaliver Mode")]
        private void DebugExcaliverPulled()
        {
            ExcaliverMode();
        }
        #endif
    }

}