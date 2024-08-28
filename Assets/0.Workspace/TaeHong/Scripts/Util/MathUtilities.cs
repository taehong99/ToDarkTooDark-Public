using UnityEngine;
using System.Collections.Generic;
using System;

namespace Tae
{
    public static class MathUtilities
    {
        public static float Square(float value)
        {
            return value * value;
        }

        public static float EaseOutQuad(float t)
        {
            return 1 - (1 - t) * (1 - t);
        }

        public static float EaseInQuad(float t)
        {
            return t * t;
        }

        public static Vector2 QuadraticBezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector2 p = uu * p0;
            p += 2 * u * t * p1;
            p += tt * p2;

            return p;
        }

        public static Vector2 CubicBezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector2 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }

        //Fisher-Yates List Shuffle
        public static void ShuffleList<T>(List<T> list, int seed)
        {
            UnityEngine.Random.InitState(seed);
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1); // 0 <= j <= i
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        // List element swap
        public static void SwapElements<T>(this List<T> list, int i, int j)
        {
            if (i < 0 || i >= list.Count || j < 0 || j >= list.Count)
            {
                throw new ArgumentOutOfRangeException("Index out of range");
            }

            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}