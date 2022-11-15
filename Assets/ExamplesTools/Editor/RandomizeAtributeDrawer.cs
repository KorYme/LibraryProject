using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RandomizeAttribute))]
public class RandomizeAtributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Float)
        {
            EditorGUI.LabelField(position, "Use Test attribute with float");
            return;
        }
        EditorGUI.BeginProperty(position, label, property);
        Rect labelPosition = new Rect(position.x, position.y, position.width, 16f);
        Rect buttonPosition = new Rect(position.x, position.y + labelPosition.height, position.width, 16f);
        EditorGUI.LabelField(labelPosition, label, new GUIContent(property.floatValue.ToString()));
        if (GUI.Button(buttonPosition, "Randomize"))
        {
            RandomizeAttribute target = (RandomizeAttribute)attribute;
            property.floatValue = Random.Range(target.minValue, target.maxValue);
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 32f;
    }
}
