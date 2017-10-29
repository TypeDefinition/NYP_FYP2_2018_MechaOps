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
            if (tileSystem.GetNumTiles() == 0 || EditorUtility.DisplayDialog("TileSystem - Generate Tiles", "Are you sure you want to generate new tiles? This will clear all current tiles in this TileSystem.", "Yes", "No"))
            {
                tileSystem.GenerateTiles();
                EditorUtility.SetDirty(tileSystem);
            }
        }

        if (GUILayout.Button("Clear Tiles")) {
            if (EditorUtility.DisplayDialog("TileSystem - Clear Tiles", "Are you sure you want to clear all tiles in this TileSystem?", "Yes", "No"))
            {
                tileSystem.ClearTiles();
                EditorUtility.SetDirty(tileSystem);
            }
            
        }

        if (GUILayout.Button("Randomize Tile Types"))
        {
            if (EditorUtility.DisplayDialog("TileSystem - Randomize Tile Types", "Are you sure you want to randomize tile types? This will replace all existing tiles.", "Yes", "No"))
            {
                tileSystem.RandomizeTileTypes();
                EditorUtility.SetDirty(tileSystem);
            }
        }

        if (GUILayout.Button("Randomize Hazard Types"))
        {
            if (EditorUtility.DisplayDialog("TileSystem - Randomize Hazard Types", "Are you sure you want to randomize hazard types? This will replace all existing hazards.", "Yes", "No"))
            {
                tileSystem.RandomizeHazardTypes();
                EditorUtility.SetDirty(tileSystem);
            }
        }

        if (GUILayout.Button("Load Tile & Hazard Types"))
        {
            tileSystem.LoadTileAndHazardTypes();
            EditorUtility.SetDirty(tileSystem);
        }
    }

}