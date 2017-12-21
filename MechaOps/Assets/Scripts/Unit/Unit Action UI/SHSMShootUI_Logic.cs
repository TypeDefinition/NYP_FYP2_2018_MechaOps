using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

/// <summary>
/// The logic for artillery attack UI
/// </summary>
public class SHSMShootUI_Logic : TweenUI_Scale
{
    // Unit Action
    protected SHSMShootAction m_UnitAction = null;

    // Tiles
    protected List<TileId> m_AttackableTiles = new List<TileId>();
    protected Tile m_TargetedTile = null;
    [SerializeField] protected GameObject m_TargetTilesMarker_Prefab = null;
    protected List<GameObject> m_TargetTilesMarkers = new List<GameObject>();

    // Systems
    protected GameSystemsDirectory m_GameSystemsDirectory = null;
    protected TileSystem m_TileSystem = null;

    // Action Name & Description
    [SerializeField] protected TextMeshProUGUI m_ActionNameText;
    [SerializeField] protected TextMeshProUGUI m_ActionDescriptionText;

    private void InitEvents()
    {
        GameEventSystem.GetInstance().SubscribeToEvent<IUnitAction>("SelectedAction", SetUnitAction);
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("ClickedTile", ClickedTile);
    }

    private void DeinitEvents()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<IUnitAction>("SelectedAction", SetUnitAction);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("ClickedTile", ClickedTile);
    }

    protected override void Awake()
    {
        base.Awake();
        m_GameSystemsDirectory = FindObjectOfType<GameSystemsDirectory>();
        m_TileSystem = m_GameSystemsDirectory.GetTileSystem();
    }

    private void OnEnable()
    {
        // Animate the UI when enabled
        AnimateUI();

        GameEventSystem.GetInstance().TriggerEvent("ToggleSelectingUnit");
        InitEvents();
    }

    private void OnDisable()
    {
        DeinitEvents();
        ClearTargetedTileMarkers();
        m_TileSystem.ClearPathMarkers();
    }

    public void PressedConfirm()
    {
        if (m_TargetedTile)
        {
            m_UnitAction.SetTarget(m_TargetedTile.gameObject);
            m_UnitAction.TurnOn();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// When pressing the back button
    /// </summary>
    public void PressedCancel()
    {
        // ensure that the player will be able to click on unit again!
        GameEventSystem.GetInstance().TriggerEvent("ToggleSelectingUnit");
        // there is no point in keeping this UI anymore so destroy it!
        Destroy(gameObject);
    }

    /// <summary>
    /// To get the event message from the GameEventSystem
    /// </summary>
    /// <param name="_action"></param>
    public void SetUnitAction(IUnitAction _action)
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

    protected void ClearTargetedTileMarkers()
    {
        for (int i = 0; i < m_TargetTilesMarkers.Count; ++i)
        {
            GameObject.Destroy(m_TargetTilesMarkers[i]);
        }
        m_TargetTilesMarkers.Clear();
    }

    /// <summary>
    /// When player clicked the tile or unit!
    /// </summary>
    /// <param name="_clickedTile"></param>
    protected void ClickedTile(GameObject _clickedTile)
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