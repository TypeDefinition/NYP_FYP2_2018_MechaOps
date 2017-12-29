using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It will make all tiles visible
/// </summary>
public class AllTileVisible : MonoBehaviour {
    [SerializeField, Tooltip("The center of the tile")]
    TileId m_CenterTile;

    TileId[] m_AllOfTheTiles;
    TileSystem m_TileSystem;

    // Use this for initialization
    void Start () {
        m_TileSystem = FindObjectOfType<TileSystem>();
        m_AllOfTheTiles = m_TileSystem.GetSurroundingTiles(m_CenterTile, 999);
        foreach (TileId zeTileID in m_AllOfTheTiles)
        {
            m_TileSystem.GetTile(zeTileID).VisibleCounter++;
        }
	}

    private void OnDestroy()
    {
        foreach (TileId zeTileID in m_AllOfTheTiles)
        {
            m_TileSystem.GetTile(zeTileID).VisibleCounter--;
        }
    }
}
