using System.Collections.Generic;
using System;
using ToolLibrary;
using UnityEditor;
using UnityEngine;
using Unity.VisualScripting.ReorderableList;

public class PrefabsModificationsEditorWindow : EditorWindow
{
    Vector2 _scrollPos;

    public GameObject[] _prefabs;

    SerializedObject _prefabsObj;

    [MenuItem("Tools/GD Tools/Prefabs Modifications")]
    public static void OpenWindow()
    {
        PrefabsModificationsEditorWindow window = CreateWindow<PrefabsModificationsEditorWindow>("Prefabs Modifications");
    }

    private void OnEnable()
    {
        _prefabsObj = new(this);
    }

    private void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView( _scrollPos );
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Prefabs Modifications", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(_prefabsObj.FindProperty("_prefabs"));
        

        if (EditorGUI.EndChangeCheck())
        {
            //UpdateGUI();
            _prefabsObj.ApplyModifiedProperties();
        }


        EditorGUILayout.EndScrollView();
    }

    void UpdateGUI()
    {
        AttributeSearcher.UpdateAttribute(typeof(GDModifAttribute));
        foreach (Type type in AttributeSearcher._typesByAttributeType[typeof(GDModifAttribute)])
        {
            Debug.Log(type.Name);
        }
    }
}
