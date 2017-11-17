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

/*
[System.Serializable]
public class TileInfo
{

    [SerializeField] private TileId m_Id;
    [SerializeField] private Tile m_Tile;

    public TileInfo(TileId _id, Tile _tile)
    {
        m_Id = _id;
        m_Tile = _tile;
    }

    ~TileInfo() {}

    public TileId GetId()
    {
        return m_Id;
    }

    public Tile GetTile()
    {
        return m_Tile;
    }

}
*/

[System.Serializable]
public class TileDictionaryPair
{

    public TileId m_Id = null;
    public Tile m_Tile = null;

    public TileDictionaryPair(TileId _id, Tile _info)
    {
        m_Id = _id;
        m_Tile = _info;
    }

}

[DisallowMultipleComponent]
public class TileSystem : MonoBehaviour
{

    private class SearchNode
    {
        private TileId m_Id;
        private int m_GCost; // From start to here.
        private int m_HCost; // From here to the end.
        private SearchNode m_Parent;

        public SearchNode(TileId _id, int _gCost, int _hCost, SearchNode _parent)
        {
            m_Id = _id;
            m_GCost = _gCost;
            m_HCost = _hCost;
            m_Parent = _parent;
        }

        public TileId Id { get { return m_Id; } }

        // From start to here.
        public int GCost { get { return m_GCost; } set { m_GCost = value; } }

        // From here to the end.
        public int HCost { get { return m_HCost; } set { m_HCost = value; } }

        // Total Cost.
        public int FCost { get { return m_GCost + m_HCost; } }

        public SearchNode Parent { get { return m_Parent; } set { m_Parent = value; } }

    }

    [SerializeField] private HazardLibrary m_HazardLibrary = null;
    [SerializeField] private TileLibrary m_TileLibrary = null;
    [SerializeField] private Tile m_DefaultTile = null;

    private Dictionary<TileId, Tile> m_TileDictionary = new Dictionary<TileId, Tile>();
    [HideInInspector, SerializeField] private TileDictionaryPair[] m_TileArray = new TileDictionaryPair[0];

    [SerializeField] private int m_Radius = 10;
    [SerializeField] private float m_DistanceBetweenTiles = 5.0f;

    [SerializeField] private GameObject m_TilePath = null;
    [SerializeField] private GameObject m_TileSelected = null;
    [SerializeField] private GameObject m_TileReachable = null;
    private List<GameObject> m_PathMarkers = null;

    public int Radius
    {
        get { return m_Radius; }
    }

    public float DistanceBetweenTiles
    {
        get { return m_DistanceBetweenTiles; }
    }

    public HazardLibrary GetHazardLibrary()
    {
        return m_HazardLibrary;
    }

    public TileLibrary GetTileLibrary()
    {
        return m_TileLibrary;
    }

    public Tile GetDefaultTile()
    {
        return m_DefaultTile;
    }

#if UNITY_EDITOR
    public void GenerateTiles()
    {
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
        for (int x = -m_Radius; x <= m_Radius; ++x)
        {
            for (int y = Mathf.Max(-m_Radius, -m_Radius - x); y <= Mathf.Min(m_Radius, m_Radius - x); ++y)
            {
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
                tile.transform.localScale = tile.transform.localScale;

                tile.LoadTileType();

                m_TileDictionary.Add(tileId, tile);
            }
        }

        // After the tiles have been generated, we need to store it in m_TileArray.
        // This is because m_TileDictionary cannot be serialized, and it will be reseted when the game plays.
        // As a workaround, we will store the info in m_TileArray for permanent storage,
        // and copy it into m_TileDictionary when needed.
        {
            m_TileArray = new TileDictionaryPair[m_TileDictionary.Count];
            int index = 0;
            foreach (KeyValuePair<TileId, Tile> iter in m_TileDictionary)
            {
                m_TileArray[index++] = new TileDictionaryPair(iter.Key, iter.Value);
            }
        }
    }

    public void ClearTiles()
    {
        LoadDictionary();

        if (m_TileDictionary.Count == 0) {
            //Debug.Log(MethodBase.GetCurrentMethod().Name + " - No tiles to clear!");
            return;
        }

        // Destroy any existing tiles.
        foreach (Tile value in m_TileDictionary.Values)
        {
            if (value != null)
            {
                GameObject.DestroyImmediate(value.gameObject);
            }
        }

        m_TileArray = new TileDictionaryPair[0];
        m_TileDictionary.Clear();
    }

    public void RandomizeTileTypes()
    {
        LoadDictionary();

        foreach (KeyValuePair<TileId, Tile> iter in m_TileDictionary)
        {
            TileType tileType = (TileType)Random.Range(0, (int)TileType.Num_TileType - 1);
            iter.Value.SetTileType(tileType);
        }
    }

    public void RandomizeHazardTypes()
    {
        LoadDictionary();

        foreach (KeyValuePair<TileId, Tile> iter in m_TileDictionary)
        {
            //HazardType hazardType = (HazardType)Random.Range((int)HazardType.None, (int)HazardType.Num_HazardType - 1);
            HazardType hazardType = (HazardType)Random.Range(0, 1);
            Debug.Log((int)hazardType);
            iter.Value.SetHazardType(hazardType);
        }
    }

    private void OnValidate()
    {
        m_Radius = Mathf.Max(0, m_Radius);
        m_DistanceBetweenTiles = Mathf.Max(0.0f, m_DistanceBetweenTiles);
    }
#endif // UNITY_EDITOR

    public void LoadTileAndHazardTypes()
    {
        LoadDictionary();

        if (m_TileDictionary.Count == 0)
        {
            Debug.Log(MethodBase.GetCurrentMethod().Name + " - No tiles to load!");
            return;
        }

        foreach (Tile value in m_TileDictionary.Values)
        {
            if (value != null)
            {
                value.LoadTileType();
                value.LoadHazardType();
            }
        }

    }

    private void LoadDictionary()
    {
        m_TileDictionary.Clear();
        for (int i = 0; i < m_TileArray.Length; ++i)
        {
            m_TileDictionary.Add(m_TileArray[i].m_Id, m_TileArray[i].m_Tile);
        }
    }

    // Get our search area so that we do not have to search the whole map.
    private HashSet<TileId> GetSearchArea(TileId _start, int _movementPoints)
    {
        HashSet<TileId> searchArea = new HashSet<TileId>();
        {
            TileId[] surroundingTiles = GetSurroundingTiles(_start, _movementPoints);
            for (int i = 0; i < surroundingTiles.Length; ++i)
            {
                searchArea.Add(surroundingTiles[i]);
            }
        }

        return searchArea;
    }

    // We add our overrides into a dictionary for easy lookup.
    private Dictionary<TileType, TileAttributeOverride> GenerateOverrideDictionary(TileAttributeOverride[] _overrides)
    {
        Dictionary<TileType, TileAttributeOverride> overrideDictionary = new Dictionary<TileType, TileAttributeOverride>();
        if (_overrides != null)
        {
            for (int i = 0; i < _overrides.Length; ++i)
            {
                overrideDictionary.Add(_overrides[i].Type, _overrides[i]);
            }
        }

        return overrideDictionary;
    }

    private SearchNode GetCheapestNode(List<SearchNode> _list)
    {
        if (_list == null || _list.Count == 0)
        {
            return null;
        }

        SearchNode cheapestNode = _list[0];
        for (int i = 1; i < _list.Count; ++i)
        {
            if (_list[i].FCost < cheapestNode.FCost)
            {
                cheapestNode = _list[i];
            }
        }

        return cheapestNode;
    }

    private void Awake()
    {
        LoadDictionary();
        // LoadTileAndHazardTypes();
    }

    // Interface Function(s)
    public TileId[] GetSurroundingTiles(TileId _centre, int _radius)
    {
        _radius = Mathf.Min(_radius, m_Radius * 2);

        // _radius should never be < 0.
        Assert.IsFalse(_radius < 0, MethodBase.GetCurrentMethod().Name + " - Invalid value for _radius!");

        List<TileId> result = new List<TileId>();

        for (int x = -_radius; x <= _radius; ++x)
        {
            for (int y = Mathf.Max(-_radius, -_radius - x); y <= Mathf.Min(_radius, _radius - x); ++y)
            {
                TileId tileId = new TileId(x + _centre.GetX(), y + _centre.GetY());

                // Check that the tile exist (in case of tiles near/at the edge of the map).
                if (m_TileDictionary.ContainsKey(tileId))
                {
                    result.Add(tileId);
                }
            }
        }

        return result.ToArray();
    }
    
    public TileId[] GetReachableTiles(int _movementPoints, TileId _start, TileAttributeOverride[] _overrides)
    {
        if (_movementPoints <= 0)
        {
            return null;
        }

        // Get our search area so that we do not have to search the whole map.
        HashSet<TileId> searchArea = GetSearchArea(_start, _movementPoints);
        // We add our overrides into a dictionary for easy lookup.
        Dictionary<TileType, TileAttributeOverride> overrideDictionary = GenerateOverrideDictionary(_overrides);

        HashSet<TileId> walkableList = new HashSet<TileId>();
        HashSet<TileId> unwalkableList = new HashSet<TileId>();

        // Search through each tile to check if they are reachable.
        foreach (TileId end in searchArea)
        {
            // If we have already determined the reachability of this tile, continue.
            if (walkableList.Contains(end) || unwalkableList.Contains(end))
            {
                continue;
            }

            int minDistance = TileId.GetDistance(_start, end);
            List<SearchNode> openList = new List<SearchNode>();
            List<SearchNode> closedList = new List<SearchNode>();
            openList.Add(new SearchNode(_start, 0, minDistance, null));

            while (openList.Count != 0)
            {
                SearchNode cheapestNode = GetCheapestNode(openList);

                // 惨了，我们没办法达到目的地。
                if (cheapestNode.FCost > _movementPoints)
                {
                    unwalkableList.Add(cheapestNode.Id);
                    break;
                }

                // 我们肯定能来到这个瓦。
                walkableList.Add(cheapestNode.Id);

                // 到达了目的地吗？
                if (cheapestNode.Id.Equals(end))
                {
                    break;
                }

                TileId[] neighbors = cheapestNode.Id.GetNeighbors();
                for (int i = 0; i < neighbors.Length; ++i)
                {
                    TileId neighborId = neighbors[i];

                    // Ensure that the tile is within our search area.
                    if (searchArea.Contains(neighborId) == false)
                    {
                        // Debug.Log(MethodBase.GetCurrentMethod().Name + " - Tile " + neighborId.ToString() + " is not in search area.");
                        continue;
                    }

                    // Don't bother searching if we already know that it is unwalkable.
                    if (unwalkableList.Contains(neighborId))
                    {
                        continue;
                    }

                    Tile tile = GetTile(neighborId);
                    TileAttributeOverride attributeOverride;
                    overrideDictionary.TryGetValue(tile.GetTileType(), out attributeOverride);

                    // Ensure that the tile is walkable.
                    bool walkable = tile.GetIsWalkable();
                    int hCost = TileId.GetDistance(neighborId, end);
                    int gCost = cheapestNode.GCost;
                    if (attributeOverride == null)
                    {
                        gCost += tile.GetTotalMovementCost();
                    }
                    else
                    {
                        walkable = attributeOverride.Walkable;
                        gCost += attributeOverride.MovementCost;
                        Hazard hazard = tile.GetHazard();
                        if (hazard != null) { gCost += hazard.Attributes.MovementCost; }
                    }

                    if (walkable == false)
                    {
                        if (unwalkableList.Contains(cheapestNode.Id) == false)
                        {
                            unwalkableList.Add(cheapestNode.Id);
                        }
                        continue;
                    }

                    // 目的地に到達した？
                    if (neighborId.Equals(end))
                    {
                        closedList.Add(cheapestNode);

                        SearchNode endNode = new SearchNode(neighborId, gCost, hCost, cheapestNode);
                        openList.Clear(); // We found the endNode. There's no need for the rest.
                        openList.Add(endNode);
                        break;
                    }

                    // Check that it is not already in closedList.
                    bool inClosedList = false;
                    for (int j = 0; j < closedList.Count; ++j)
                    {
                        if (closedList[j].Id.Equals(neighborId))
                        {
                            inClosedList = true;
                            break;
                        }
                    }
                    if (inClosedList)
                    {
                        continue;
                    }

                    SearchNode neighbourNode = null;
                    // See if it is already in the openList.
                    for (int j = 0; j < openList.Count; ++j)
                    {
                        if (openList[j].Id.Equals(neighborId))
                        {
                            neighbourNode = openList[j];
                            if (gCost < neighbourNode.GCost)
                            {
                                neighbourNode.GCost = gCost;
                                neighbourNode.Parent = cheapestNode;
                            }
                            break;
                        }
                    }

                    // If it isn't, add it to the openList.
                    if (neighbourNode == null)
                    {
                        neighbourNode = new SearchNode(neighborId, gCost, hCost, cheapestNode);
                        openList.Add(neighbourNode);
                    }
                }

                openList.Remove(cheapestNode);
                closedList.Add(cheapestNode);
            }
        }

        if (walkableList.Count == 0)
        {
            return null;
        }

        TileId[] result = new TileId[walkableList.Count];
        int iteration = 0;
        foreach (TileId tileId in walkableList)
        {
            result[iteration++] = tileId;
        }

        return result;
    }

    public TileId[] GetPath(int _movementPoints, TileId _start, TileId _end, TileAttributeOverride[] _overrides)
    {
        // 查看我们是不是已经到达了目的地。
        if (_start.Equals(_end))
        {
            Debug.Log(MethodBase.GetCurrentMethod().Name + " - Start == End.");
            return null;
        }

        // 查看到底有没有达到目的地的这个可能。
        int minDistance = TileId.GetDistance(_start, _end);
        if (minDistance > _movementPoints)
        {
            Debug.Log(MethodBase.GetCurrentMethod().Name + " - Insufficient Movement Points.");
            return null;
        }

        // Check that both tiles are valid.
        if (GetTile(_start) == null || GetTile(_end) == null)
        {
            Debug.Log(MethodBase.GetCurrentMethod().Name + " - Invalid Tile.");
            return null;
        }

        // Get our search area so that we do not have to search the whole map.
        HashSet<TileId> searchArea = GetSearchArea(_start, _movementPoints);
        // We add our overrides into a dictionary for easy lookup.
        Dictionary<TileType, TileAttributeOverride> overrideDictionary = GenerateOverrideDictionary(_overrides);

        List<SearchNode> openList = new List<SearchNode>(); // Tiles that are waiting to be checked.
        List<SearchNode> closedList = new List<SearchNode>(); // Tiles that have already been checked.
        openList.Add(new SearchNode(_start, 0, minDistance, null));

        // Check the openList until we find our path.
        // If the openList is empty, it means that there is no path.
        while (openList.Count != 0)
        {
            // We will always go for the cheapest path.
            SearchNode cheapestNode = GetCheapestNode(openList);

            // 惨了，我们没办法达到目的地。
            if (cheapestNode.FCost > _movementPoints)
            {
                Debug.Log(MethodBase.GetCurrentMethod().Name + " - Insufficient Speed. Needed: " + cheapestNode.FCost.ToString() + "(Or More) Have: " + _movementPoints.ToString());
                return null;
            }

            // 到达了目的地吗？
            if (cheapestNode.Id.Equals(_end))
            {
                List<TileId> result = new List<TileId>();
                SearchNode node = cheapestNode;
                while (node != null)
                {
                    result.Add(node.Id);
                    node = node.Parent;
                }
                result.Reverse();

                return result.ToArray();
            }

            // Not there yet? Okies let's check our neighbours.
            TileId[] neighbors = cheapestNode.Id.GetNeighbors();
            for (int i = 0; i < neighbors.Length; ++i)
            {
                TileId neighborId = neighbors[i];

                // Ensure that the tile is within our search area.
                if (searchArea.Contains(neighborId) == false)
                {
                    // Debug.Log(MethodBase.GetCurrentMethod().Name + " - Tile " + neighborId.ToString() + " is not in search area.");
                    continue;
                }

                Tile tile = GetTile(neighborId);
                TileAttributeOverride attributeOverride;
                overrideDictionary.TryGetValue(tile.GetTileType(), out attributeOverride);

                // Ensure that the tile is walkable.
                bool walkable = tile.GetIsWalkable();
                int hCost = TileId.GetDistance(neighborId, _end);
                int gCost = cheapestNode.GCost;
                if (attributeOverride == null)
                {
                    gCost += tile.GetTotalMovementCost();
                }
                else
                {
                    walkable = attributeOverride.Walkable;
                    gCost += attributeOverride.MovementCost;
                    Hazard hazard = tile.GetHazard();
                    if (hazard != null) { gCost += hazard.Attributes.MovementCost; }
                }

                if (walkable == false)
                {
                    continue;
                }

                // 目的地に到達した？
                if (neighborId.Equals(_end))
                {
                    closedList.Add(cheapestNode);

                    SearchNode endNode = new SearchNode(neighborId, gCost, hCost, cheapestNode);
                    openList.Clear(); // We found the endNode. There's no need for the rest.
                    openList.Add(endNode);
                    break;
                }

                // Check that it is not already in closedList.
                bool inClosedList = false;
                for (int j = 0; j < closedList.Count; ++j)
                {
                    if (closedList[j].Id.Equals(neighborId))
                    {
                        inClosedList = true;
                        break;
                    }
                }
                if (inClosedList)
                {
                    continue;
                }

                SearchNode neighbourNode = null;
                // See if it is already in the openList.
                for (int j = 0; j < openList.Count; ++j)
                {
                    if (openList[j].Id.Equals(neighborId))
                    {
                        neighbourNode = openList[j];
                        if (gCost < neighbourNode.GCost)
                        {
                            neighbourNode.GCost = gCost;
                            neighbourNode.Parent = cheapestNode;
                        }
                        break;
                    }
                }

                // If it isn't, add it to the openList.
                if (neighbourNode == null)
                {
                    neighbourNode = new SearchNode(neighborId, gCost, hCost, cheapestNode);
                    openList.Add(neighbourNode);
                }
            }

            openList.Remove(cheapestNode);
            closedList.Add(cheapestNode);
        }

        Debug.Log(MethodBase.GetCurrentMethod().Name + " - No path found.");
        return null;
    }

    public int GetNumTiles()
    {
        LoadDictionary();

        return m_TileDictionary.Count;
    }

    public Tile GetTile(TileId _id)
    {
        return m_TileDictionary.ContainsKey(_id) ? m_TileDictionary[_id] : null;
    }

    public void ClearPathMarkers()
    {
        if (m_PathMarkers == null)
        {
            return;
        }

        for (int i = 0; i < m_PathMarkers.Count; ++i)
        {
            if (m_PathMarkers[i] == null)
            {
                continue;
            }

            GameObject.Destroy(m_PathMarkers[i]);
        }
        m_PathMarkers = null;
    }

    // Pass in null if you do not want to render the reachable tiles or path.
    public void SetPathMarkers(TileId[] _reachableTiles, TileId[] _path)
    {
        ClearPathMarkers();

        if (_reachableTiles == null && _path == null)
        {
            return;
        }

        m_PathMarkers = new List<GameObject>();
        HashSet<TileId> pathTiles = new HashSet<TileId>();
        
        if (_path != null)
        {
            LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
            Assert.IsTrue(lineRenderer != null, MethodBase.GetCurrentMethod().Name + " - LineRenderer required to work!");
            List<Vector3> linePositions = new List<Vector3>();

            for (int i = 0; i < _path.Length; ++i)
            {
                Tile tile = GetTile(_path[i]);
                Assert.IsTrue(tile != null, MethodBase.GetCurrentMethod().Name + " - Invalid path given!");

                GameObject pathMarker = GameObject.Instantiate((i == (_path.Length - 1) ? m_TileSelected : m_TilePath));
                pathMarker.transform.position = tile.gameObject.transform.position;
                pathMarker.transform.parent = transform;
                m_PathMarkers.Add(pathMarker);
                pathTiles.Add(_path[i]);

                linePositions.Add(tile.gameObject.transform.position);
            }
            lineRenderer.positionCount = linePositions.Count;
            lineRenderer.SetPositions(linePositions.ToArray());
        }

        if (_reachableTiles != null)
        {
            for (int i = 0; i < _reachableTiles.Length; ++i)
            {
                Tile tile = GetTile(_reachableTiles[i]);
                Assert.IsTrue(tile != null, MethodBase.GetCurrentMethod().Name + " - Invalid reachable tiles given!");

                if (pathTiles.Contains(_reachableTiles[i]))
                {
                    continue;
                }

                GameObject pathMarker = GameObject.Instantiate(m_TileReachable);
                pathMarker.transform.position = tile.gameObject.transform.position;
                pathMarker.transform.parent = transform;
                m_PathMarkers.Add(pathMarker);
            }
        }
    }
}