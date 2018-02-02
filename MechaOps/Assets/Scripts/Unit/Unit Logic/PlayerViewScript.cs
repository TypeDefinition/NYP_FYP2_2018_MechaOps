using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// This will be used to render the tiles of this unit surrounding!
/// </summary>
[DisallowMultipleComponent]
public class PlayerViewScript : ViewScript
{
    protected List<Tile> m_ViewedTiles = new List<Tile>();

    protected override void InitEvents()
    {
        base.InitEvents();
        GameEventSystem.GetInstance().SubscribeToEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitMovedToTile), OnUnitMovedToTile);
        GameEventSystem.GetInstance().SubscribeToEvent<UnitStats, bool>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitDead), OnUnitDead);
    }

    protected override void DeinitEvents()
    {
        base.DeinitEvents();
        GameEventSystem.GetInstance().UnsubscribeFromEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitMovedToTile), OnUnitMovedToTile);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<UnitStats, bool>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitDead), OnUnitDead);
    }

    // Callbacks
    protected override void OnUnitsSpawned()
    {
        CheckVisibleTiles();
    }

    protected virtual void OnUnitMovedToTile(UnitStats _movedUnit)
    {
        if (_movedUnit == m_UnitStats)
        {
            CheckVisibleTiles();
        }
    }

    protected virtual void OnUnitDead(UnitStats _deadUnit, bool _deadUnitVisible)
    {
        if (_deadUnit == m_UnitStats)
        {
            ClearVisibleTiles();
        }
    }

    /// <summary>
    /// This will help to render the tiles surrounding of this unit!
    /// </summary>
    protected virtual void CheckVisibleTiles()
    {
        ClearVisibleTiles();

        TileId[] surroundingTiles = m_TileSystem.GetSurroundingTiles(m_UnitStats.CurrentTileID, m_UnitStats.ViewRange);
        // And then iterate though the list and increase the tile range
        foreach (TileId tileId in surroundingTiles)
        {
            if (TileId.GetDistance(m_UnitStats.CurrentTileID, tileId) > m_UnitStats.ViewRange)
            {
                continue;
            }

            Tile tile = m_TileSystem.GetTile(tileId);
            // then raycast to that tile to see if it works and get the id
            if (RaycastToTile(tile))
            {
                m_ViewedTiles.Add(tile);
                ++tile.VisibleCounter;
            }
        }
    }

    /// <summary>
    /// Decrease the visibility counter of the tiles that are out of range.
    /// </summary>
    protected virtual void ClearVisibleTiles()
    {
        foreach (Tile tile in m_ViewedTiles)
        {
            --tile.VisibleCounter;
        }
        m_ViewedTiles.Clear();
    }

    /// <summary>
    /// Increase the counter of how many units can see us.
    /// </summary>
    public override void IncreaseVisibility()
    {
        ++m_VisibilityCount;
        Assert.IsTrue(m_VisibilityCount >= 0);
        if (m_VisibilityCount == 1)
        {
            GameEventSystem.GetInstance().TriggerEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitSeen), m_UnitStats);
        }
    }

    /// <summary>
    /// Decrease the counter of how many units can see us.
    /// </summary>
    public override void DecreaseVisibility()
    {
        --m_VisibilityCount;
        Assert.IsTrue(m_VisibilityCount >= 0);
        if (m_VisibilityCount == 0)
        {
            GameEventSystem.GetInstance().TriggerEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitUnseen), m_UnitStats);
        }
    }

    /// <summary>
    /// It is always visible to the player
    /// </summary>
    /// <returns></returns>
    public override bool IsVisibleToPlayer()
    {
        return true;
    }
}
