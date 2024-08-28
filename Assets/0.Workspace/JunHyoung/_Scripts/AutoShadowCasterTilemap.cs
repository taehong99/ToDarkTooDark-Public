using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

[RequireComponent(typeof(CompositeCollider2D))]
public class AutoShadowCasterTilemap : MonoBehaviour
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


    // TileMap에서 모서리 하나 비워두고 생성하면 오류 없이 잘 생성됨 (...) 
    // 부모에 Composite ShadowCaster2D 
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

        var index = 0;
        var endPath = unionVertices[unionVertices.Count - 1];
        var length = Vector2.Distance(pathVertices[0], endPath);

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

        Debug.Log("Shadow Generate Done");

    }



    [ContextMenu("Generate")]
    public void Generate()
    {
        DestroyAllChildren();

        tilemapCollider = GetComponent<CompositeCollider2D>();

        for (int i = 0; i < tilemapCollider.pathCount; i++)
        {
            Vector2[] pathVertices = new Vector2[tilemapCollider.GetPathPointCount(i)];
            tilemapCollider.GetPath(i, pathVertices);
            GameObject shadowCaster = new GameObject("shadow_caster_" + i);
            shadowCaster.transform.parent = gameObject.transform;
            ShadowCaster2D shadowCasterComponent = shadowCaster.AddComponent<ShadowCaster2D>();
            shadowCasterComponent.selfShadows = this.selfShadows;

            Vector3[] testPath = new Vector3[pathVertices.Length];
            for (int j = 0; j < pathVertices.Length; j++)
            {
                testPath[j] = pathVertices[j];
            }

            shapePathField.SetValue(shadowCasterComponent, testPath);
            shapePathHashField.SetValue(shadowCasterComponent, Random.Range(int.MinValue, int.MaxValue));
            meshField.SetValue(shadowCasterComponent, new Mesh());
            generateShadowMeshMethod.Invoke(shadowCasterComponent, new object[] { meshField.GetValue(shadowCasterComponent), shapePathField.GetValue(shadowCasterComponent) });
        }

        Debug.Log("Shadow Generate Done");
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