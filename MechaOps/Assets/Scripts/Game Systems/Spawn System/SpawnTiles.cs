using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileList
{
    [SerializeField] private TileId[] m_List = null;

    public TileId[] GetList() { return m_List; }
}

[System.Serializable, CreateAssetMenu(fileName = "SpawnTiles", menuName = "Spawn System/Spawn Tiles")]
public class SpawnTiles : ScriptableObject
{
    [SerializeField] private TileList[] m_FactionTileLists = null;

    public TileList GetFactionTileList(FactionType _factionType)
    {
        return m_FactionTileLists[(int)_factionType];
    }

    private void OnValidate()
    {
        if (m_FactionTileLists == null)
        {
            m_FactionTileLists = new TileList[(int)FactionType.Num_FactionType];
        }

        if (m_FactionTileLists.Length != (int)FactionType.Num_FactionType)
        {
            TileList[] currentTileList = m_FactionTileLists;
            m_FactionTileLists = new TileList[(int)FactionType.Num_FactionType];
            for (int i = 0; i < currentTileList.Length && i < m_FactionTileLists.Length; ++i)
            {
                m_FactionTileLists[i] = currentTileList[i];
            }
        }
    }
}