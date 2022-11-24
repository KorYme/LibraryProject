using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class ExposedFieldEditorWindow : EditorWindow
{
    [MenuItem("Tools/Exposed Variables")]
    public static void OpenWindow()
    {
        ExposedFieldEditorWindow window = CreateWindow<ExposedFieldEditorWindow>("Exposed Variables");
    }

    List<ExposedFieldInfo> exposedMembers = new();

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
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("ExposedProperties", EditorStyles.boldLabel);

        foreach (ExposedFieldInfo member in exposedMembers)
        {
            EditorGUILayout.LabelField($"{member._memberInfo.ReflectedType} - {member._exposedFieldAttribute._displayName}");
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