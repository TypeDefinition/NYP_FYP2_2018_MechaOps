using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FactionNames))]
public class FactionNamesEditor : Editor
{
    SerializedProperty m_FactionNamesProperty;

    void OnEnable()
    {
        m_FactionNamesProperty = serializedObject.FindProperty("m_FactionNames");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        for (int i = 0; i < m_FactionNamesProperty.arraySize; ++i)
        {
            SerializedProperty property = m_FactionNamesProperty.GetArrayElementAtIndex(i);
            string propertyName = ((FactionType)i).ToString();
            EditorGUILayout.PropertyField(property, new GUIContent(propertyName), true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}