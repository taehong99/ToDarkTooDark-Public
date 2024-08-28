using UnityEditor;
using UnityEngine;

namespace ItemLootSystem
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(BaseItemData))]
    public class ItemDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            EditorGUI.BeginProperty(position, label, property);


            // Calculate the widths for each field
            float padding = 5f;
            float idDataWidth = position.width * 0.15f;
            float itemDataWidth = position.width * 0.15f;
            float nameWidth = position.width * 0.4f;
            float rarityWidth = position.width * 0.2f;


            // SerializedProperty itemDataProp = property.FindPropertyRelative("itemData");
            if (property.objectReferenceValue != null)
            {
                SerializedObject itemDataObject = new SerializedObject(property.objectReferenceValue);

                Rect idRect = new Rect(position.x, position.y, idDataWidth, position.height);
                Rect itemDataRect = new Rect(position.x+ idDataWidth + padding, position.y, itemDataWidth , position.height);
                Rect nameRect = new Rect(position.x + idDataWidth + itemDataWidth + padding+padding, position.y, nameWidth, position.height);
                Rect rarityRect = new Rect(position.x + idDataWidth+ itemDataWidth + nameWidth + padding+padding, position.y, rarityWidth, position.height);

                SerializedProperty itemTierProperty = itemDataObject.FindProperty("rarity");
                SerializedProperty iDProperty = itemDataObject.FindProperty("ID");
                SerializedProperty nameProperty = itemDataObject.FindProperty("Name");

                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.PropertyField(idRect, iDProperty, GUIContent.none);
                EditorGUI.EndDisabledGroup();
                EditorGUI.PropertyField(itemDataRect, property, GUIContent.none);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.PropertyField(nameRect, nameProperty, GUIContent.none);
                EditorGUI.PropertyField(rarityRect, itemTierProperty, GUIContent.none);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, position.height), property, GUIContent.none);
            }

            EditorGUI.EndProperty();
            property.serializedObject.ApplyModifiedProperties();
        }
    }


    [CustomPropertyDrawer(typeof(ItemRarity))]
    public class RarityDrawer : PropertyDrawer
    {
        public static Color GetColorForRarity(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Normal:
                    return Color.white;
                case ItemRarity.Rare:
                    return Color.yellow;
                case ItemRarity.Epic:
                    return new Color(0.5f, 0, 0.5f, .8f);
                case ItemRarity.Legendary:
                    return Color.red;
                case ItemRarity.Unique:
                    return Color.magenta;
                case ItemRarity.Artifact:
                    return Color.green;
                default:
                    return Color.white;
            }

        }

        public static Color GetTextColorForRarity(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Normal:
                    return Color.white;
                case ItemRarity.Rare:
                    return Color.white;
                case ItemRarity.Epic:
                    return Color.white;
                case ItemRarity.Legendary:
                    return Color.yellow;
                case ItemRarity.Unique:
                    return Color.red;
                case ItemRarity.Artifact:
                    return new Color(0.5f, 0, 0.5f, .8f);
                default:
                    return Color.white;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Get the current Rarity enum value
            ItemRarity rarity = (ItemRarity) property.enumValueIndex;

            // Save the default GUI and content colors
            Color defaultBackgroundColor = GUI.backgroundColor;
            Color defaultContentColor = GUI.contentColor;

            // Change the background and text color based on rarity
            GUI.backgroundColor = GetColorForRarity(rarity);
            GUI.contentColor = GetTextColorForRarity(rarity);

            // Draw the property field
            EditorGUI.PropertyField(position, property, label);

            // Reset the GUI and content colors to default
            GUI.backgroundColor = defaultBackgroundColor;
            GUI.contentColor = defaultContentColor;

            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            EditorGUI.LabelField(labelRect, label);

            EditorGUI.EndProperty();
        }
    }
}
#else

        public class RarityDrawer
        {
            public static Color GetColorForRarity(ItemRarity rarity)
            {
                switch (rarity)
                {
                    case ItemRarity.Normal:
                        return Color.white;
                    case ItemRarity.Rare:
                        return Color.yellow;
                    case ItemRarity.Epic:
                        return new Color(0.5f, 0, 0.5f, .8f);
                    case ItemRarity.Legendary:
                        return Color.red;
                    case ItemRarity.Unique:
                        return Color.magenta;
                    case ItemRarity.Artifact:
                        return Color.green;
                    default:
                        return Color.white;
                }

            }

            public static Color GetTextColorForRarity(ItemRarity rarity)
            {
                switch (rarity)
                {
                    case ItemRarity.Normal:
                        return Color.white;
                    case ItemRarity.Rare:
                        return Color.white;
                    case ItemRarity.Epic:
                        return Color.white;
                    case ItemRarity.Legendary:
                        return Color.yellow;
                    case ItemRarity.Unique:
                        return Color.red;
                    case ItemRarity.Artifact:
                        return new Color(0.5f, 0, 0.5f, .8f);
                    default:
                        return Color.white;
                }
            }
        }
    
}
#endif