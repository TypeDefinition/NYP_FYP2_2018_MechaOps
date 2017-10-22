using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TileBuilderSettings : EditorWindow
{

    private bool m_ForceSelectionToParent = false;

	[MenuItem("Window/Tile Builder Settings")]
    static void ShowWindow()
    {
        //Get existing open window or if none, make a new one.
        TileBuilderSettings window = (TileBuilderSettings)EditorWindow.GetWindow(typeof(TileBuilderSettings), false, "Tile Builder Settings");
        window.Show();
    }

    private void OnGUI()
    {
        GUIStyle warningStyle = new GUIStyle(EditorStyles.boldLabel);
        warningStyle.font = EditorStyles.boldFont;
        warningStyle.fontSize = 25;
        GUILayout.Label("使用时请保持焦点!", warningStyle);

        GUILayout.Label("Tile Building", EditorStyles.boldLabel);
        m_ForceSelectionToParent = EditorGUILayout.Toggle("Force Selection To Parent", m_ForceSelectionToParent);
    }

    private void Update()
    {
        if (m_ForceSelectionToParent &&
            Selection.activeTransform != null &&
            Selection.activeTransform.parent != null &&
            Selection.activeTransform.gameObject.CompareTag("TileDisplay"))
        {
            Selection.activeTransform = Selection.activeTransform.parent;
        }
    }

}