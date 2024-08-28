using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MapGenerator;

[CustomEditor(typeof(MapGenerator.MapGenerator))]
public class MapGeneratorButton : Editor
{

    private static GUIContent genrateButton;
    private static GUIContent seedButton;

    public void OnEnable()
    {
        if (genrateButton == null)
            genrateButton = EditorGUIUtility.IconContent("Tilemap Icon");

        if (seedButton == null)
            seedButton = EditorGUIUtility.IconContent("PlatformEffector2D Icon");

        genrateButton.text = "   Generate Map";
        seedButton.text = "   New Seed";
    }

    public override void OnInspectorGUI()
    {
        MapGenerator.MapGenerator mapGenerator = (MapGenerator.MapGenerator)target;

        EditorGUILayout.BeginVertical();
        GUILayout.Space(10);


        if (GUILayout.Button(genrateButton ,GUILayout.Height(40)))
        {
            mapGenerator.GenerateMap();
        }
        
        GUILayout.Space(10);

        if (GUILayout.Button(seedButton, GUILayout.Height(40)))
        {
            mapGenerator.InitSeed();
        }
        GUILayout.Space(30);
        EditorGUILayout.EndVertical();
        base.OnInspectorGUI();
    }
}
