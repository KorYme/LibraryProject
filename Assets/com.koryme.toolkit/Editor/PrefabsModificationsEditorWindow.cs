using System.Collections.Generic;
using System;
using ToolLibrary;
using UnityEditor;
using UnityEngine;
using Unity.VisualScripting.ReorderableList;
using System.IO;
using System.Linq;

public class PrefabsModificationsEditorWindow : EditorWindow
{
    Vector2 _scrollPos;

    public enum PREFABS_FOLDERS
    {
        PREFABS,
        RESOURCES,
        BOTH,
        OTHER,
    }

    public PREFABS_FOLDERS _prefabsFolders;
    public List<GameObject> _prefabs;


    SerializedObject _thisObject;
    SerializedProperty _folderPathProperty;

    [MenuItem("Fabien's courses/GD Tools/Prefabs Modifications")]
    public static void OpenWindow()
    {
        PrefabsModificationsEditorWindow window = CreateWindow<PrefabsModificationsEditorWindow>("Prefabs Modifications");
    }

    private void OnEnable()
    {
        _thisObject = new(this);
        _folderPathProperty = _thisObject.FindProperty("_prefabsFolders");
    }

    private void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView( _scrollPos );
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Prefabs Modifications", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_folderPathProperty, new GUIContent("Prefabs Folders"));
        if (EditorGUI.EndChangeCheck())
        {
            _thisObject.ApplyModifiedProperties();
        }
        EditorGUILayout.Space(5);
        if (GUILayout.Button("Refresh"))
        {
            UpdateList();
        }

        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("AllPrefabs", EditorStyles.boldLabel);
        EditorGUILayout.EndScrollView();
    }

    void UpdateList()
    {
        _prefabs.Clear();
        switch (_prefabsFolders)
        {
            case PREFABS_FOLDERS.PREFABS:
                AddToPrefabsByPath("Prefabs/");
                return;
            case PREFABS_FOLDERS.RESOURCES:
                AddToPrefabsByResources();
                return;
            case PREFABS_FOLDERS.BOTH:
                AddToPrefabsByPath("Prefabs/");
                AddToPrefabsByResources();
                return;
            default:
                return;
        }
    }

    void AddToPrefabsByResources()
    {
        GameObject[] tmp = Resources.LoadAll<GameObject>("");
        for (int i = 0; i < tmp.Length; i++)
        {
            _prefabs.Add(tmp[i]);
            Debug.Log(_prefabs[i].name);
        }
    }

    void AddToPrefabsByPath(string path)
    {
        string[] search_results = System.IO.Directory.GetFiles($"Assets/{path}", "*.prefab", System.IO.SearchOption.AllDirectories);
        for (int i = 0; i < search_results.Length; i++)
        {
            _prefabs.Add((GameObject)AssetDatabase.LoadAssetAtPath(search_results[i], typeof(GameObject)));
            Debug.Log(_prefabs[i].name);
        }
    }
}
