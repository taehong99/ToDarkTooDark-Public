using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

[RequireComponent(typeof(CompositeCollider2D))]
public class AutoShadowCasterTilemapCustom: MonoBehaviour
{

    [Space]
    [SerializeField]
    private bool selfShadows = false;

    private CompositeCollider2D tilemapCollider;
    private List<Vector2> unionVertices = new();


    static readonly FieldInfo meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly FieldInfo shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly FieldInfo shapePathHashField = typeof(ShadowCaster2D).GetField("m_ShapePathHash", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly MethodInfo generateShadowMeshMethod = typeof(ShadowCaster2D)
            .Assembly
            .GetType("UnityEngine.Rendering.Universal.ShadowUtility")
            .GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);

    [Header("Debug")]
    [SerializeField] GameObject Dot1;
    [SerializeField] GameObject Dot2;

    [ContextMenu("Generate(ClosedMap)")]
    public void GenerateClosedMap()
    {
        tilemapCollider = GetComponent<CompositeCollider2D>();

        if (tilemapCollider.pathCount != 2)
        {
            print("Shadow must be used in one closed tiles. Please erase the other tiles to other Tilemap.");
            return;
        }

        unionVertices.Clear();
        DestroyAllChildren();


        Vector2[] pathVertices = new Vector2[tilemapCollider.GetPathPointCount(0)];
        tilemapCollider.GetPath(0, unionVertices);
        tilemapCollider.GetPath(1, pathVertices);
        Debug.Log($"unionVertices : {unionVertices.Count}");
        Debug.Log($"pathVertices : {pathVertices.Length}");


        var index = 0;
        var endPath = unionVertices[unionVertices.Count - 1];
        var length = Vector2.Distance(pathVertices[0], endPath);
        Debug.Log($"endPath : {endPath}");
        Debug.Log($"pathVertices : {pathVertices[0]}");
        Dot1.transform.position = endPath;
        Dot2.transform.position = pathVertices[0];

        for (var i = 1; i < pathVertices.Length; i++)
        {
            var path = pathVertices[i];
            var curLen = Vector2.Distance(endPath, path);
            if (curLen < length)
            {
                length = curLen;
                index = i;
            }
        }

        for (var i = 0; i < pathVertices.Length; i++)
        {
            var path = pathVertices[(i + index) % pathVertices.Length];
            unionVertices.Add(path);
        }

        for (var i = 0; i < unionVertices.Count; i++)
        {
            var cur = unionVertices[i];
            var next = unionVertices[(i + 1) % unionVertices.Count];

            Debug.DrawLine(cur, cur + (next - cur) * 0.5f, Color.red);
        }

        var shadowCaster = new GameObject("ShadowCaster");
        shadowCaster.transform.parent = gameObject.transform;
        shadowCaster.transform.position = Vector3.zero;
        ShadowCaster2D shadowCasterComponent = shadowCaster.AddComponent<ShadowCaster2D>();
        shadowCasterComponent.selfShadows = this.selfShadows;

        var testPath = new Vector3[unionVertices.Count];
        var j = 0;
        foreach (var path in unionVertices)
        {
            testPath[j++] = path;
        }

        shapePathField.SetValue(shadowCasterComponent, testPath);
        meshField.SetValue(shadowCasterComponent, new Mesh());
        generateShadowMeshMethod.Invoke(shadowCasterComponent, new object[] { meshField.GetValue(shadowCasterComponent), shapePathField.GetValue(shadowCasterComponent) });

        Debug.Log("Shadow Generated");

    }



    [ContextMenu("Generate")]
    public void Generate()
    {
        DestroyAllChildren();

        tilemapCollider = GetComponent<CompositeCollider2D>();

        // CompositeCollider2D에 생성된 선의 집합만큼 시행
        for (int i = 0; i < tilemapCollider.pathCount; i++)
        {
            // CompositeCollider2D의 콜라이더 테두리 윤곽선 꼭지점 위치를 저장한 Vector2 list를 불러옴
            Vector2[] pathVertices = new Vector2[tilemapCollider.GetPathPointCount(i)];
            tilemapCollider.GetPath(i, pathVertices);
            // shadow_caster_숫자를 이름으로 ShadowCaster2D 컴포넌트를 가지는 GameObject 생성 및 자식으로 이동
            GameObject shadowCaster = new GameObject("shadow_caster_" + i);
            shadowCaster.transform.parent = gameObject.transform;
            ShadowCaster2D shadowCasterComponent = shadowCaster.AddComponent<ShadowCaster2D>();
            shadowCasterComponent.selfShadows = this.selfShadows;   // selfshadow init설정 설정할지 지정한것을 여기서 적용

            // testPath에 pathVertices복사
            Vector3[] testPath = new Vector3[pathVertices.Length];
            for (int j = 0; j < pathVertices.Length; j++)
            {
                testPath[j] = pathVertices[j];
            }

            // 생성한 shadowCasterComponent에 위치값을 넣음으로서 자동으로 그림자 물체 생성
            shapePathField.SetValue(shadowCasterComponent, testPath);
            shapePathHashField.SetValue(shadowCasterComponent, Random.Range(int.MinValue, int.MaxValue));
            meshField.SetValue(shadowCasterComponent, new Mesh());
            generateShadowMeshMethod.Invoke(shadowCasterComponent, new object[] { meshField.GetValue(shadowCasterComponent), shapePathField.GetValue(shadowCasterComponent) });
        }

        Debug.Log("Generate Done");
    }

    public void DestroyAllChildren()
    {

        var tempList = transform.Cast<Transform>().ToList();
        foreach (var child in tempList)
        {
            DestroyImmediate(child.gameObject);
        }

    }
}