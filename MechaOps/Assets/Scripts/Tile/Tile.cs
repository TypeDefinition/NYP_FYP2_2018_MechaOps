﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Reflection;

/*
The tiles are hexagons.
Their sides are labels from 0 to 6, with 0 being the top, and increasing in the anti-clockwise direction.
*/

[System.Serializable]
public class TileId {

    [SerializeField] private int x, y, z;

    public TileId(int _x = 0, int _y = 0) { Set(_x, _y); }
    public TileId(TileId _other) { Set(_other.x, _other.y); }
    ~TileId() {}

    public int GetX() { return x; }
    public int GetY() { return y; }
    public int GetZ() { return z; }

    // Do not allow the TileId to be changed once created.
    private void Set(int _x, int _y) {
        x = _x; y = _y; z = -(_x + _y);
    }

    public TileId[] GetNeighbors() {
        TileId[] result = new TileId[6];

        // Starting from the top and rotating anti-clockwise.
        result[0] = new TileId(x, y + 1);
        result[1] = new TileId(x - 1, y + 1);
        result[2] = new TileId(x - 1, y);
        result[3] = new TileId(x, y - 1);
        result[4] = new TileId(x + 1, y - 1);
        result[5] = new TileId(x + 1, y);

        return result;
    }

    public static int GetDistance(TileId _origin, TileId _destination) {
        /*
        In the cube coordinate system, each hexagon is a cube in 3d space.
        Adjacent hexagons are distance 1 apart in the hex grid but distance 2 apart in the cube grid.
        This makes distances simple.
        In a square grid, Manhattan distances are abs(dx) + abs(dy).
        In a cube grid, Manhattan distances are abs(dx) + abs(dy) + abs(dz).
        The distance on a hex grid is half that.
        */
        return (Mathf.Abs(_origin.x - _destination.x) + Mathf.Abs(_origin.y - _destination.y) + Mathf.Abs(_origin.z - _destination.z)) >> 1;

        /*
        An equivalent way to write this is by noting that one of the three coordinates
        must be the sum of the other two, then picking that one as the distance.
        You may prefer the “divide by two” form above, or the “max” form here,
        but they give the same result.
        */
        // return Mathf.Max(Mathf.Abs(_origin.x - _destination.x), Mathf.Abs(_origin.y - _destination.y), Mathf.Abs(_origin.z - _destination.z));
    }

    public void PrintDebug() {
        Debug.Log("[" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + "]");
    }

    // It is necessary to override these functions to be able to use TileId as a Key in a HashMap or Dictionary.
    public override int GetHashCode() {
        string combined = x.ToString() + y.ToString() + z.ToString();
        return combined.GetHashCode();
    }

    public override bool Equals(object _obj) {
        return Equals(_obj as TileId);
    }

    public bool Equals(TileId _other) {
        return (_other != null) && (x == _other.x) && (y == _other.y) && (z == _other.z);
    }

}

[DisallowMultipleComponent]
public class Tile : MonoBehaviour {

    private TileId m_Id = null;

    private TileLibrary m_TileLibrary = null;
    [SerializeField] private TileAttributes m_Attributes;
    [SerializeField] private TileDisplay m_DisplayObject;
    [SerializeField] private Hazard m_Hazard = null;
    [SerializeField] private TileType m_Type = TileType.TileType_Normal;
    
    public void InitId(TileId _id) {
        Assert.IsTrue(m_Id == null, MethodBase.GetCurrentMethod().Name + " - InitId can only be called once per Tile!");

        m_Id = _id;
    }

    public void InitLibrary(TileLibrary _library) {
        Assert.IsTrue(m_TileLibrary == null, MethodBase.GetCurrentMethod().Name + " - InitLibrary can only be called once per Tile!");

        m_TileLibrary = _library;
    }

    public TileId GetId() {
        return m_Id;
    }

    public TileType GetTileType() {
        return m_Type;
    }

    public void LoadType() {
        m_Attributes = m_TileLibrary.GetAttribute(m_Type);

        if (m_DisplayObject != null) {
            GameObject.DestroyImmediate(m_DisplayObject);
        }

        if (m_Attributes.m_DisplayObject != null) {
            m_DisplayObject = GameObject.Instantiate(m_Attributes.m_DisplayObject.gameObject).GetComponent<TileDisplay>();
            m_DisplayObject.InitTile(this);
            m_DisplayObject.transform.SetParent(transform);
            m_DisplayObject.transform.position = transform.position;
        }
    }

    public void SetHazard(Hazard _hazard) {
        m_Hazard = _hazard;
    }

    public Hazard GetHazard() {
        return m_Hazard;
    }

    public bool HasHazard() {
        return m_Hazard != null;
    }

}