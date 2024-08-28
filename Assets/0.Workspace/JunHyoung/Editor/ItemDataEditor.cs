using UnityEditor;
using UnityEngine;
using ItemLootSystem;

namespace ItemLootSystem
{

    [CustomEditor(typeof(BaseItemData))]
    public class ItemDataEditor : Editor
    {
        float iconSize = 150;

        public override void OnInspectorGUI()
        {
            SerializedProperty itemIconProp = serializedObject.FindProperty("icon");
            SerializedProperty nameProp = serializedObject.FindProperty("Name");
            SerializedProperty IDProp = serializedObject.FindProperty("ID");
            SerializedProperty weightProp = serializedObject.FindProperty("weight");
            SerializedProperty itemTypeProp = serializedObject.FindProperty("itemType");
            SerializedProperty rarityProp = serializedObject.FindProperty("rarity");
            SerializedProperty tootipProp = serializedObject.FindProperty("toolTip");

            Color defaultColor = GUI.color;

            serializedObject.Update();
            float currentWidth = EditorGUIUtility.currentViewWidth;

            GUIContent title = new GUIContent("Item Data", "Input Item Data");
            GUILayout.Label(title);
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(200));
            EditorGUILayout.LabelField("Name" , GUILayout.MaxWidth(50));
            EditorGUILayout.PropertyField(nameProp, GUIContent.none, GUILayout.MinWidth(150));

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("ID", GUILayout.MaxWidth(20));
            EditorGUILayout.PropertyField(IDProp, GUIContent.none, GUILayout.MinWidth(100));

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space(15);

            // Horizontal Layout
            EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(200));

            // Fist Vertical Layout
            EditorGUILayout.BeginVertical(GUILayout.MinWidth(iconSize));
            EditorGUILayout.LabelField("Icon", GUILayout.MinWidth(iconSize));
            itemIconProp.objectReferenceValue = EditorGUILayout.ObjectField(itemIconProp.objectReferenceValue, typeof(Sprite), false, GUILayout.Width(iconSize), GUILayout.Height(iconSize));
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // 2nd Vertical Layout
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Space(15);

            if (weightProp.intValue == 0)
            {
                GUI.contentColor = Color.red;
                GUI.backgroundColor = Color.red;
            }
            EditorGUILayout.LabelField("Weight", GUILayout.MaxWidth(iconSize));
            EditorGUILayout.PropertyField(weightProp, GUIContent.none);


            GUI.contentColor = defaultColor;
            GUI.backgroundColor = defaultColor;

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Type");
            EditorGUILayout.PropertyField(itemTypeProp, GUIContent.none);


            EditorGUILayout.EndVertical();
            // End Horizontal
            EditorGUILayout.EndHorizontal();

            // 3rd Vertical Layout
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Space(15);

            EditorGUILayout.LabelField("ItemRarity", GUILayout.MaxWidth(iconSize));
            EditorGUILayout.PropertyField(rarityProp, GUIContent.none);
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("ToolTip");
            EditorGUILayout.PropertyField(tootipProp, GUIContent.none, GUILayout.Height(iconSize * 0.5f));

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }


    [CustomEditor(typeof(EquipItemData))]
    public class EqipItemDataEditor : Editor
    {
        float iconSize = 150;

        public override void OnInspectorGUI()
        {
            SerializedProperty itemIconProp = serializedObject.FindProperty("icon");
            SerializedProperty nameProp = serializedObject.FindProperty("Name");
            SerializedProperty IDProp = serializedObject.FindProperty("ID");
            SerializedProperty weightProp = serializedObject.FindProperty("weight");
            SerializedProperty itemTypeProp = serializedObject.FindProperty("itemType");
            SerializedProperty equipmentTypeProp = serializedObject.FindProperty("equipmentType");
            SerializedProperty rarityProp = serializedObject.FindProperty("rarity");
            SerializedProperty tootipProp = serializedObject.FindProperty("toolTip");


            Color defaultColor = GUI.color;

            serializedObject.Update();
            float currentWidth = EditorGUIUtility.currentViewWidth;

            GUIContent title = new GUIContent("Item Data", "Input Item Data");
            GUILayout.Label(title);
            EditorGUILayout.Space(10);

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(200));
            EditorGUILayout.LabelField("Name", GUILayout.MaxWidth(50));
            EditorGUILayout.PropertyField(nameProp, GUIContent.none, GUILayout.MinWidth(150));

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("ID", GUILayout.MaxWidth(20));
            EditorGUILayout.PropertyField(IDProp, GUIContent.none, GUILayout.MinWidth(100));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // Horizontal Layout
            EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(200));

            // Fist Vertical Layout
            EditorGUILayout.BeginVertical(GUILayout.MinWidth(iconSize));
            EditorGUILayout.LabelField("Icon", GUILayout.MinWidth(iconSize));
            itemIconProp.objectReferenceValue = EditorGUILayout.ObjectField(itemIconProp.objectReferenceValue, typeof(Sprite), false, GUILayout.Width(iconSize), GUILayout.Height(iconSize));
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // 2nd Vertical Layout
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Space(15);

            if (weightProp.intValue == 0)
            {
                GUI.contentColor = Color.red;
                GUI.backgroundColor = Color.red;
            }

            EditorGUILayout.LabelField("Weight", GUILayout.MaxWidth(iconSize));
            EditorGUILayout.PropertyField(weightProp, GUIContent.none);

            GUI.contentColor = defaultColor;
            GUI.backgroundColor = defaultColor;

            EditorGUILayout.Space(10);

            if (itemTypeProp.enumValueFlag != 1)
            {
                GUI.contentColor = Color.red;
                GUI.backgroundColor = Color.red;
            }
            EditorGUILayout.LabelField("Type");
            EditorGUILayout.PropertyField(itemTypeProp, GUIContent.none);

            GUI.contentColor = defaultColor;
            GUI.backgroundColor = defaultColor;


            EditorGUILayout.Space(10);

            if (equipmentTypeProp.enumValueFlag == 0)
            {
                GUI.contentColor = Color.red;
                GUI.backgroundColor = Color.red;
            }
            EditorGUILayout.LabelField("Equipment Type");
            EditorGUILayout.PropertyField(equipmentTypeProp, GUIContent.none);
            GUI.contentColor = defaultColor;
            GUI.backgroundColor = defaultColor;


            EditorGUILayout.EndVertical();
            // End Horizontal
            EditorGUILayout.EndHorizontal();

            // 3rd Vertical Layout
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Space(15);

            EditorGUILayout.LabelField("ItemRarity", GUILayout.MaxWidth(iconSize));
            EditorGUILayout.PropertyField(rarityProp, GUIContent.none);
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("ToolTip");
            EditorGUILayout.PropertyField(tootipProp, GUIContent.none, GUILayout.Height(iconSize * 0.5f));

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }


    [CustomEditor(typeof(WeaponItemData))]
    public class WeaponItemDataEditor : Editor
    {
        float iconSize = 150;

        public override void OnInspectorGUI()
        {
            SerializedProperty itemIconProp = serializedObject.FindProperty("icon");
            SerializedProperty nameProp = serializedObject.FindProperty("Name");
            SerializedProperty IDProp = serializedObject.FindProperty("ID");
            SerializedProperty weightProp = serializedObject.FindProperty("weight");
            SerializedProperty itemTypeProp = serializedObject.FindProperty("itemType");
            SerializedProperty weaponTypeProp = serializedObject.FindProperty("weaponType");
            SerializedProperty equipmentTypeProp = serializedObject.FindProperty("equipmentType");
            SerializedProperty rarityProp = serializedObject.FindProperty("rarity");
            SerializedProperty tootipProp = serializedObject.FindProperty("toolTip");


            Color defaultColor = GUI.color;

            serializedObject.Update();
            float currentWidth = EditorGUIUtility.currentViewWidth;

            GUIContent title = new GUIContent("Item Data", "Input Item Data");
            GUILayout.Label(title);
            EditorGUILayout.Space(10);

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(200));
            EditorGUILayout.LabelField("Name", GUILayout.MaxWidth(50));
            EditorGUILayout.PropertyField(nameProp, GUIContent.none, GUILayout.MinWidth(150));

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("ID", GUILayout.MaxWidth(20));
            EditorGUILayout.PropertyField(IDProp, GUIContent.none, GUILayout.MinWidth(100));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // Horizontal Layout
            EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(200));

            // Fist Vertical Layout
            EditorGUILayout.BeginVertical(GUILayout.MinWidth(iconSize));
            EditorGUILayout.LabelField("Icon", GUILayout.MinWidth(iconSize));
            itemIconProp.objectReferenceValue = EditorGUILayout.ObjectField(itemIconProp.objectReferenceValue, typeof(Sprite), false, GUILayout.Width(iconSize), GUILayout.Height(iconSize));
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // 2nd Vertical Layout
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Space(15);

            if (weightProp.intValue == 0)
            {
                GUI.contentColor = Color.red;
                GUI.backgroundColor = Color.red;
            }

            EditorGUILayout.LabelField("Weight", GUILayout.MaxWidth(iconSize));
            EditorGUILayout.PropertyField(weightProp, GUIContent.none);

            GUI.contentColor = defaultColor;
            GUI.backgroundColor = defaultColor;

            if (itemTypeProp.enumValueFlag != 1)
            {
                GUI.contentColor = Color.red;
                GUI.backgroundColor = Color.red;
            }
            EditorGUILayout.LabelField("Type");
            EditorGUILayout.PropertyField(itemTypeProp, GUIContent.none);

            GUI.contentColor = defaultColor;
            GUI.backgroundColor = defaultColor;

            if (equipmentTypeProp.enumValueFlag == 0)
            {
                GUI.contentColor = Color.red;
                GUI.backgroundColor = Color.red;
            }
            EditorGUILayout.LabelField("Equipment Type");
            EditorGUILayout.PropertyField(equipmentTypeProp, GUIContent.none);
            GUI.contentColor = defaultColor;
            GUI.backgroundColor = defaultColor;

            if (weaponTypeProp.enumValueFlag == 0)
            {
                GUI.contentColor = Color.red;
                GUI.backgroundColor = Color.red;
            }
            EditorGUILayout.LabelField("Weapon Type");
            EditorGUILayout.PropertyField(weaponTypeProp, GUIContent.none);
            GUI.contentColor = defaultColor;
            GUI.backgroundColor = defaultColor;

            EditorGUILayout.EndVertical();
            // End Horizontal
            EditorGUILayout.EndHorizontal();

            // 3rd Vertical Layout
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Space(15);

            EditorGUILayout.LabelField("ItemRarity", GUILayout.MaxWidth(iconSize));
            EditorGUILayout.PropertyField(rarityProp, GUIContent.none);
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("ToolTip");
            EditorGUILayout.PropertyField(tootipProp, GUIContent.none, GUILayout.Height(iconSize * 0.5f));

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}