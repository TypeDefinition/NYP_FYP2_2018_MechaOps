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
        SetArrayName<GameEventNames.SceneManagementNames>("m_SceneManagementNames", "Scene Management");

        GameEventNames gameEventNames = (GameEventNames)target;
        if (GUILayout.Button("Validate Game Event Names"))
        {
            if (gameEventNames.HasDuplicateNames())
            {
                EditorUtility.DisplayDialog("Duplicate Names Found!", "Good luck finding out which ones. I haven't (and might never) add a functionality to automagically find out which names are the problematic ones.", "Oh, fuck...");
            }
            else
            {
                EditorUtility.DisplayDialog("All Is Well!", "There seems to be nothing wrong... as far as I can tell.", "Sure, I guess.");
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}