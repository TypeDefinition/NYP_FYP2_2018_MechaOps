using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameEventNames))]
public class GameEventNamesEditor : Editor
{
    SerializedProperty m_EventNameProperty;

    private void SetArrayName<T>(string _propertyName, string _headerName)
    {
        EditorGUILayout.LabelField(_headerName, EditorStyles.boldLabel);

        m_EventNameProperty = serializedObject.FindProperty(_propertyName);

        for (int i = 0; i < m_EventNameProperty.arraySize; ++i)
        {
            SerializedProperty property = m_EventNameProperty.GetArrayElementAtIndex(i);
            string propertyName = ((T)(object)i).ToString();
            EditorGUILayout.PropertyField(property, new GUIContent(propertyName), true);
        }
    }

    /// <summary>
    /// Update this whenever new arrays or enums are added to GameEventNames!
    /// </summary>
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SetArrayName<GameEventNames.SpawnSystemNames>("m_SpawnSystemNames", "Spawn System");
        SetArrayName<GameEventNames.GameAudioNames>("m_GameAudioNames", "Game Audio");
        SetArrayName<GameEventNames.GameplayNames>("m_GameplayNames", "Gameplay");
        SetArrayName<GameEventNames.GameUINames>("m_GameUINames", "Game UI");
        SetArrayName<GameEventNames.TouchGestureNames>("m_TouchGestureNames", "Touch Gesture");
        //SetArrayName<GameEventNames.GameSystemsNames>("m_GameSystemsNames", "Game Systems");

        serializedObject.ApplyModifiedProperties();
    }
}