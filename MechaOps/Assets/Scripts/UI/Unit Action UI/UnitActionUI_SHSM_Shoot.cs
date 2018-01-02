using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

/// <summary>
/// The logic for artillery attack UI
/// </summary>
public class UnitActionUI_SHSM_Shoot : UnitActionUI
{
    // Serialised Variable(s)
    [SerializeField] protected GameObject m_TargetTilesMarker_Prefab = null;

    // Non-Serialised Variable(s)
    // Unit Action
    protected SHSMShootAction m_UnitAction = null;

    // Tiles
    protected Tile m_TargetedTile = null;
    protected List<TileId> m_AttackableTiles = new List<TileId>();
    protected List<GameObject> m_TargetTilesMarkers = new List<GameObject>();

    // Systems
    protected TileSystem m_TileSystem = null;

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
        m_GameEventNames = GameSystemsDirectory.GetSceneInstance().GetGameEventNames();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        ClearTargetedTileMarkers();
        m_TileSystem.ClearPathMarkers();
    }

    protected void ClearTargetedTileMarkers()
    {
        for (int i = 0; i < m_TargetTilesMarkers.Count; ++i)
        {
            GameObject.Destroy(m_TargetTilesMarkers[i]);
        }
        m_TargetTilesMarkers.Clear();
    }

    public override void PressedConfirm()
    {
        if (m_TargetedTile)
        {
            m_UnitAction.SetTarget(m_TargetedTile.gameObject);
            m_UnitAction.TurnOn();
            Destroy(gameObject);
        }
    }

    // Callbacks
    /// <summary>
    /// To get the event message from the GameEventSystem
    /// </summary>
    /// <param name="_action"></param>
    protected override void SetUnitAction(IUnitAction _action)
    {
        m_UnitAction = _action as SHSMShootAction;

        // Set the name and description.
        m_ActionNameText.text = _action.UnitActionName;
        m_ActionDescriptionText.text = _action.UnitActionDescription;

        // then we highlight all of the tiles from here
        TileId[] tilesInRange = m_TileSystem.GetSurroundingTiles(m_UnitAction.GetUnitStats().CurrentTileID, m_UnitAction.MaxAttackRange);
        foreach (TileId tileId in tilesInRange)
        {
            int distanceToTile = TileId.GetDistance(m_UnitAction.GetUnitStats().CurrentTileID, tileId);
            // check if it is within the range
            if (distanceToTile >= m_UnitAction.MinAttackRange && distanceToTile <= m_UnitAction.MaxAttackRange)
            {
                m_AttackableTiles.Add(tileId);
            }
        }

        m_TileSystem.SetPathMarkers(m_AttackableTiles.ToArray(), null);
    }

    /// <summary>
    /// When player clicked the tile or unit!
    /// </summary>
    /// <param name="_clickedTile"></param>
    protected void OnClickedTile(GameObject _clickedTile)
    {
        Assert.IsNotNull(_clickedTile.GetComponent<Tile>(), MethodBase.GetCurrentMethod().Name + " - A clicked tile has no Tile Component.");

        TileId clickedTileId = _clickedTile.GetComponent<Tile>().GetTileId();

        // Ensure that this tile is within our attack range.
        int distanceToClickedTile = TileId.GetDistance(clickedTileId, m_UnitAction.GetUnitStats().CurrentTileID);
        if (distanceToClickedTile < m_UnitAction.MinAttackRange || distanceToClickedTile > m_UnitAction.MaxAttackRange) { return; }

        if (clickedTileId.Equals(m_UnitAction.GetUnitStats().CurrentTileID)) { return; }

        m_TargetedTile = _clickedTile.GetComponent<Tile>();
        ClearTargetedTileMarkers();

        TileId[] targetedTiles = m_TileSystem.GetSurroundingTiles(clickedTileId, m_UnitAction.GetTargetRadius());
        for (int i = 0; i < targetedTiles.Length; ++i)
        {
            GameObject targetTileMarker = GameObject.Instantiate(m_TargetTilesMarker_Prefab);
            targetTileMarker.transform.position = m_TileSystem.GetTile(targetedTiles[i]).transform.position;
            m_TargetTilesMarkers.Add(targetTileMarker);
        }
    }
}