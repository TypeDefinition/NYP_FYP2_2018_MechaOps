using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

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
    public UnitWalkAction m_UnitAction;

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
        Assert.IsNotNull(m_GameSystemsDirectory);
        m_TileSystem = m_GameSystemsDirectory.GetTileSystem();
    }

    private void InitEvents()
    {
        // And then access the tile stuff from the system and get reachable tiles
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("ClickedUnit", PlayerClickedTile);
        GameEventSystem.GetInstance().SubscribeToEvent<IUnitAction>("SelectedAction", SetUnitAction);
    }

    private void DeinitEvents()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("ClickedUnit", PlayerClickedTile);
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

    void PlayerClickedTile(GameObject _clickedObj)
    {
        if (_clickedObj.tag == "TileDisplay" || _clickedObj.tag == "TileBase")
        {
            // We have to hardcode a bit since the tags will differ!
            Tile zeTileComponent = null;
            switch (_clickedObj.tag)
            {
                case "TileDisplay":
                    zeTileComponent = _clickedObj.transform.parent.GetComponent<Tile>();
                    break;
                case "TileBase":
                    zeTileComponent = _clickedObj.GetComponent<Tile>();
                    break;
            }
            if (m_AllReachableTileHighlight.Contains(zeTileComponent.GetTileId()))
            {
                // Then we have to find the path for it!
                m_MovementPath = m_TileSystem.GetPath(m_UnitAction.m_MovementPoints, m_UnitAction.GetUnitStats().CurrentTileID, zeTileComponent.GetTileId(), m_UnitAction.GetUnitStats().GetTileAttributeOverrides());
                m_TileSystem.SetPathMarkers(m_AllReachableTileHighlight.ToArray(), m_MovementPath);
            }
        }
    }

    /// <summary>
    /// When player pressed back button!
    /// </summary>
    public void PressedBack()
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
        m_UnitAction = (UnitWalkAction)_action;
        TileId[] reachableTiles = m_TileSystem.GetReachableTiles(m_UnitAction.m_MovementPoints, m_UnitAction.GetUnitStats().CurrentTileID, m_UnitAction.GetUnitStats().GetTileAttributeOverrides());
        m_AllReachableTileHighlight = new List<TileId>(reachableTiles);
        m_AllReachableTileHighlight.Remove(m_UnitAction.GetUnitStats().CurrentTileID);
        m_TileSystem.SetPathMarkers(m_AllReachableTileHighlight.ToArray(), null);
    }
}