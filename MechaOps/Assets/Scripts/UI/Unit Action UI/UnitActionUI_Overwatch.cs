using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionUI_Overwatch : UnitActionUI
{
    protected UnitOverwatchAction m_UnitAction;
    protected TileSystem m_TileSystem = null;

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

    public override void PressedConfirm()
    {
        m_UnitAction.TurnOn();
        GameEventSystem.GetInstance().TriggerEvent(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitStartAction));
        Destroy(gameObject);
    }

    protected override void SetUnitAction(IUnitAction _action)
    {
        m_UnitAction = (UnitOverwatchAction)_action;

        // Get the tiles that we can see.
        List<Tile> viewedTiles = m_UnitAction.GetUnitStats().GetViewScript().GetViewedTiles();
        HashSet<TileId> viewedTilesIds = new HashSet<TileId>();
        foreach (Tile tile in viewedTiles)
        {
            viewedTilesIds.Add(tile.GetTileId());
        }

        // Highlight the tiles within range.
        TileId[] tilesInRange = m_TileSystem.GetSurroundingTiles(m_UnitAction.GetUnitStats().CurrentTileID, m_UnitAction.MaxAttackRange);
        List<TileId> attackableTiles = new List<TileId>();
        foreach (TileId tileId in tilesInRange)
        {
            int distanceToTile = TileId.GetDistance(m_UnitAction.GetUnitStats().CurrentTileID, tileId);
            // Check if it is within the range
            if (distanceToTile >= m_UnitAction.MinAttackRange && distanceToTile <= m_UnitAction.MaxAttackRange)
            {
                if (viewedTilesIds.Contains(tileId))
                {
                    attackableTiles.Add(tileId);
                }
            }
        }
        m_TileSystem.SetPathMarkers(attackableTiles.ToArray(), null);

        UpdateActionInfo(_action);
    }

    protected override void UpdateActionInfo(IUnitAction _action)
    {
        UnitAttackAction attackAction = (UnitAttackAction)_action;

        m_ActionNameText.text = _action.UnitActionName;
        string actionCostText = string.Format("Action Cost: {0}", _action.ActionCost);
        string endsTurnText = _action.EndsTurn ? "Ends Turn: Yes" : "Ends Turn: No";
        string damageText = string.Format("Damage: {0}", attackAction.DamagePoints);

        m_ActionDescriptionText.text = actionCostText + " " + endsTurnText + "\n" + damageText + "\n" + _action.UnitActionDescription;
    }
}