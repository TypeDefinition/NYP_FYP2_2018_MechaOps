using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileAttributeOverride))]
[CanEditMultipleObjects]
public class TileAttributeOverrideEditor : Editor {

    SerializedProperty m_OverridesProperty;

    void OnEnable()
    {
        m_OverridesProperty = serializedObject.FindProperty("m_Overrides");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        for (int i = 0; i < m_OverridesProperty.arraySize; ++i)
        {
            SerializedProperty property = m_OverridesProperty.GetArrayElementAtIndex(i);
            string propertyName = ((TileType)i).ToString();
            EditorGUILayout.PropertyField(property, new GUIContent(propertyName), true);
        }

        serializedObject.ApplyModifiedProperties();
    }

}