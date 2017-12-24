using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

/// <summary>
/// Handles how the UI for unit walking works
/// </summary>
public class WalkUI_Logic : TweenUI_Scale
{
    [Header("The linking and variables needed for WalkUI")]
    protected GameSystemsDirectory m_GameSystemsDirectory = null;

    [Header("Debugging references")]
    [Tooltip("The array of tiles that the unit can reach")]
    public List<TileId> m_AllReachableTileHighlight;
    [SerializeField, Tooltip("The chosen path to walk to")]
    protected TileId[] m_MovementPath;
    [Tooltip("The Reference to unit walk action")]
    public UnitMoveAction m_UnitAction;

    // Action Name & Description
    [SerializeField] protected TextMeshProUGUI m_ActionNameText;
    [SerializeField] protected TextMeshProUGUI m_ActionDescriptionText;

    private TileSystem m_TileSystem = null;

    public void SetGameSystemsDirectory(GameSystemsDirectory _gameSystemDirectory)
    {
        m_GameSystemsDirectory = _gameSystemDirectory;
        m_TileSystem = m_GameSystemsDirectory.GetTileSystem();
    }

    public GameSystemsDirectory GetGameSystemsDirectory() { return m_GameSystemsDirectory; }

    protected override void Awake()
    {
        base.Awake();
        // Since the tile system is not linked from the start, find it at the scene.
        m_GameSystemsDirectory = FindObjectOfType<GameSystemsDirectory>();
        Assert.IsNotNull(m_GameSystemsDirectory, MethodBase.GetCurrentMethod().Name + " - m_GameSystemsDirectory must not be null!");
        m_TileSystem = m_GameSystemsDirectory.GetTileSystem();
    }

    private void InitEvents()
    {
        // And then access the tile stuff from the system and get reachable tiles
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("ClickedTile", PlayerClickedTile);
        GameEventSystem.GetInstance().SubscribeToEvent<IUnitAction>("SelectedAction", SetUnitAction);
    }

    private void DeinitEvents()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("ClickedTile", PlayerClickedTile);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<IUnitAction>("SelectedAction", SetUnitAction);
    }

    private void OnEnable()
    {
        AnimateUI();
        
        // For now, it will just pick the 1st enemy in the array
        GameEventSystem.GetInstance().TriggerEvent("ToggleSelectingUnit");
        InitEvents();
    }

    private void OnDisable()
    {
        // This is the only way to remove that green line renderer!
        m_TileSystem.ClearPathMarkers();
        DeinitEvents();
    }

    void PlayerClickedTile(GameObject _clickedTile)
    {
        if (_clickedTile.tag != "TileBase") { return; }

        Tile tileComponent = null;
        tileComponent = _clickedTile.GetComponent<Tile>();
        
        if (m_AllReachableTileHighlight.Contains(tileComponent.GetTileId()))
        {
            // Then we have to find the path for it!
            m_MovementPath = m_TileSystem.GetPath(m_UnitAction.MovementPoints, m_UnitAction.GetUnitStats().CurrentTileID, tileComponent.GetTileId(), m_UnitAction.GetUnitStats().GetTileAttributeOverrides());
            m_TileSystem.SetPathMarkers(m_AllReachableTileHighlight.ToArray(), m_MovementPath);
        }
    }

    /// <summary>
    /// When player pressed back button!
    /// </summary>
    public void PressedCancel()
    {
        // since the PlayerUnitSystem will gob back to normal
        GameEventSystem.GetInstance().TriggerEvent("ToggleSelectingUnit");
        // Since this UI is not needed anymore!
        Destroy(gameObject);
    }

    /// <summary>
    /// When player pressed the confirm button at the UI
    /// </summary>
    public void PressedConfirm()
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

    /// <summary>
    /// Called when the Player unit manager triggers this event!
    /// </summary>
    /// <param name="_action">The unit action that is supposed to pass in!</param>
    protected virtual void SetUnitAction(IUnitAction _action)
    {
        m_UnitAction = (UnitMoveAction)_action;

        // Set the name and description.
        m_ActionNameText.text = _action.UnitActionName;
        m_ActionDescriptionText.text = _action.UnitActionDescription;

        TileId[] reachableTiles = m_TileSystem.GetReachableTiles(m_UnitAction.MovementPoints, m_UnitAction.GetUnitStats().CurrentTileID, m_UnitAction.GetUnitStats().GetTileAttributeOverrides());
        m_AllReachableTileHighlight = new List<TileId>(reachableTiles);
        m_AllReachableTileHighlight.Remove(m_UnitAction.GetUnitStats().CurrentTileID);
        m_TileSystem.SetPathMarkers(m_AllReachableTileHighlight.ToArray(), null);
    }
}