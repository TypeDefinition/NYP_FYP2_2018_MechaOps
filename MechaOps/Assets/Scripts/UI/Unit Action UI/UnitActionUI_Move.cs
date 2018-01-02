using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

/// <summary>
/// Handles how the UI for unit walking works
/// </summary>
public class UnitActionUI_Move : UnitActionUI
{
    // Non-Serialised Variable(s)
    // Unit Action
    protected UnitMoveAction m_UnitAction = null;

    // Tile System
    protected TileSystem m_TileSystem = null;
    protected List<TileId> m_ReachableTileHighlight = new List<TileId>();
    protected TileId[] m_MovementPath = new TileId[0];

    protected override void InitEvents()
    {
        base.InitEvents();
        // And then access the tile stuff from the system and get reachable tiles
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.ClickedTile), OnClickedTile);
    }

    protected override void DeinitEvents()
    {
        base.DeinitEvents();
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.ClickedTile), OnClickedTile);
    }

    protected override void Awake()
    {
        base.Awake();
        m_TileSystem = GameSystemsDirectory.GetSceneInstance().GetTileSystem();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_TileSystem.ClearPathMarkers();
    }

    /// <summary>
    /// When player pressed the confirm button at the UI
    /// </summary>
    public override void PressedConfirm()
    {
        // then the unit will walk that path! if the path exists
        if (m_MovementPath.Length == 0)
        {
            Debug.Log("Player is trying to walk no path");
        }
        else
        {
            m_UnitAction.SetTilePath(m_MovementPath);
            m_UnitAction.TurnOn();
            Destroy(gameObject);
        }
    }

    // Callbacks
    /// <summary>
    /// Called when the Player unit manager triggers this event!
    /// </summary>
    /// <param name="_action">The unit action that is supposed to pass in!</param>
    protected override void SetUnitAction(IUnitAction _action)
    {
        m_UnitAction = (UnitMoveAction)_action;

        TileId[] reachableTiles = m_TileSystem.GetReachableTiles(m_UnitAction.MovementPoints, m_UnitAction.GetUnitStats().CurrentTileID, m_UnitAction.GetUnitStats().GetTileAttributeOverrides());
        m_ReachableTileHighlight = new List<TileId>(reachableTiles);
        if (m_ReachableTileHighlight.Contains(m_UnitAction.GetUnitStats().CurrentTileID))
        {
            m_ReachableTileHighlight.Remove(m_UnitAction.GetUnitStats().CurrentTileID);
        }
        m_TileSystem.SetPathMarkers(m_ReachableTileHighlight.ToArray(), null);

        UpdateActionInfo(_action);
    }

    void OnClickedTile(GameObject _clickedTile)
    {
        if (_clickedTile.tag != "TileBase") { return; }

        Tile tileComponent = _clickedTile.GetComponent<Tile>();
        Assert.IsNotNull(tileComponent);

        if (m_ReachableTileHighlight.Contains(tileComponent.GetTileId()))
        {
            // Then we have to find the path for it!
            m_MovementPath = m_TileSystem.GetPath(m_UnitAction.MovementPoints, m_UnitAction.GetUnitStats().CurrentTileID, tileComponent.GetTileId(), m_UnitAction.GetUnitStats().GetTileAttributeOverrides());
            m_TileSystem.SetPathMarkers(m_ReachableTileHighlight.ToArray(), m_MovementPath);
        }
    }
}