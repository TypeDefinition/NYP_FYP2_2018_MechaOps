using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Reflection;

/*
The tiles are hexagons.
Their sides are labels from 0 to 6, with 0 being the top, and increasing in the anti-clockwise direction.
*/

using TileId = System.Int32;

[System.Serializable]
public class TileInfo {

    public TileId m_Id;
    public int m_Ring;
    public Tile m_Tile;
    public TileId[] m_Neighbours;

    public TileInfo(TileId _id, int _ring) {
        m_Id = _id;
        m_Ring = _ring;
        m_Tile = null;
        m_Neighbours = new TileId[6];
        for (int i = 0; i < m_Neighbours.Length; ++i) {
            m_Neighbours[i] = Tile.INVALID_ID;
        }
    }

    ~TileInfo() {}

    public void SetNeighbour(TileInfo _neighbour, uint _index) {
        if (_neighbour == null) {
            m_Neighbours[_index] = Tile.INVALID_ID;
            m_Tile.SetNeighbour(null, _index);
        } else {
            m_Neighbours[_index] = _neighbour.m_Id;
            _neighbour.m_Neighbours[(_index + 3) % 6] = this.m_Id;

            m_Tile.SetNeighbour(_neighbour.m_Tile, _index);
        }
    }

}

[DisallowMultipleComponent]
public class TileSystem : MonoBehaviour {

    [Range(0, 5000)]
    public int m_Radius = 20;
    public Tile m_DefaultTile;
    [SerializeField]
    private TileInfo[] m_TileInfos;

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

    // This function calculates how many tiles we need to generate based on the radius.
    private int CalculateNumTiles(int _radius) {
        // _radius should never be < 0.
        Assert.IsFalse(_radius < 0, MethodBase.GetCurrentMethod().Name + " -Invalid value for _radius!");

        // We start from 1 because if the radius is 0, there is still 1 tile.
        // Becareful of the <= or < sign when looping.
        int result = 1;
        for (int i = 1; i <= _radius; ++i) {
            result += i * 6;
        }

        return result;
    }
	
    private TileId CalculateRingStartId(int _ring) {
        // _ring should never be < 0.
        Assert.IsFalse(_ring < 0, MethodBase.GetCurrentMethod().Name + " - Invalid value for _ring!");

        // Special case for 0.
        if (_ring == 0) {
            return 0;
        }

        // We have to add 1 to the result, or we will get the last id of the previous ring.
        TileId result = 1;
        // Becareful of the <= or < sign when looping.
        for (int i = 1; i < _ring; ++i) {
            result += i * 6;
        }
        
        return result;
    }

    private void GenerateRing(int _ring, Vector3 _centre) {
        // _ring should never be < 0.
        Assert.IsFalse(_ring < 0, MethodBase.GetCurrentMethod().Name + " - Invalid value for _ring!");

        // The centre is a special case.
        if (_ring == 0) {
            // Create the root tile.
            TileId tileId = 0;
            Tile tile = GameObject.Instantiate(m_DefaultTile.gameObject).GetComponent<Tile>();
            tile.SetId(tileId);
            tile.transform.position = _centre;

            m_TileInfos[tileId] = new TileInfo(tileId, _ring);
            m_TileInfos[tileId].m_Tile = tile;

            return;
        }

        // Create all the tiles.
        int numTiles = _ring * 6;
        TileId startId = CalculateRingStartId(_ring);
        for (int i = 0; i < numTiles; ++i) {
            TileId tileId = startId + i;
            Tile tile = GameObject.Instantiate(m_DefaultTile.gameObject).GetComponent<Tile>();
            tile.SetId(tileId);

            m_TileInfos[tileId] = new TileInfo(tileId, _ring);
            m_TileInfos[tileId].m_Tile = tile;
        }

        // Assuming the distance from the centre of the hexagon to one of the vertices is 0.5 units,
        // The perpendicular distance from the centre of the hexagon to one of the edges is 0.5*sin(60) units.
        // Therefore the distance between 2 opposite edges are sin(60) units.
        // I calculated this on paper beforehand.
        // We assume that the tile's X and Z scale are equal.
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
            float offsetAngle = ((float)currentOffsetSide/6.0f * 360.0f + 90.0f) * Mathf.Deg2Rad;
            Vector3 offsetDir = new Vector3(Mathf.Cos(offsetAngle), 0.0f, Mathf.Sin(offsetAngle)) * offsetDist;
            
            // The number of tiles per side = _ring.
            for (uint i = 0; i < _ring; ++i) {
                // Set the tile positions.
                Tile tile = m_TileInfos[currentId].m_Tile;
                tile.transform.position = currentPos;
                
                ++currentId;
                currentPos += offsetDir;
            }

            currentOffsetSide = (currentOffsetSide + 1) % 6;
        } while (currentOffsetSide != startOffsetSide);
    }

    private int CalculateNumTileInRing(int _ring) {
        // _ring should never be < 0.
        Assert.IsFalse(_ring < 0, MethodBase.GetCurrentMethod().Name + " - Invalid value for _ring!");

        if (_ring == 0) {
            return 1;
        }

        return _ring*6;
    }

    private void SetTileNeighbours(int _radius) {
        if (_radius <= 0) {
            return;
        }

        // Special Case for Tile 0.
        for (uint side = 0; side < 6; ++side) {
            m_TileInfos[0].SetNeighbour(m_TileInfos[side + 1], side);
        }

        // Fuck it, just hardcode this damn thing.
        for (int ring = 1; ring <= _radius; ++ring) {
            // The number of tiles for each side = ring.
            TileId startId = CalculateRingStartId(ring);
            TileId currentId = startId;

            // Side 0 (Top)
            for (int j = 0; j < ring; ++j) {
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 0), 0);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 1), 1);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + 1), 2);

                if (j == ring - 1) {
                    m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId - CalculateNumTileInRing(ring - 1) - 0), 3);
                }

                ++currentId;
            }

            // Side 1 (Top Left)
            for (int j = 0; j < ring; ++j) {
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 1), 1);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 2), 2);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + 1), 3);

                if (j == ring - 1) {
                    m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId - CalculateNumTileInRing(ring - 1) - 1), 4);
                }

                ++currentId;
            }

            // Side 2 (Bottom Left)
            for (int j = 0; j < ring; ++j) {
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 2), 2);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 3), 3);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + 1), 4);

                if (j == ring - 1) {
                    m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId - CalculateNumTileInRing(ring - 1) - 2), 5);
                }

                ++currentId;
            }

            // Side 2 (Bottom)
            for (int j = 0; j < ring; ++j) {
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 3), 3);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 4), 4);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + 1), 5);

                if (j == ring - 1) {
                    m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId - CalculateNumTileInRing(ring - 1) - 3), 0);
                }

                ++currentId;
            }

            // Side 3 (Bottom Right)
            for (int j = 0; j < ring; ++j) {
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 4), 4);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + ring * 6 + 5), 5);
                m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId + 1), 0);

                if (j == ring - 1) {
                    m_TileInfos[currentId].SetNeighbour(GetTileInfo(currentId - CalculateNumTileInRing(ring - 1) - 4), 1);
                }

                ++currentId;
            }

            // Side 4 (Top Right)
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
        return (_id < 0 || _id >= m_TileInfos.Length) ? false : (m_TileInfos[_id] != null);
    }

    public TileInfo GetTileInfo(TileId _id) {
        return HasTileInfo(_id) ? m_TileInfos[_id] : null;
    }

    public void GenerateTiles() {
        // Delete existing tiles.
        ClearTiles();

        // Make the array the necessary size.
        m_TileInfos = new TileInfo[CalculateNumTiles(m_Radius)];

        // Create the tiles.
        for (int i = 0; i <= m_Radius; ++i) {
            GenerateRing(i, transform.position);
        }

        // Calculate the adjacent tiles for each tiles.
        SetTileNeighbours(m_Radius);

        for (int i = 0; i < m_TileInfos.Length; ++i) {
            m_TileInfos[i].m_Tile.gameObject.name = m_DefaultTile.name + " " + i.ToString();
            m_TileInfos[i].m_Tile.transform.SetParent(gameObject.transform);
            m_TileInfos[i].m_Tile.LoadType();
        }
    }

    public int GetNumTiles() {
        return m_TileInfos.Length;
    }

    private void OnDestroy() {
    }

}