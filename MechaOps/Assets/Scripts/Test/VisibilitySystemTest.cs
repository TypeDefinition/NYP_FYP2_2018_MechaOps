using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilitySystemTest : MonoBehaviour
{
    [SerializeField] private TileSystem m_TileSystem = null;
    [SerializeField] private int m_KnownRadius = 12;
    [SerializeField] private int m_VisibleRadius = 8;

    // Use this for initialization
    void Start ()
    {
        TileId[] surroundingTiles = m_TileSystem.GetSurroundingTiles(new TileId(0, 0), m_KnownRadius);
        for (int i = 0; i < surroundingTiles.Length; ++i)
        {
            Tile tile = m_TileSystem.GetTile(surroundingTiles[i]);
            tile.VisibleCounter += 1;

            if (TileId.GetDistance(surroundingTiles[i], new TileId(0, 0)) > m_VisibleRadius)
            {
                tile.VisibleCounter -= 1;
            }
        }
	}

}