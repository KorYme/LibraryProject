using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using ToolLibrary;

public class ExposedFieldEditorWindow : EditorWindow
{
    
    Vector2 _scrollPos;
    bool _isHierarchised;

    List<Type> _allTypes = new();
    Dictionary<GameObject, Dictionary<Component, Tuple<SerializedObject, List<FieldInfo>>>> _allComponents = new();
    Dictionary<GameObject, bool> _foldoutGameObject = new();
    Dictionary<Component, bool> _foldoutComponent = new();
    List<Node> _allNodes = new();

    class Node
    {
        public GameObject _go;
        public bool _isOriginal;
        public List<Node> _allChilds;

        public Node(GameObject go, bool isOriginal, List<Node> nodes)
        {
            _go = go;
            _isOriginal = isOriginal;
            _allChilds = nodes;
        }
    }



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
        CreateBinaryTree();
    }

    private void UpdateTypes()
    {
        _allTypes.Clear();
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                foreach (MemberInfo member in type.GetMembers(ToolLibraryClass.FLAGS_FIELDS))
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
            foreach (Component component in FindObjectsOfType(type))
            {
                List<FieldInfo> allFields = new();
                foreach (MemberInfo member in component.GetType().GetMembers(ToolLibraryClass.FLAGS_FIELDS))
                {
                    if (member.CustomAttributes.ToArray().Length > 0)
                    {
                        ExposedFieldAttribute attribute = member.GetCustomAttribute<ExposedFieldAttribute>();
                        if (attribute != null)
                        {
                            if (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
                            {
                                allFields.Add((FieldInfo)member);
                            }
                        }
                    }
                }
                if (!_allComponents.ContainsKey(component.gameObject))
                {
                    _allComponents.Add(component.gameObject, new());
                    _foldoutGameObject.Add(component.gameObject, false);
                }
                _allComponents[component.gameObject].Add(component, Tuple.Create(new SerializedObject(component), allFields));
                _foldoutComponent.Add(component, false);
            }
        }
    }

    private void CreateBinaryTree()
    {
        _allNodes.Clear();
        foreach (GameObject go in _allComponents.Keys.ToArray())
        {
            _allNodes.Add(new Node(go, true, new()));
        }
        for (int i = 0; i < _allNodes.Count; i++)
        {
            Transform tempParent = _allNodes[i]._go.transform.parent;
            while (tempParent.parent != null && _allNodes[i]._isOriginal)
            {
                if (_allComponents.Keys.ToList().Contains(tempParent.gameObject))
                {
                    _allNodes[i]._isOriginal = false;
                    _allNodes.Find(x=>x._go == tempParent.gameObject)._allChilds.Add(_allNodes[i]);
                }
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
        _isHierarchised = EditorGUILayout.Toggle(new GUIContent("Hierarchy"), _isHierarchised);
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
        if (!_isHierarchised)
        {
            EditorGUILayout.BeginVertical();
            foreach (GameObject gameObject in _allComponents.Keys.ToArray().OrderBy(x => x.name))
            {
                EditorGUILayout.Space(10);
                _foldoutGameObject[gameObject] = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutGameObject[gameObject], gameObject.name);
                EditorGUI.indentLevel++;
                if (_foldoutGameObject[gameObject])
                {
                    foreach (Component component in _allComponents[gameObject].Keys.ToArray().OrderBy(x => x.name))
                    {
                        EditorGUILayout.Space(2);
                        _foldoutComponent[component] = EditorGUILayout.Foldout(_foldoutComponent[component], component.GetType().ToString());
                        EditorGUI.indentLevel++;
                        if (_foldoutComponent[component])
                        {
                            foreach (FieldInfo field in _allComponents[gameObject][component].Item2)
                            {
                                SerializedProperty serializedProperty = _allComponents[gameObject][component].Item1.FindProperty(field.Name);
                                EditorGUILayout.PropertyField(serializedProperty, new GUIContent(ToolLibraryClass.FieldName(field.Name)));
                                field.SetValue(component, ToolLibraryClass.GetValue(serializedProperty));
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            EditorGUILayout.Space(30);
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.BeginVertical();
            foreach (Node node in _allNodes.Where(x=>x._isOriginal).OrderBy(x => x._go.name))
            {
                EditorGUILayout.Space(10);
                _foldoutGameObject[node._go] = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutGameObject[node._go], node._go.name);
                EditorGUI.indentLevel++;
                if (_foldoutGameObject[node._go])
                {
                    foreach (Component component in _allComponents[node._go].Keys.ToArray().OrderBy(x => x.name))
                    {
                        EditorGUILayout.Space(2);
                        _foldoutComponent[component] = EditorGUILayout.Foldout(_foldoutComponent[component], component.GetType().ToString());
                        EditorGUI.indentLevel++;
                        if (_foldoutComponent[component])
                        {
                            foreach (FieldInfo field in _allComponents[node._go][component].Item2)
                            {
                                SerializedProperty serializedProperty = _allComponents[node._go][component].Item1.FindProperty(field.Name);
                                EditorGUILayout.PropertyField(serializedProperty, new GUIContent(ToolLibraryClass.FieldName(field.Name)));
                                field.SetValue(component, ToolLibraryClass.GetValue(serializedProperty));
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                    DisplayChildGameObjects(node._allChilds);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            EditorGUILayout.Space(30);
            EditorGUILayout.EndVertical();
        }
        GUILayout.EndScrollView();
    }

    private void DisplayChildGameObjects(List<Node> allChilds)
    {
        foreach (Node node in allChilds.OrderBy(x => x._go.name))
        {
            EditorGUILayout.Space(10);
            _foldoutGameObject[node._go] = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutGameObject[node._go], node._go.name);
            EditorGUI.indentLevel++;
            if (_foldoutGameObject[node._go])
            {
                foreach (Component component in _allComponents[node._go].Keys.ToArray().OrderBy(x => x.name))
                {
                    EditorGUILayout.Space(2);
                    _foldoutComponent[component] = EditorGUILayout.Foldout(_foldoutComponent[component], component.GetType().ToString());
                    EditorGUI.indentLevel++;
                    if (_foldoutComponent[component])
                    {
                        foreach (FieldInfo field in _allComponents[node._go][component].Item2)
                        {
                            SerializedProperty serializedProperty = _allComponents[node._go][component].Item1.FindProperty(field.Name);
                            EditorGUILayout.PropertyField(serializedProperty, new GUIContent(ToolLibraryClass.FieldName(field.Name)));
                            field.SetValue(component, ToolLibraryClass.GetValue(serializedProperty));
                        }
                    }
                    EditorGUI.indentLevel--;
                }
                DisplayChildGameObjects(node._allChilds);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}