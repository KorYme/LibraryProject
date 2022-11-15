using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumDataAttribute))]
public class EnumDataAttributeDrawer : PropertyDrawer
{
    SerializedProperty array;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EnumDataAttribute target = (EnumDataAttribute)attribute;
        string path = property.propertyPath;
        if (array == null)
        {
            array = property.serializedObject.FindProperty(path.Substring(0, path.LastIndexOf('.')));
            if (array == null)
            {
                EditorGUI.LabelField(position, "Use EnumAttribute on arrays.");
                return;
            }
        }
        if (array.arraySize != target.names.Length)
        {
            array.arraySize = target.names.Length;
        }
        int indexOfNum = path.LastIndexOf('[') + 1;
        int index = System.Convert.ToInt32(path.Substring(indexOfNum, path.LastIndexOf(']') - indexOfNum));
        label.text = target.names[index];
        EditorGUI.PropertyField(position, property, label, true);
    }
}
