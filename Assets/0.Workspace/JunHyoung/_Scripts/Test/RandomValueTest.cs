using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RandomTest
{
    public class RandomValueTest : MonoBehaviour
    {
        [SerializeField] int seed;

        [ContextMenu("RandomValueTest")]
        void RandomValue()
        {
            Random.InitState(seed);
            for(int i = 0; i < 10; i++) {
                Debug.Log(Random.Range(0, 100));
            }
        }
    }
}