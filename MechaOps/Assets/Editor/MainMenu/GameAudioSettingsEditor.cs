using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(GameAudioSettings))]
public class GameAudioSettingsEditor : Editor
{
    SerializedProperty m_GameEventNamesProperty;
    SerializedProperty m_VolumesProperty;

    private void OnEnable()
    {
        m_GameEventNamesProperty = serializedObject.FindProperty("m_GameEventNames");
        m_VolumesProperty = serializedObject.FindProperty("m_Volumes");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GameAudioSettings gameAudioSettings = (GameAudioSettings)target;

        {
            EditorGUILayout.BeginHorizontal();
            GameEventNames gameEventNames = (GameEventNames)EditorGUILayout.ObjectField("Game Event Names: ", gameAudioSettings.GetGameEventNames(), typeof(GameEventNames), false);
            gameAudioSettings.SetGameEventNames(gameEventNames);
            EditorGUILayout.EndHorizontal();
            EditorUtility.SetDirty(gameAudioSettings);
        }

        for (int i = 0; i < m_VolumesProperty.arraySize; ++i)
        {
            SerializedProperty property = m_VolumesProperty.GetArrayElementAtIndex(i);
            string propertyName = ((VolumeType)i).ToString();
            EditorGUILayout.PropertyField(property, new GUIContent(propertyName), true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}