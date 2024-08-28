using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MonsterSpawnPoint))]
public class SpriteGizmoDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MonsterSpawnPoint monster = (MonsterSpawnPoint) target;

        if (GUI.changed)
        {
            monster.UpdateSprite();
        }
    }

    void OnSceneGUI()
    {
        MonsterSpawnPoint component = (MonsterSpawnPoint) target;
        if (component.curSprite != null)
        {
            Handles.BeginGUI();
            Vector3 position = component.transform.position;
            Vector2 size = new Vector2(component.curSprite.bounds.size.x, component.curSprite.bounds.size.y);

            // Convert world coordinates to GUI coordinates
            Vector3 screenPoint = Camera.current.WorldToScreenPoint(position);
            Rect rect = new Rect(screenPoint.x - size.x / 2, Screen.height - screenPoint.y - size.y / 2, size.x, size.y);

            GUI.DrawTexture(rect, component.curSprite.texture);
            Handles.EndGUI();
        }
    }
}
