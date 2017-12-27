using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(GameAudioSettings))]
public class GameAudioSettingsEditor : Editor
{
    SerializedProperty m_VolumeSliderProperty;

    private void OnEnable()
    {
        m_VolumeSliderProperty = serializedObject.FindProperty("m_Volumes");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        for (int i = 0; i < m_VolumeSliderProperty.arraySize; ++i)
        {
            SerializedProperty property = m_VolumeSliderProperty.GetArrayElementAtIndex(i);
            string propertyName = ((VolumeType)i).ToString();
            EditorGUILayout.PropertyField(property, new GUIContent(propertyName), true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}