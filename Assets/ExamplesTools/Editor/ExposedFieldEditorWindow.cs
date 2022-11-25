using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class ExposedFieldEditorWindow : EditorWindow
{
    const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    List<Type> _allTypes = new();
    Dictionary<GameObject, List<Component>> _allComponents = new();


    [MenuItem("Tools/Modifications Variables")]
    public static void OpenWindow()
    {
        ExposedFieldEditorWindow window = CreateWindow<ExposedFieldEditorWindow>("GD Modifications");
    }

    public void OnEnable()
    {
        UpdateGUI();
    }

    private void UpdateGUI()
    {
        UpdateTypes();
        UpdateDictionary();
    }

    private void UpdateTypes()
    {
        _allTypes.Clear();
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                foreach (MemberInfo member in type.GetMembers(FLAGS))
                {
                    if (member.GetCustomAttribute<ExposedFieldAttribute>() != null)
                    {
                        if (!_allTypes.Contains(type))
                        {
                            _allTypes.Add(type);
                        }
                    }
                }
            }
        }
    }

    private void UpdateDictionary()
    {
        _allComponents.Clear();
        foreach (Type type in _allTypes)
        {
            foreach (Component item in FindObjectsOfType(type))
            {
                if (_allComponents.ContainsKey(item.gameObject))
                {
                    _allComponents[item.gameObject].Add(item);
                }
                else
                {
                    _allComponents.Add(item.gameObject, new List<Component> { item });
                }
            }
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);

        foreach (GameObject gameObject in _allComponents.Keys.ToArray())
        {
            EditorGUILayout.LabelField(gameObject.name);
            foreach (Component component in _allComponents[gameObject])
            {
                EditorGUILayout.LabelField(component.GetType().ToString());
                foreach (MemberInfo member in component.GetType().GetMembers(FLAGS))
                {
                    if (member.CustomAttributes.ToArray().Length > 0)
                    {
                        ExposedFieldAttribute attribute = member.GetCustomAttribute<ExposedFieldAttribute>();
                        if (attribute != null)
                        {
                            if (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
                            {
                                FieldInfo field = (FieldInfo)member;
                                EditorGUILayout.LabelField($"{field.Name} - {field.GetValue(component)}");
                            }
                        }
                    }
                }
            }
        }
    }


}