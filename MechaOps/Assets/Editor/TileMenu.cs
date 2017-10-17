using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileMenu {

    [MenuItem("Tile/Generate Tiles")]
    static void GenerateTiles() {
        //Debug.Log("Do Something");
        if (Selection.activeObject == null) {
            Debug.Log("No TileSystem selected. Unable to generate tiles.");
        }
        else {
            GameObject tileSystem = (GameObject)Selection.activeObject;
            if (tileSystem.GetComponent<TileSystem>() == null) {
                Debug.Log("Selected GameObject has no TileSystem. Unable to generate tiles.");
            }
            else {
                tileSystem.GetComponent<TileSystem>().GenerateTiles();
                //Undo.RecordObject(gridSystem.GetComponent<GridSystem>(), gridSystem.name + "Load Grid");
                EditorUtility.SetDirty(tileSystem.GetComponent<TileSystem>());
            }
        }
    }

    [MenuItem("Tile/Clear Tiles")]
    static void ClearTiles() {
        //Debug.Log("Do Something");
        if (Selection.activeObject == null) {
            Debug.Log("No TileSystem selected. Unable to clear tiles.");
        }
        else {
            GameObject tileSystem = (GameObject)Selection.activeObject;
            if (tileSystem.GetComponent<TileSystem>() == null) {
                Debug.Log("Selected GameObject has no TileSystem. Unable to clear tiles.");
            }
            else {
                tileSystem.GetComponent<TileSystem>().ClearTiles();
                //Undo.RecordObject(gridSystem.GetComponent<GridSystem>(), gridSystem.name + "Load Grid");
                EditorUtility.SetDirty(tileSystem.GetComponent<TileSystem>());
            }
        }
    }

}
