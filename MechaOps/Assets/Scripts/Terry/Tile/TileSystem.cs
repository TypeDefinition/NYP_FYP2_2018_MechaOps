using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
The tiles are hexagons.
Their sides are labels from 0 to 6, with 0 being the top, and increasing in the anti-clockwise direction.
*/

using TileId = System.Int32;

public class TileInfo {

    // The tiles are hexagons.
    public const int NUM_SIDES = 6; // Might as well hardcode this to 6. This whole file will break if the number changes.

    public TileId m_Id;
    public int m_Ring;
    public TileInfo[] m_Neighbours;
    public Tile m_Tile;

    public TileInfo(TileId _id, int _ring) {
        m_Id = _id;
        m_Ring = _ring;
        m_Neighbours = new TileInfo[6];
        m_Tile = null;
        for (int i = 0; i < NUM_SIDES; ++i) {
            m_Neighbours[i] = null;
        }
    }

    ~TileInfo() {}

    public void SetNeighbour(TileInfo _neighbourTile, int _index) {
        if (_neighbourTile == null) {
            m_Neighbours[_index] = null;
            m_Tile.m_Neighbours[_index] = null;
        } else {
            m_Neighbours[_index] = _neighbourTile;
            m_Tile.m_Neighbours[_index] = _neighbourTile.m_Tile;

            _neighbourTile.m_Neighbours[(_index + 3) % 6] = this;
            _neighbourTile.m_Tile.m_Neighbours[(_index + 3) % 6] = this.m_Tile;
        }
    }

}

[DisallowMultipleComponent]
public class TileSystem : MonoBehaviour {

    [Range(0, 5000)]
    public int m_Radius = 20;
    public Tile m_DefaultTile;

    //[SerializeField]
    private TileInfo[] m_TileInfos;

    public int m_NumTiles = 0;

    public void ClearTiles() {
        if (m_TileInfos == null) {
            return;
        }

        // Destroy any existing tiles.
        for (int i = 0; i < m_TileInfos.Length; ++i) {
            GameObject tileObject = m_TileInfos[i].m_Tile.gameObject;
            if (tileObject != null) {
                GameObject.DestroyImmediate(tileObject);
            }
        }

        m_TileInfos = null;
    }

    private int CalculateNumTiles(int _radius) {
        // _radius should never be < 0.
        if (_radius < 0) {
            Debug.Log("TileSystem.CalculateNumTiles - Invalid value for _radius!");
            return 0;
        }

        // We start from 1 because if the radius is 0, there is still 1 tile.
        // Becareful of the <= or < sign when looping.
        int result = 1;
        for (int i = 1; i <= _radius; ++i) {
            result += i * TileInfo.NUM_SIDES;
        }

        return result;
    }
	
    private TileId CalculateRingStartId(int _ring) {
        // Special case for 0.
        if (_ring == 0) {
            return 0;
        }

        // We have to add 1 to the result, or we will get the last id of the previous ring.
        TileId result = 1;
        // Becareful of the <= or < sign when looping.
        for (int i = 1; i < _ring; ++i) {
            result += i * TileInfo.NUM_SIDES;
        }
        
        return result;
    }

    private void GenerateRing(int _ring, Vector3 _centre) {
        if (_ring < 0) {
            Debug.Log("TileSystem.GenerateRing - Invalid value for _ring!");
        }

        // The centre is a special case.
        if (_ring == 0) {
            // Create the root tile.
            TileId tileId = 0;
            Tile tile = GameObject.Instantiate(m_DefaultTile.gameObject).GetComponent<Tile>();
            tile.SetId(tileId);
            tile.transform.position = _centre;
            tile.gameObject.name = m_DefaultTile.name = tileId.ToString();

            m_TileInfos[tileId] = new TileInfo(tileId, _ring);
            m_TileInfos[tileId].m_Tile = tile;

            return;
        }

        // Create all the tiles.
        int numTiles = _ring * TileInfo.NUM_SIDES;
        TileId startId = CalculateRingStartId(_ring);
        for (int i = 0; i < numTiles; ++i) {
            TileId tileId = startId + i;
            Tile tile = GameObject.Instantiate(m_DefaultTile.gameObject).GetComponent<Tile>();
            tile.SetId(tileId);
            tile.gameObject.name = m_DefaultTile.name = tileId.ToString();

            m_TileInfos[tileId] = new TileInfo(tileId, _ring);
            m_TileInfos[tileId].m_Tile = tile;
        }

        // Assuming the distance from the centre of the hexagon to one of the vertices is 0.5 units,
        // The perpendicular distance from the centre of the hexagon to one of the edges is 0.5*sin(60) units.
        // Therefore the distance between 2 opposite edges are sin(60) units.
        // I calculated this on paper beforehand.
        float offsetDist = Mathf.Sin(60.0f * Mathf.Deg2Rad) * m_DefaultTile.transform.localScale.x;

        // This is the position of the tile at the top.
        Vector3 startPos = new Vector3(_centre.x, _centre.y, _centre.z + offsetDist * (float)_ring);
        Vector3 currentPos = startPos;

        // Position the tiles.
        // Split up the tiles into sides.
        // We will start with the tile at the very top of the ring, and go in the anti-clockwise direction.
        TileId currentId = startId;

        // The sides are the direction of the hexagon that the offset is going towards.
        int startOffsetSide = 2;
        int currentOffsetSide = startOffsetSide;
        do {
            // We add 90.0f at the end as we want 0 degrees to start upwards rather than to the right.
            float offsetAngle = ((float)currentOffsetSide / (float)TileInfo.NUM_SIDES * 360.0f + 90.0f) * Mathf.Deg2Rad;
            Vector3 offsetDir = new Vector3(Mathf.Cos(offsetAngle), 0.0f, Mathf.Sin(offsetAngle)) * offsetDist;
            
            // The number of tiles per side = _ring.
            for (uint i = 0; i < _ring; ++i) {
                // Set the tile positions.
                Tile tile = m_TileInfos[currentId].m_Tile;
                tile.transform.position = currentPos;
                
                ++currentId;
                currentPos += offsetDir;
            }

            currentOffsetSide = (currentOffsetSide + 1) % TileInfo.NUM_SIDES;
        } while (currentOffsetSide != startOffsetSide);
    }

    private int CalculateNumTileInRing(int _ring) {
        if (_ring == 0) {
            return 1;
        }

        return _ring * 6;
    }

    private void SetTileNeighbours(int _radius) {
        if (_radius <= 0) {
            return;
        }

        // Special Case for Tile 0.
        for (int side = 0; side < TileInfo.NUM_SIDES; ++side) {
            m_TileInfos[0].SetNeighbour(m_TileInfos[side + 1], side);
        }

        // Fuck it, just hardcode this damn thing.
        for (int ring = 1; ring <= _radius; ++ring) {
            TileId startId = CalculateRingStartId(ring);
            TileId currentId = startId;

            // The number of tiles for each side = ring.
            for (int j = 0; j < ring; ++j) {
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 0), 0);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 1), 1);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + 1), 2);

                if (j == ring - 1) {
                    m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId - CalculateNumTileInRing(ring - 1) - 0), 3);
                }

                ++currentId;
            }

            for (int j = 0; j < ring; ++j) {
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 1), 1);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 2), 2);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + 1), 3);

                if (j == ring - 1) {
                    m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId - CalculateNumTileInRing(ring - 1) - 1), 4);
                }

                ++currentId;
            }

            for (int j = 0; j < ring; ++j) {
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 2), 2);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 3), 3);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + 1), 4);

                if (j == ring - 1) {
                    m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId - CalculateNumTileInRing(ring - 1) - 2), 5);
                }

                ++currentId;
            }

            for (int j = 0; j < ring; ++j) {
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 3), 3);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 4), 4);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + 1), 5);

                if (j == ring - 1) {
                    m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId - CalculateNumTileInRing(ring - 1) - 3), 0);
                }

                ++currentId;
            }

            for (int j = 0; j < ring; ++j) {
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 4), 4);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 5), 5);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + 1), 0);

                if (j == ring - 1) {
                    m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId - CalculateNumTileInRing(ring - 1) - 4), 1);
                }

                ++currentId;
            }

            for (int j = 0; j < ring; ++j) {
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 5), 5);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 6), 0);

                if (j == ring - 1) {
                    m_TileInfos[currentId].SetNeighbour(GetTileInfo(startId), 1);
                    m_TileInfos[currentId].SetNeighbour(GetTileInfo(CalculateRingStartId(ring - 1)), 2);
                } else {
                    m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + 1), 1);
                    m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId - CalculateNumTileInRing(ring - 1) - 5), 2);
                }

                ++currentId;
            }
        }
    }

    public bool HasTileInfo(TileId _id) {
        if (_id < 0 || _id >= m_TileInfos.Length) {
            return false;
        }

        return m_TileInfos[_id] != null;
    }

    public TileInfo GetTileInfo(TileId _id) {
        if (HasTileInfo(_id)) {
            return m_TileInfos[_id];
        }

        return null;
    }

    public void GenerateTiles() {
        ClearTiles();

        m_TileInfos = new TileInfo[CalculateNumTiles(m_Radius)];

        for (int i = 0; i <= m_Radius; ++i) {
            GenerateRing(i, transform.position);
        }

        for (int i = 0; i < m_TileInfos.Length; ++i) {
            m_TileInfos[i].m_Tile.transform.SetParent(gameObject.transform);
        }

        SetTileNeighbours(m_Radius);
    }

    public int GetNumTiles() {
        return m_TileInfos.Length;
    }

    private void OnDestroy() {
        ClearTiles();
    }

    private void Awake() {
        m_NumTiles = m_TileInfos.Length;
    }

}