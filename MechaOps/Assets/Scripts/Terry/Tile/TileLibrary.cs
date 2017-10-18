using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {
    TileType_Normal = 0,
    TileType_FullCover,
    TileType_HalfCover,
    TileType_Obstacle,
    TileType_Water,
    TileType_Forest,

    TileType_NumTypes
}

[System.Serializable]
public class TileAttributes {
    // This is the GameObject that will be rendered.
    public int m_MovementCost = 0;
    public int m_ConcealmentPoints = 0;
    public GameObject m_DisplayObject = null;
}

[DisallowMultipleComponent]
public class TileLibrary : MonoBehaviour {

    [SerializeField]
    private TileAttributes[] m_Library = new TileAttributes[(uint)TileType.TileType_NumTypes];

    public TileAttributes GetAttribute(TileType _type) {
        return m_Library[(uint)_type];
    }
	
}