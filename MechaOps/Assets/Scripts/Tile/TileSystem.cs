using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Reflection;

// References:
// Hexagon Grid - https://www.redblobgames.com/grids/hexagons

/*
The tiles are flat topped hexagons.
Their sides are labels from 0 to 6, with 0 being the top, and increasing in the anti-clockwise direction.
*/

[System.Serializable]
public class TileInfo {

    [SerializeField] private TileId m_Id;
    [SerializeField] private Tile m_Tile;

    public TileInfo(TileId _id, Tile _tile) {
        m_Id = _id;
        m_Tile = _tile;
    }

    ~TileInfo() {}

    public TileId GetId() {
        return m_Id;
    }

    public Tile GetTile() {
        return m_Tile;
    }

}

[System.Serializable]
public class TileDictionaryPair {

    public TileId m_Id = null;
    public TileInfo m_TileInfo = null;

    public TileDictionaryPair(TileId _id, TileInfo _info) {
        m_Id = _id;
        m_TileInfo = _info;
    }

}

[DisallowMultipleComponent]
public class TileSystem : MonoBehaviour {

    [SerializeField] private TileLibrary m_TileLibrary = null;
    [SerializeField] private Tile m_DefaultTile = null;

    private Dictionary<TileId, TileInfo> m_TileDictionary = new Dictionary<TileId, TileInfo>();
    [HideInInspector, SerializeField] private TileDictionaryPair[] m_TileArray = new TileDictionaryPair[0];

    public int m_Radius = 10;
    public float m_DistanceBetweenTiles = 1.0f;

    public TileLibrary GetTileLibrary() {
        return m_TileLibrary;
    }

    public Tile GetDefaultTile() {
        return m_DefaultTile;
    }

    public TileInfo GetTileInfo(TileId _id) {
        return m_TileDictionary.ContainsKey(_id) ? (TileInfo)m_TileDictionary[_id] : null;
    }

    public void GenerateTiles() {
        // Delete existing tiles.
        ClearTiles();

        // Create the tiles.
        /*
        Given a hex center and a range N, which hexes are within N steps from it?
        We can work backwards from the hex distance formula, distance = max(abs(dx), abs(dy), abs(dz)).
        To find all hexes within N steps, we need max(abs(dx), abs(dy), abs(dz)) ≤ N.
        This means we need all three of: abs(dx) ≤ N and abs(dy) ≤ N and abs(dz) ≤ N.
        Removing absolute value, we get -N ≤ dx ≤ N and -N ≤ dy ≤ N and -N ≤ dz ≤ N

        for (int x = -m_Radius; x <= m_Radius; ++x) {
            for (int y = -m_Radius; y <= m_Radius; ++y) {
                for (int z = -m_Radius; z <= m_Radius; ++z) {
                    if (x + y + z == 0) {
                        // Create Tile.
                    }
                }
            }
        }
        */

        // The above can be further optimised.
        // I understood this code by drawing out the hexagon grid on a piece of paper and seeing the order
        // which the loop generated the hexagons and compared the TileId(s).
        for (int x = -m_Radius; x <= m_Radius; ++x) {
            for (int y = Mathf.Max(-m_Radius, -m_Radius-x); y <= Mathf.Min(m_Radius, m_Radius-x); ++y) {
                TileId tileId = new TileId(x, y);
                Tile tile = GameObject.Instantiate(m_DefaultTile).GetComponent<Tile>();
                tile.InitId(tileId);
                tile.SetTileSystem(this);
                tile.gameObject.name = "Tile (" + tileId.GetX().ToString() + ", " + tileId.GetY().ToString() + ", " + tileId.GetZ().ToString() + ")";
                // The x, y and z here are the axis of the grid, not the world.
                Vector3 xOffset = new Vector3(1.0f, 0.0f, 0.0f) * tileId.GetX();
                Vector3 yOffset = new Vector3(Mathf.Cos(120.0f * Mathf.Deg2Rad), 0.0f, Mathf.Sin(120.0f * Mathf.Deg2Rad)) * tileId.GetY();
                Vector3 zOffset = new Vector3(Mathf.Cos(240.0f * Mathf.Deg2Rad), 0.0f, Mathf.Sin(240.0f * Mathf.Deg2Rad)) * tileId.GetZ();
                // The reason we multiply by 0.5 here is because the Ids of adjacemt tiles are 2 apart in a cube coordinate.
                // So if we do not multiple 0.5, the tiles will be twice as far apart as they should be.
                tile.transform.position = transform.position + (xOffset + yOffset + zOffset) * m_DistanceBetweenTiles * 0.5f;
                tile.transform.SetParent(transform);

                tile.LoadType();

                m_TileDictionary.Add(tileId, new TileInfo(tileId, tile));
            }
        }

        // After the tiles have been generated, we need to store it in m_TileArray.
        // This is because m_TileDictionary cannot be serialized, and it will be reseted when the game plays.
        // As a workaround, we will store the info in m_TileArray for permanent storage,
        // and copy it into m_TileDictionary when needed.
        {
            m_TileArray = new TileDictionaryPair[m_TileDictionary.Count];
            int index = 0;
            foreach (KeyValuePair<TileId, TileInfo> iter in m_TileDictionary) {
                m_TileArray[index++] = new TileDictionaryPair(iter.Key, iter.Value);
            }
        }
    }

    private void LoadDictionary() {
        m_TileDictionary.Clear();
        for (int i = 0; i < m_TileArray.Length; ++i) {
            m_TileDictionary.Add(m_TileArray[i].m_Id, m_TileArray[i].m_TileInfo);
        }
    }

    public void ClearTiles() {
        LoadDictionary();

        if (m_TileDictionary.Count == 0) {
            //Debug.Log(MethodBase.GetCurrentMethod().Name + " - No tiles to clear!");
            return;
        }

        // Destroy any existing tiles.
        foreach (TileInfo value in m_TileDictionary.Values) {
            if (value != null) {
                GameObject.DestroyImmediate(value.GetTile().gameObject);
            }
        }

        m_TileArray = new TileDictionaryPair[0];
        m_TileDictionary.Clear();
    }

    public TileId[] GetSurroundingTiles(TileId _centre, int _radius) {
        // _radius should never be < 0.
        Assert.IsFalse(_radius < 0, MethodBase.GetCurrentMethod().Name + " - Invalid value for _radius!");

        List<TileId> result = new List<TileId>();

        for (int x = -_radius; x <= _radius; ++x) {
            for (int y = Mathf.Max(-_radius, -_radius - x); y <= Mathf.Min(_radius, _radius - x); ++y) {
                TileId tileId = new TileId(x + _centre.GetX(), y + _centre.GetY());
        
                // Check that the tile exist (in case of tiles near/at the edge of the map).
                if (m_TileDictionary.ContainsKey(tileId)) {
                    result.Add(tileId);
                }
            }
        }

        return result.ToArray();
    }
    
    public void LoadTileTypes()
    {
        LoadDictionary();

        if (m_TileDictionary.Count == 0)
        {
            Debug.Log(MethodBase.GetCurrentMethod().Name + " - No tiles to load!");
            return;
        }

        foreach (TileInfo value in m_TileDictionary.Values)
        {
            if (value != null && value.GetTile() != null)
            {
                value.GetTile().LoadType();
            }
        }

    }

    public int GetNumTiles() {
        LoadDictionary();

        return m_TileDictionary.Count;
    }

    private void OnValidate() {
        m_Radius = Mathf.Max(0, m_Radius);
        m_DistanceBetweenTiles = Mathf.Max(0.0f, m_DistanceBetweenTiles);
    }

}