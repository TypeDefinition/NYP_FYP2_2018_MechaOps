using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TileBuilderSettings : EditorWindow
{
    private bool m_SelectTilesOnly = false;

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
        m_SelectTilesOnly = EditorGUILayout.Toggle("Select Tiles Only", m_SelectTilesOnly);
    }

    private void Update()
    {
        if (m_SelectTilesOnly)
        {
            GameObject[] selection = Selection.gameObjects;
            List<GameObject> tileList = new List<GameObject>();
            HashSet<int> tileListInstanceIds = new HashSet<int>();
            for (int i = 0; i < selection.Length; ++i)
            {
                if (selection[i].tag == "Tile")
                {
                    // Ensure that we do not add the same object to the list twice.
                    if (!tileListInstanceIds.Contains(selection[i].GetInstanceID()))
                    {
                        tileList.Add(selection[i]);
                        tileListInstanceIds.Add(selection[i].GetInstanceID());
                    }
                    continue;
                }

                Transform parentTransform = selection[i].transform.parent;
                while (parentTransform != null)
                {
                    if (parentTransform.gameObject.tag == "Tile")
                    {
                        // Ensure that we do not add the same object to the list twice.
                        if (!tileListInstanceIds.Contains(selection[i].GetInstanceID()))
                        {
                            tileList.Add(parentTransform.gameObject);
                            tileListInstanceIds.Add(selection[i].GetInstanceID());
                        }
                        break;
                    }
                    parentTransform = parentTransform.transform.parent;
                }
            }

            Selection.objects = tileList.ToArray();
        }
    }

}