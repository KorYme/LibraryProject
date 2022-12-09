using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using ToolLibrary;

public class SceneModificationsEditorWindow : EditorWindow
{
    #region PARAMETERS
    Vector2 _scrollPos;
    bool _areParametersOpened;
    bool _isHierarchised;
    Color _colorGameObjectLabel;
    Color _colorComponentLabel;
    #endregion

    #region CONTAINERS
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
    List<Type> _allTypes = new();
    Dictionary<GameObject, Dictionary<Component, Tuple<SerializedObject, List<FieldInfo>>>> _allComponents = new();
    Dictionary<GameObject, bool> _foldoutGameObject = new();
    Dictionary<Component, bool> _foldoutComponent = new();
    List<Node> _allNodes = new();
    #endregion

    #region UNITY_METHODS
    [MenuItem("Tools/Fabien's courses/Scene Modifications")]
    public static void OpenWindow()
    {
        SceneModificationsEditorWindow window = CreateWindow<SceneModificationsEditorWindow>("Scene Modifications");
    }

    public void OnEnable()
    {
        InitAllContainers();
        if (_colorComponentLabel == Color.clear && _colorGameObjectLabel == Color.clear)
        {
            _colorGameObjectLabel = Color.red;
            _colorComponentLabel = Color.blue;
        }
    }

    private void OnHierarchyChange()
    {
        CheckAllContainers();
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
        EditorGUILayout.EndHorizontal();
        if (_areParametersOpened = EditorGUILayout.Foldout(_areParametersOpened, "[PARAMETERS]", true, KTLClass.ToRichText("foldout")))
        {
            EditorGUI.indentLevel++;
            _isHierarchised = EditorGUILayout.ToggleLeft(new GUIContent("Hierarchy"), _isHierarchised);
            _colorGameObjectLabel = EditorGUILayout.ColorField(new GUIContent("Parameters color"), _colorGameObjectLabel);
            _colorComponentLabel = EditorGUILayout.ColorField(new GUIContent("Parameters color"), _colorComponentLabel);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("[ALLFIELDS]", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.BeginVertical();
        if (!_isHierarchised)
        {
            foreach (GameObject gameObject in _allComponents.Keys.ToArray().OrderBy(x => x.transform.GetSiblingIndex()))
            {
                EditorGUILayout.Space(10);
                if (_foldoutGameObject[gameObject] = EditorGUILayout.Foldout(_foldoutGameObject[gameObject],
                    KTLClass.DisplayTextWithColours("[GAMEOBJECT] ", _colorGameObjectLabel) + gameObject.name, true, KTLClass.ToRichText("foldout")))
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.Space(10);
                    using (new EditorGUI.DisabledScope(true)) EditorGUILayout.ObjectField("GameObject",
                        gameObject, typeof(GameObject), false);
                    foreach (Component component in _allComponents[gameObject].Keys.ToArray().OrderBy(x => x.name))
                    {
                        EditorGUILayout.Space(2);
                        if (_foldoutComponent[component] = EditorGUILayout.Foldout(_foldoutComponent[component],
                            KTLClass.DisplayTextWithColours("[COMPONENT] ", _colorComponentLabel) + component.GetType().ToString(),
                            true, KTLClass.ToRichText("foldout")))
                        {
                            EditorGUI.indentLevel++;
                            foreach (FieldInfo field in _allComponents[gameObject][component].Item2)
                            {
                                SerializedProperty serializedProperty = _allComponents[gameObject][component].Item1.FindProperty(field.Name);
                                EditorGUI.BeginChangeCheck();
                                EditorGUILayout.PropertyField(serializedProperty, new GUIContent(KTLClass.FieldName(field.Name)));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    _allComponents[gameObject][component].Item1.ApplyModifiedProperties();
                                }
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }
        else
        {

            foreach (Node node in _allNodes.Where(x=>x._isOriginal).OrderBy(x => x._go.transform.GetSiblingIndex()))
            {
                EditorGUILayout.Space(10);
                if (_foldoutGameObject[node._go] = EditorGUILayout.Foldout(_foldoutGameObject[node._go],
                    KTLClass.DisplayTextWithColours("[GAMEOBJECT] ", _colorGameObjectLabel) + node._go.name, true, KTLClass.ToRichText("foldout")))
                {
                    EditorGUI.indentLevel++;
                    using (new EditorGUI.DisabledScope(true)) EditorGUILayout.ObjectField("GameObject", node._go, typeof(GameObject), false);
                    foreach (Component component in _allComponents[node._go].Keys.ToArray().OrderBy(x => x.name))
                    {
                        EditorGUILayout.Space(2);
                        if (_foldoutComponent[component] = EditorGUILayout.Foldout(_foldoutComponent[component],
                            KTLClass.DisplayTextWithColours("[COMPONENT] ", _colorComponentLabel) + component.GetType().ToString(), 
                            true, KTLClass.ToRichText("foldout")))
                        {
                            EditorGUI.indentLevel++;
                            foreach (FieldInfo field in _allComponents[node._go][component].Item2)
                            {
                                SerializedProperty serializedProperty = _allComponents[node._go][component].Item1.FindProperty(field.Name);
                                EditorGUI.BeginChangeCheck();
                                EditorGUILayout.PropertyField(serializedProperty, new GUIContent(KTLClass.FieldName(field.Name)));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    _allComponents[node._go][component].Item1.ApplyModifiedProperties();
                                }
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                    DisplayChildGameObjects(node._allChilds);
                    EditorGUI.indentLevel--;
                }
            }
        }
        EditorGUILayout.Space(30);
        EditorGUILayout.EndVertical();
        GUILayout.EndScrollView();
    }
    #endregion

    #region INITIALIZATIONS

    private void InitAllContainers()
    {
        InitTypes();
        InitDictionary();
        InitHierarchyTree();
        _scrollPos = Vector2.zero;
    }

    private void InitTypes()
    {
        _allTypes.Clear();
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                foreach (MemberInfo member in type.GetMembers(KTLClass.FLAGS_FIELDS))
                {
                    if (member.GetCustomAttribute<GDModifAttribute>() != null)
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
    
    private void InitDictionary()
    {
        _allComponents.Clear();
        _foldoutComponent.Clear();
        _foldoutGameObject.Clear();
        foreach (Type type in _allTypes)
        {
            foreach (Component component in FindObjectsOfType(type))
            {
                List<FieldInfo> allFields = new();
                foreach (MemberInfo member in component.GetType().GetMembers(KTLClass.FLAGS_FIELDS))
                {
                    if (member.CustomAttributes.ToArray().Length > 0)
                    {
                        GDModifAttribute attribute = member.GetCustomAttribute<GDModifAttribute>();

                        if (attribute != null)
                        {
                            if (!attribute._isASceneObject)
                            {
                                continue;
                            }
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

    private void InitHierarchyTree()
    {
        _allNodes.Clear();
        foreach (GameObject go in _allComponents.Keys.ToArray())
        {
            _allNodes.Add(new Node(go, true, new()));
        }
        for (int i = 0; i < _allNodes.Count; i++)
        {
            Transform tempParent = _allNodes[i]._go.transform.parent;
            while (tempParent != null && _allNodes[i]._isOriginal)
            {
                if (_allComponents.Keys.ToList().Contains(tempParent.gameObject))
                {
                    _allNodes[i]._isOriginal = false;
                    _allNodes.Find(x=>x._go == tempParent.gameObject)._allChilds.Add(_allNodes[i]);
                }
                tempParent= tempParent.parent;
            }
        }
    }
    #endregion

    #region CHECKS
    private void CheckAllContainers()
    {
        CheckDictionnary();
        InitHierarchyTree();
        _scrollPos = Vector2.zero;
    }

    private void CheckDictionnary()
    {
        RemoveNullKeysDictionary();
        AddNewKeysDictionary();
    }

    private void RemoveNullKeysDictionary()
    {
        foreach (GameObject gameObject in _allComponents.Keys.ToArray())
        {
            if (gameObject == null)
            {
                _allComponents.Remove(gameObject);
                _foldoutGameObject.Remove(gameObject);
            }
            foreach (Component component in _allComponents[gameObject].Keys.ToArray())
            {
                if (component == null)
                {
                    _allComponents[gameObject].Remove(component);
                    _foldoutComponent.Remove(component);
                }
            }
        }
    }

    private void AddNewKeysDictionary()
    {
        foreach (Type type in _allTypes)
        {
            foreach (Component component in FindObjectsOfType(type))
            {
                if ((_allComponents.ContainsKey(component.gameObject) ? !_allComponents[component.gameObject].ContainsKey(component) : false) ||
                    !_allComponents.ContainsKey(component.gameObject))
                {
                    List<FieldInfo> allFields = new();
                    foreach (MemberInfo member in component.GetType().GetMembers(KTLClass.FLAGS_FIELDS))
                    {
                        if (member.CustomAttributes.ToArray().Length > 0)
                        {
                            GDModifAttribute attribute = member.GetCustomAttribute<GDModifAttribute>();
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
    }
    #endregion

    #region GUI_METHODS
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

    private void DisplayChildGameObjects(List<Node> allChilds)
    {
        foreach (Node node in allChilds.OrderBy(x => x._go.transform.GetSiblingIndex()))
        {
            EditorGUILayout.Space(10);
            if (_foldoutGameObject[node._go] = EditorGUILayout.Foldout(_foldoutGameObject[node._go],
                KTLClass.DisplayTextWithColours("[GAMEOBJECT] ", _colorGameObjectLabel) + node._go.name, true, KTLClass.ToRichText("foldout")))
            {
                EditorGUI.indentLevel++;
                using (new EditorGUI.DisabledScope(true)) EditorGUILayout.ObjectField("GameObject", node._go, typeof(GameObject), false);
                foreach (Component component in _allComponents[node._go].Keys.ToArray().OrderBy(x => x.name))
                {
                    EditorGUILayout.Space(2);
                    if (_foldoutComponent[component] = EditorGUILayout.Foldout(_foldoutComponent[component], KTLClass.DisplayTextWithColours("[COMPONENT] ",
                        _colorComponentLabel) + component.GetType().ToString(), false, KTLClass.ToRichText("foldout")))
                    {
                        EditorGUI.indentLevel++;
                        foreach (FieldInfo field in _allComponents[node._go][component].Item2)
                        {
                            SerializedProperty serializedProperty = _allComponents[node._go][component].Item1.FindProperty(field.Name);
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(serializedProperty, new GUIContent(KTLClass.FieldName(field.Name)));
                            if (EditorGUI.EndChangeCheck())
                            {
                                _allComponents[node._go][component].Item1.ApplyModifiedProperties();
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                DisplayChildGameObjects(node._allChilds);
                EditorGUI.indentLevel--;
            }
        }
    }
    #endregion
}