using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    TileType_Normal = 0,
    TileType_FullCover,
    TileType_HalfCover,
    TileType_Obstacle,
    TileType_Swirl,
    TileType_Stealth,

    TileType_NumTypes
}

[System.Serializable]
public class TileAttributes
{
    
    // This is the GameObject that will be rendered.
    [SerializeField] private int m_MovementCost = 0;
    [SerializeField] private int m_ConcealmentPoints = 0;
    [SerializeField] private TileDisplay m_DisplayObject = null;

    // These have no setters as they should only ever be changed in the Inspector.
    public int MovementCost
    {
        get
        {
            return m_MovementCost;
        }
    }

    public int ConcealmentPoints
    {
        get
        {
            return m_ConcealmentPoints;
        }
    }

    public TileDisplay DisplayObject
    {
        get
        {
            return m_DisplayObject;
        }
    }

    public void Validate()
    {
        m_MovementCost = Mathf.Max(1, m_MovementCost);
        m_ConcealmentPoints = Mathf.Max(0, m_ConcealmentPoints);
    }

}

[DisallowMultipleComponent]
public class TileLibrary : MonoBehaviour
{

    [SerializeField] private TileAttributes[] m_Library = new TileAttributes[(uint)TileType.TileType_NumTypes];

    public TileAttributes GetAttribute(TileType _type)
    {
        return m_Library[(uint)_type];
    }

    private void OnValidate()
    {
        for (int i = 0; i < m_Library.Length; ++i)
        {
            if (m_Library != null)
            {
                m_Library[i].Validate();
            }
        }
    }

}