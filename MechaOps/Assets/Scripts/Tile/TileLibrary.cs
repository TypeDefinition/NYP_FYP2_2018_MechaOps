﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public enum TileType
{
    Grass = 0,
    Rock,
    Water,
    Sand,
    Forest,

    Num_TileType
}

[System.Serializable]
public class TileAttributes
{
    [SerializeField] private bool m_Walkable = true;
    [SerializeField] private int m_MovementCost = 1;
    [SerializeField] private int m_ConcealmentPoints = 0;
    [SerializeField] private int m_EvasionPoints = 0;
    [SerializeField] private string m_Description;
    // This is the GameObject that will be rendered.
    [SerializeField] private TileDisplay m_DisplayObject = null;

    // These have no setters as they should only ever be changed in the Inspector.
    public bool Walkable { get { return m_Walkable; } }

    public int MovementCost { get { return m_MovementCost; } }

    public int ConcealmentPoints { get { return m_ConcealmentPoints; } }

    public int EvasionPoints { get { return m_EvasionPoints; } }

    public string Description { get { return m_Description; } }

    public TileDisplay DisplayObject { get { return m_DisplayObject; } }

#if UNITY_EDITOR
    public void EditorValidate()
    {
        m_MovementCost = Mathf.Max(1, m_MovementCost);
        m_ConcealmentPoints = Mathf.Max(0, m_ConcealmentPoints);
    }
#endif // UNITY_EDITOR

}

// Have to manually update this file whenever more attributes are added to TileAttributes.
[System.Serializable]
public class TileAttributeOverride
{

    [SerializeField] private TileType m_Type = TileType.Grass;
    [SerializeField] private bool m_Walkable = true;
    [SerializeField] private int m_MovementCost = 1;
    [SerializeField] private int m_ConcealmentPoints = 0;
    [SerializeField] private int m_EvasionPoints = 0;

    public TileType GetTileType() { return m_Type; }

    public void SetTileType(TileType _tileType)
    {
        Assert.IsFalse(_tileType == TileType.Num_TileType, "TileAttributeOverride.Type - Invalid value for m_Type!");
        m_Type = _tileType;
    }

    public bool Walkable { get { return m_Walkable; } }

    public int MovementCost { get { return m_MovementCost; } }

    public int ConcealmentPoints { get { return m_ConcealmentPoints; } }

    public int EvasionPoints { get { return m_EvasionPoints; } }

#if UNITY_EDITOR
    public void EditorValidate()
    {
        if (m_Type == TileType.Num_TileType)
        {
            EditorUtility.DisplayDialog("Invalid Value!", "TileType.TileType_NumTypes is an invalid value for m_Type! Defaulting to TileType.TileType_Normal.", "OK");
            m_Type = TileType.Grass;
        }
        
        m_MovementCost = Mathf.Max(1, m_MovementCost);
        m_ConcealmentPoints = Mathf.Max(0, m_ConcealmentPoints);
    }
#endif // UNITY_EDITOR

}

[System.Serializable, CreateAssetMenu(fileName = "TileLibrary", menuName = "Tile/Tile Library")]
public class TileLibrary : ScriptableObject
{
    [SerializeField] private TileAttributes[] m_Library = new TileAttributes[(uint)TileType.Num_TileType];

    public TileAttributes GetAttribute(TileType _type)
    {
        return m_Library[(uint)_type];
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < m_Library.Length; ++i)
        {
            if (m_Library != null)
            {
                m_Library[i].EditorValidate();
            }
        }
    }
#endif // UNITY_EDITOR

}