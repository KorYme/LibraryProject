using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using TMPro;

public class ExposedFieldEditorWindow : EditorWindow
{
    const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    List<Type> _allTypes = new();
    Dictionary<GameObject, Dictionary<Component, List<MemberInfo>>> _allComponents = new();
    Dictionary<GameObject, bool> _foldoutGameObject = new();
    Dictionary<Component, bool> _foldoutComponent = new();

    Vector2 _scrollPos;


    [MenuItem("Tools/Modifications Variables")]
    public static void OpenWindow()
    {
        ExposedFieldEditorWindow window = CreateWindow<ExposedFieldEditorWindow>("GD Modifications");
    }

    public void OnEnable()
    {
        UpdateGUI();
        _scrollPos = Vector2.zero;
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
        _foldoutComponent.Clear();
        _foldoutGameObject.Clear();
        foreach (Type type in _allTypes)
        {
            foreach (Component item in FindObjectsOfType(type))
            {
                List<MemberInfo> allMembers = new();
                foreach (MemberInfo member in item.GetType().GetMembers(FLAGS))
                {
                    if (member.CustomAttributes.ToArray().Length > 0)
                    {
                        ExposedFieldAttribute attribute = member.GetCustomAttribute<ExposedFieldAttribute>();
                        if (attribute != null)
                        {
                            if (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
                            {
                                allMembers.Add(member);
                            }
                        }
                    }
                }
                if (!_allComponents.ContainsKey(item.gameObject))
                {
                    _allComponents.Add(item.gameObject, new());
                    _foldoutGameObject.Add(item.gameObject, false);
                }
                _allComponents[item.gameObject].Add(item, allMembers);
                _foldoutComponent.Add(item, false);
            }
        }
    }

    private void OpenAllFoldout(bool opened)
    {
        foreach (Component item in _foldoutComponent.Keys.ToArray())
        {
            _foldoutComponent[item] = opened;
        }
        foreach (GameObject item in _foldoutGameObject.Keys.ToArray())
        {
            _foldoutGameObject[item] = opened;
        }
    }

    private void OnGUI()
    {
        _scrollPos = GUILayout.BeginScrollView(_scrollPos);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Open All"))
        {
            OpenAllFoldout(true);
        }
        if (GUILayout.Button("Close All"))
        {
            OpenAllFoldout(false);
        }
        if (GUILayout.Button("Refresh"))
        {
            UpdateGUI();
        }

        EditorGUILayout.EndHorizontal();
        foreach (GameObject gameObject in _allComponents.Keys.ToArray())
        {
            EditorGUILayout.Space(10);
            _foldoutGameObject[gameObject] = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutGameObject[gameObject], gameObject.name);
            EditorGUI.indentLevel++;
            if (_foldoutGameObject[gameObject])
            {
                foreach (Component component in _allComponents[gameObject].Keys.ToArray())
                {
                    EditorGUILayout.Space(2);
                    _foldoutComponent[component] = EditorGUILayout.Foldout(_foldoutComponent[component], component.GetType().ToString());
                    EditorGUI.indentLevel++;
                    if (_foldoutComponent[component])
                    {
                        foreach (MemberInfo member in _allComponents[gameObject][component])
                        {
                            FieldInfo field = (FieldInfo)member;
                            EditorGUILayout.LabelField($"{field.Name} - {field.GetValue(component)}"); //TRAITOR A MODIFIER
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        EditorGUILayout.Space(30);
        GUILayout.EndScrollView();
    }


}