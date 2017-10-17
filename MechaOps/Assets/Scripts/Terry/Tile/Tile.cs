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

    [SerializeField]
    private int m_Id = INVALID_ID;
    [SerializeField]
    public Tile[] m_Neighbours = new Tile[6];

    public void SetId(TileId _id) {
        m_Id = Mathf.Max(INVALID_ID, _id);
    }

    public TileId GetId() {
        return m_Id;
    }

    public void Reset() {
        m_Id = INVALID_ID;
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