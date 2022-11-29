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
    public const BindingFlags FLAGS_FIELDS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

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

    public static GUIStyle RICH_TEXT
    {
        get
        {
            GUIStyle style = new();
            style.richText= true;
            style.normal.textColor = Color.white;
            return style;
        }
    }

    public static GUIStyle ToRichText(GUIStyle style)
    {
        style.richText = true;
        style.normal.textColor = Color.white;
        return style;
    }
}

public static class AttributeSearcher
{
    public static Dictionary<Type, List<Type>> _typesByAttributeType = new();

    public static void UpdateAttribute(Type attributeType)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (_typesByAttributeType.ContainsKey(attributeType) ? _typesByAttributeType[attributeType].Contains(type) : false) continue;
                foreach (MemberInfo member in type.GetMembers(KTLClass.FLAGS_FIELDS))
                {
                    if (member.GetCustomAttribute(attributeType) != null)
                    {
                        if (!_typesByAttributeType.ContainsKey(attributeType))
                        {
                            _typesByAttributeType.Add(attributeType, new List<Type>() { type });
                        }
                        else
                        {
                            _typesByAttributeType[attributeType].Add(type);
                        }
                    }
                }
            }
        }
    }
}
