using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class ExposedFieldEditorWindow : EditorWindow
{
    [MenuItem("Tools/Modifications Variables")]
    public static void OpenWindow()
    {
        ExposedFieldEditorWindow window = CreateWindow<ExposedFieldEditorWindow>("GD Modifications");
    }

    List<ExposedFieldInfo> exposedMembers = new();
    Dictionary<string, bool> alreadyListed = new();

    public void OnEnable()
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (Assembly assembly in assemblies)
        {
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                MemberInfo[] members = type.GetMembers(flags);
                foreach (MemberInfo member in members)
                {
                    if (member.CustomAttributes.ToArray().Length > 0)
                    {
                        ExposedFieldAttribute attribute = member.GetCustomAttribute<ExposedFieldAttribute>();
                        if (attribute != null)
                        {
                            exposedMembers.Add(new ExposedFieldInfo(member, attribute));
                        }
                    }
                }
            }
        }
        UpdateGUI();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("ExposedProperties", EditorStyles.boldLabel);
        foreach (string key in alreadyListed.Keys.ToArray())
        {
            alreadyListed[key] = EditorGUILayout.BeginFoldoutHeaderGroup(alreadyListed[key], key);
            if (alreadyListed[key])
            {
                foreach (ExposedFieldInfo member in exposedMembers.Where(x => x._exposedFieldAttribute._gameObjectName == key).ToList())
                {
                    EditorGUILayout.LabelField($"{member._exposedFieldAttribute._displayName} -");
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }

    private void UpdateGUI()
    {
        alreadyListed.Clear();
        foreach (ExposedFieldInfo member in exposedMembers)
        {
            if (!alreadyListed.ContainsKey(member._exposedFieldAttribute._gameObjectName))
            {
                alreadyListed.Add(member._exposedFieldAttribute._gameObjectName, false);
            }
        }
    }
}

public struct ExposedFieldInfo
{
    public MemberInfo _memberInfo;
    public ExposedFieldAttribute _exposedFieldAttribute;
    public ExposedFieldInfo(MemberInfo info, ExposedFieldAttribute attribute)
    {
        _memberInfo = info;
        _exposedFieldAttribute = attribute;
        if (attribute._displayName == null)
        {
            attribute._displayName = info.Name;
        }
    }
}