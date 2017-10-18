using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
The tiles are hexagons.
Their sides are labels from 0 to 6, with 0 being the top, and increasing in the anti-clockwise direction.
*/

using TileId = System.Int32;

[DisallowMultipleComponent]
public class Tile : MonoBehaviour {

    // Constants in C# are Static.
    public const TileId INVALID_ID = -1;

    public TileLibrary m_TileLibrary = null;

    [SerializeField] private int m_Id = INVALID_ID;
    [SerializeField] private Tile[] m_Neighbours = new Tile[6];

    [SerializeField] private Hazard m_Hazard = null;
    [SerializeField] private TileAttributes m_Attributes;
    [SerializeField] private GameObject m_DisplayObject;
    [SerializeField] private TileType m_Type = TileType.TileType_Normal;

    public void SetId(TileId _id) {
        m_Id = Mathf.Max(INVALID_ID, _id);
    }

    public TileId GetId() {
        return m_Id;
    }

    public void SetNeighbour(Tile _neighbour, uint _index) {
        m_Neighbours[_index] = _neighbour;

        if (_neighbour != null) {
            _neighbour.m_Neighbours[(_index + 3) % 6] = this;
        }
    }

    public Tile[] GetNeighbours() {
        return m_Neighbours;
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
            m_DisplayObject = GameObject.Instantiate(m_Attributes.m_DisplayObject);

            m_DisplayObject.transform.SetParent(transform);
            m_DisplayObject.transform.position = gameObject.transform.position;
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

    public void Reset() {
        m_Id = INVALID_ID;
        m_Neighbours = new Tile[6];
    }

    private void Awake() {
    }
    
    // Use this for initialization
    private void Start() {
    }

    // Update is called once per frame
    private void Update() {
    }
    
}