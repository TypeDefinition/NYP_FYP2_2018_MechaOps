using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ViewTileScript : MonoBehaviour {
    [Header("Variables needed for PlayerViewScript")]
    [SerializeField, Tooltip("Unit stats. Link it!")]
    protected UnitStats m_Stats;
    [SerializeField, Tooltip("Tile system of the game. Linking not required")]
    protected TileSystem m_TileSys;

    public abstract void RenderSurroundingTiles();
}
