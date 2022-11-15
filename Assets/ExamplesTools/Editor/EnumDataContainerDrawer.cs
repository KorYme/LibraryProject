using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumDataContainer<,>))]
public class EnumDataContainerDrawer : PropertyDrawer
{
    private const float FOLDOUT_HEIGHT = 16f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty content = property.FindPropertyRelative("_content");
        SerializedProperty enumType = property.FindPropertyRelative("_enumType");
        EditorGUI.BeginProperty(position, label, property);
        Rect foldoutRect = new Rect(position.x, position.y, position.width, FOLDOUT_HEIGHT);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            float addY = FOLDOUT_HEIGHT;
            for (int i = 0; i < content.arraySize; i++)
            {
                Rect rect = new Rect(position.x, position.y, position.width, EditorGUI.GetPropertyHeight(content.GetArrayElementAtIndex(i)));
                addY += rect.height;
                EditorGUI.PropertyField(rect, content.GetArrayElementAtIndex(i), new GUIContent(enumType.enumNames[i]), true);
            }
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty content = property.FindPropertyRelative("_content");
        SerializedProperty enumType = property.FindPropertyRelative("_enumType");
        float height = FOLDOUT_HEIGHT;
        if (property.isExpanded)
        {
            if (content.arraySize != enumType.enumNames.Length)
            {
                content.arraySize = enumType.enumNames.Length;
            }
            for (int i = 0; i < content.arraySize; i++)
            {
                height += EditorGUI.GetPropertyHeight(content.GetArrayElementAtIndex(i));
            }
        }
        return height;
    }
}