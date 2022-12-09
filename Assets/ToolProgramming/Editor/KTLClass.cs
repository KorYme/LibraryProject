using System;
using System.Reflection;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Stands for KorYme's Tool Library Class
/// </summary>
public sealed class KTLClass
{
    #region CONSTANTS
    public const BindingFlags FLAGS_FIELDS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    #endregion

    #region METHODS
    public static object GetPropertyValue(SerializedProperty property)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.Generic:
                return null;
            case SerializedPropertyType.Integer:
                return property.intValue;
            case SerializedPropertyType.Boolean:
                return property.boolValue;
            case SerializedPropertyType.Float:
                return property.floatValue;
            case SerializedPropertyType.String:
                return property.stringValue;
            case SerializedPropertyType.Color:
                return property.colorValue;
            case SerializedPropertyType.ObjectReference:
                return property.objectReferenceValue;
            case SerializedPropertyType.LayerMask:
                return null;
            case SerializedPropertyType.Enum:
                return property.enumValueIndex;
            case SerializedPropertyType.Vector2:
                return property.vector2Value;
            case SerializedPropertyType.Vector3:
                return property.vector3Value;
            case SerializedPropertyType.Vector4:
                return property.vector4Value;
            case SerializedPropertyType.Rect:
                return property.rectValue;
            case SerializedPropertyType.ArraySize:
                return property.arraySize;
            case SerializedPropertyType.Character:
                return null;
            case SerializedPropertyType.AnimationCurve:
                return property.animationCurveValue;
            case SerializedPropertyType.Bounds:
                return property.boundsValue;
            case SerializedPropertyType.Gradient:
                return null;
            case SerializedPropertyType.Quaternion:
                return property.quaternionValue;
            case SerializedPropertyType.ExposedReference:
                return property.exposedReferenceValue;
            case SerializedPropertyType.FixedBufferSize:
                return property.fixedBufferSize;
            case SerializedPropertyType.Vector2Int:
                return property.vector2IntValue;
            case SerializedPropertyType.Vector3Int:
                return property.vector3IntValue;
            case SerializedPropertyType.RectInt:
                return property.rectIntValue;
            case SerializedPropertyType.BoundsInt:
                return property.boundsIntValue;
            case SerializedPropertyType.ManagedReference:
                return property.managedReferenceValue;
            case SerializedPropertyType.Hash128:
                return property.hash128Value;
            default:
                return null;
        }
    }

    public static string FieldName(string initName)
    {
        List<char> chars = initName.ToCharArray().ToList();
        chars.Remove('_');
        if (chars.Count == 0)
        {
            return "";
        }
        chars[0] = char.ToUpper(chars[0]);
        int index = 1;
        while (index < chars.Count)
        {
            if (char.IsUpper(chars[index]))
            {
                chars.Insert(index, ' ');
                index++;
            }
            index++;
        }
        return new string(chars.ToArray());
    }

    public static GUIStyle ToRichText(GUIStyle style)
    {
        style.richText = true;
        style.normal.textColor = Color.white;
        return style;
    }

    public static string ColorToText(Color color)
    {
        return "#" + ColorUtility.ToHtmlStringRGB(color).ToLower();
    }

    public static string DisplayTextWithColours(string text, Color color)
    {
        return "<color=" + ColorToText(color) + ">" + text + "</color>";
    }
    #endregion
}