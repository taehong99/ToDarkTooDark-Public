using System.Collections;
using UnityEngine;

public static class BounceDropHandler
{
    const float dropSpeed = 1.75f;
    const float dropRange = 3;

    const int pingPongCount = 2;
    const float pingPongHeight = 0.5f;
    const float pingPongDuration = 0.8f;


    // RayCast해서 Colider가 없는 방향에, dropRange 범위 이내 랜덤한 정점을 반환하는 메서드
    static Vector3 CalculateDropPoint(Vector3 startPos, float dropRange, LayerMask layerMask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dropRange;
        randomDirection.z = 0;

        RaycastHit2D hit = Physics2D.Raycast(startPos, randomDirection, dropRange, layerMask);
        if (hit.collider != null)
        {
            randomDirection = hit.point - new Vector2(startPos.x, startPos.y);
        }

        return startPos + randomDirection;
    }

    // 현재 Transform.position과 DropPoint 사이에서 랜덤한  정점을 반환하는 메서드
    // 2D이니깐 포물선처럼 보일려면 Vertex는 원점보다 y값이 높으며, x 값은 원점과 드랍포인트 사이에 있어야함

    // endPos가 startPos보다 위라면 원점이 아닌 endPos보다 y값이 높아야하...나?
    public static Vector3 CalculateRandomVertex(Vector3 startPos, Vector3 endPos)
    {
        float midPointX = (startPos.x + endPos.x) / 2;
        float midPointY = Mathf.Max(startPos.y, endPos.y) + Random.Range(1.0f, 3.0f);

        return new Vector3(midPointX, midPointY, 0);
    }

    // 계산한 정점대로 베지어 곡선을 그리며 이동, dropSpeed 기반
    static IEnumerator MoveToBezierCurve(Transform transform, Vector3 startPos, Vector3 vertex, Vector3 endPos, float dropSpeed, bool isPingPong)
    {
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * dropSpeed;
            Vector3 m1 = Vector3.Lerp(startPos, vertex, time);
            Vector3 m2 = Vector3.Lerp(vertex, endPos, time);
            transform.position = Vector3.Lerp(m1, m2, time);
            yield return null;
        }

        if (isPingPong)
            yield return PingPongRoutine(transform, 2);
    }


    // 땅에 통통 튕기는 효과, 점차 줄어드며,  count 수 만큼만 튕김.
    public static void PingPongTransform(this Transform transform, MonoBehaviour mono, int count = pingPongCount, float initialHeight = pingPongHeight, float duration = pingPongDuration)
    {
        mono.StartCoroutine(PingPongRoutine(transform, count, initialHeight, duration));
    }

    private static IEnumerator PingPongRoutine(Transform transform, int count = pingPongCount, float initialHeight = pingPongHeight, float duration = pingPongDuration)
    {
        Vector3 originalPosition = transform.position;
        float time = 0f;
        for (int i = 0; i < count; i++)
        {

            float bounceHeight = initialHeight * (1f - (float) i / count);
            while (time < duration)
            {
                float t = time / duration;
                transform.position = originalPosition + Vector3.up * bounceHeight * Mathf.Sin(t * Mathf.PI);
                time += Time.deltaTime;
                yield return null;
            }
            time = 0f;
        }
        transform.position = originalPosition;
    }

    // 땅에 통통 튕기는 효과인데, x,y 스케일에 왜곡 있음
    public static void JellyPingPongTransform(this Transform transform, MonoBehaviour mono, float intensity = 0.5f, int count = pingPongCount, float initialHeight = pingPongHeight, float duration = pingPongDuration)
    {
        mono.StartCoroutine(JellyPingPongRoutine(transform, intensity, count, initialHeight, duration));
    }


    private static IEnumerator JellyPingPongRoutine(Transform transform, float intensity = 0.5f, int count = pingPongCount, float initialHeight = pingPongHeight, float duration = pingPongDuration)
    {
        Vector3 originalPosition = transform.position;
        Vector3 originalScale = transform.localScale;

        float time = 0f;
        for (int i = 0; i < count; i++)
        {

            float bounceHeight = initialHeight * (1f - (float) i / count);
            while (time < duration)
            {
                float t = time / duration;
                float sinValue = Mathf.Sin(t * Mathf.PI);
                transform.position = originalPosition + Vector3.up * bounceHeight * sinValue;

                // intensity 만큼 강도 주기
                // position y 하강중에는 X 스케일 업, Y 스케일 다운
                if (sinValue < 0)
                {
                    transform.localScale = new Vector3(
                        originalScale.x * (1 - intensity * t),
                        originalScale.y * (1 + intensity * t),
                        originalScale.z);
                }
                else // position y 상승중에는 X 스케일 다운, Y 스케일 업
                {
                    transform.localScale = new Vector3(
                        originalScale.x * (1 + intensity * t),
                        originalScale.y * (1 - intensity * t),
                        originalScale.z);
                }
                time += Time.deltaTime;
                yield return null;
            }
            time = 0f;
        }
        transform.position = originalPosition;
        transform.localScale = originalScale;
    }



    /// <summary>
    /// T해당 Transform을 베지어 곡선으로 이동시켜 Drop되는것만 같은 효과를 보여줄 수 있음, 기본적으로 Wall layer와 겹치지 않도록 떨어트려줌
    /// </summary>
    /// <param name="transform">Target Transform to Move</param>
    /// <param name="monoBehaviour">Need MonoBehaviour to run Couroutine... </param>
    /// <param name="dropRange">Drop Range, Default value is 3</param>
    /// <param name="dropSpeed">Drop Speed, Default value is 1</param>
    public static void BounceDrop(this Transform transform, MonoBehaviour monoBehaviour, LayerMask layerMask, bool isPingPong = false, float dropRange = dropRange, float dropSpeed = dropSpeed)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = CalculateDropPoint(startPos, dropRange, layerMask);
        Vector3 vertex = CalculateRandomVertex(transform.position, endPos);

        monoBehaviour.StartCoroutine(MoveToBezierCurve(transform, transform.position, vertex, endPos, dropSpeed, isPingPong));
    }
}
