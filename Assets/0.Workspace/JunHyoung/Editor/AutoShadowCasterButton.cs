using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AutoShadowCasterTilemap))]
public class AutoShadowCasterButton : Editor
{
    private static GUIContent buttonContent;

    private void OnEnable()
    {
        if(buttonContent == null)
        {
            buttonContent = EditorGUIUtility.IconContent("Shadow Icon");
        }
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AutoShadowCasterTilemap caster = (AutoShadowCasterTilemap)target;

        GUILayout.Space(15);
        EditorGUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        buttonContent.text = "    Generate Closed Shadow Caster";
        if (GUILayout.Button(buttonContent, GUILayout.Height(30)))
        {
            caster.GenerateClosedMap();
        }
        buttonContent.text = "    Generate Complex Shadow Caster";
        if (GUILayout.Button(buttonContent, GUILayout.Height(30)))
        {
            caster.Generate();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();
    }
}
