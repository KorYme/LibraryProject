using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrefabsModificationsEditorWindow : EditorWindow
{
    Vector2 _scrollPos;

    GameObject _prefab;

    [MenuItem("Tools/GD Tools/Prefabs Modifications")]
    public static void OpenWindow()
    {
        PrefabsModificationsEditorWindow window = CreateWindow<PrefabsModificationsEditorWindow>("Prefabs Modifications");
    }

    private void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView( _scrollPos );
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Prefabs Modifications", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        EditorGUI.BeginChangeCheck();
        _prefab = (GameObject)EditorGUILayout.ObjectField("Example GO", _prefab, typeof(GameObject), false);
        if (EditorGUI.EndChangeCheck())
        {
            UpdateGUI();
        }

        EditorGUILayout.EndScrollView();
    }

    void UpdateGUI()
    {

    }
}
