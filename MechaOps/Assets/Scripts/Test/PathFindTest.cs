using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class PathFindTest : MonoBehaviour
{
    public TileSystem m_TileSystem;
    public TileId m_StartTile;
    public TileId m_EndTile;
    public bool m_ClearPathMarkers = false;

    [Range(0, 10000)] public int m_MovementPoints;
    [SerializeField] public TileAttributeOverride[] m_Overrides;

    private void Start()
    {
        TileId[] reachableArea = m_TileSystem.GetReachableTiles(m_MovementPoints, m_StartTile, m_Overrides);
        TileId[] path = m_TileSystem.GetPath(m_MovementPoints, m_StartTile, m_EndTile, m_Overrides);
        m_TileSystem.SetPathMarkers(reachableArea, path);
    }

    private void Update()
    {
        if (m_ClearPathMarkers)
        {
            m_TileSystem.ClearPathMarkers();
            m_ClearPathMarkers = false;
        }
    }

}