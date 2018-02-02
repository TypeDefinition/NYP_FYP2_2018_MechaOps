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

        // Highlight the tiles within range.
        TileId[] tilesInRange = m_TileSystem.GetSurroundingTiles(m_UnitAction.GetUnitStats().CurrentTileID, m_UnitAction.MaxAttackRange);
        List<TileId> attackableTiles = new List<TileId>();
        foreach (TileId tileId in tilesInRange)
        {
            int distanceToTile = TileId.GetDistance(m_UnitAction.GetUnitStats().CurrentTileID, tileId);
            // Check if it is within the range
            if (distanceToTile >= m_UnitAction.MinAttackRange && distanceToTile <= m_UnitAction.MaxAttackRange)
            {
                attackableTiles.Add(tileId);
            }
        }
        m_TileSystem.SetPathMarkers(attackableTiles.ToArray(), null);

        UpdateActionInfo(_action);
    }
}