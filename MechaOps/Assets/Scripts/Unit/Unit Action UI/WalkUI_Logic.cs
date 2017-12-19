using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles how the UI for unit walking works
/// </summary>
public class WalkUI_Logic : TweenUI_Scale
{
    [Header("The linking and variables needed for WalkUI")]
    [Tooltip("The References to TileSystem")]
    public TileSystem m_TileSystem;

    [Header("Debugging references")]
    [Tooltip("The array of tiles that the unit can reach")]
    public List<TileId> m_AllReachableTileHighlight;
    [SerializeField, Tooltip("The chosen path to walk to")]
    protected TileId[] m_MovementPath;
    [Tooltip("The Reference to unit walk action")]
    public UnitWalkAction m_UnitWalkRef;

    private void OnEnable()
    {
        AnimateUI();
        // For now, it will just pick the 1st enemy in the array
        GameEventSystem.GetInstance().TriggerEvent("ToggleSelectingUnit");

        // And then access the tile stuff from the system and get reachable tiles
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("ClickedUnit", PlayerClickedTile);
        GameEventSystem.GetInstance().SubscribeToEvent<IUnitAction>("SelectedAction", PressedAction);

        // Since the tile system is not linked from the start, find it at the scene
        m_TileSystem = FindObjectOfType<TileSystem>();
    }

    private void OnDisable()
    {
        // This is the only way to remove that green line renderer!
        m_TileSystem.ClearPathMarkers();

        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("ClickedUnit", PlayerClickedTile);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<IUnitAction>("SelectedAction", PressedAction);
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
                m_MovementPath = m_TileSystem.GetPath(m_UnitWalkRef.m_MovementPoints, m_UnitWalkRef.GetUnitStats().CurrentTileID, zeTileComponent.GetTileId(), m_UnitWalkRef.GetUnitStats().GetTileAttributeOverrides());
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
            m_UnitWalkRef.SetTilePath(m_MovementPath);
            UnitActionScheduler zeActScheduler = FindObjectOfType<UnitActionScheduler>();
            m_UnitWalkRef.TurnOn();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called when the Player unit manager triggers this event!
    /// </summary>
    /// <param name="_action">The unit action that is supposed to pass in!</param>
    protected virtual void PressedAction(IUnitAction _action)
    {
        m_UnitWalkRef = (UnitWalkAction)_action;
        TileId[] zeReachableTiles = m_TileSystem.GetReachableTiles(m_UnitWalkRef.m_MovementPoints, m_UnitWalkRef.GetUnitStats().CurrentTileID, m_UnitWalkRef.GetUnitStats().GetTileAttributeOverrides());
        m_AllReachableTileHighlight = new List<TileId>(zeReachableTiles);
        m_AllReachableTileHighlight.Remove(m_UnitWalkRef.GetUnitStats().CurrentTileID);
        m_TileSystem.SetPathMarkers(m_AllReachableTileHighlight.ToArray(), null);
    }
}
