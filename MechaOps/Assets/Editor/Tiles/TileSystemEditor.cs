using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileSystem))]
[CanEditMultipleObjects]
public class TileSystemEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        TileSystem tileSystem = (TileSystem)target;
        if (GUILayout.Button("Generate Tiles")) {
            tileSystem.GenerateTiles();
            EditorUtility.SetDirty(tileSystem);
        }

        if (GUILayout.Button("Clear Tiles")) {
            tileSystem.ClearTiles();
            EditorUtility.SetDirty(tileSystem);
        }
    }

}