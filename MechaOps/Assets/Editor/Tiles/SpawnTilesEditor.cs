using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnTiles))]
public class SpawnTilesEditor : Editor
{
    SerializedProperty m_FactionTileListsProperty;

    void OnEnable()
    {
        m_FactionTileListsProperty = serializedObject.FindProperty("m_FactionTileLists");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        for (int i = 0; i < m_FactionTileListsProperty.arraySize; ++i)
        {
            SerializedProperty property = m_FactionTileListsProperty.GetArrayElementAtIndex(i);
            string propertyName = ((FactionType)i).ToString();
            EditorGUILayout.PropertyField(property, new GUIContent(propertyName), true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}