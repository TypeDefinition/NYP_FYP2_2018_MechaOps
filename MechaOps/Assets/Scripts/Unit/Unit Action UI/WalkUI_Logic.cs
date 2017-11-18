using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles how the UI for unit walking works
/// </summary>
public class WalkUI_Logic : MonoBehaviour {
    [Header("The linking and variables needed for WalkUI")]
    [Tooltip("The References to TileSystem")]
    public TileSystem m_TileSys;

    [Header("Debugging references")]
    [Tooltip("The array of tiles that the unit can reach")]
    public List<TileId> m_AllReachableTileHighlight;
    [SerializeField, Tooltip("The chosen path to walk to")]
    protected TileId[] m_ReachablePath;
    [Tooltip("The Reference to unit walk action")]
    public UnitWalkAction m_UnitWalkRef;

    private void OnEnable()
    { 
        // For now, it will just pick the 1st enemy in the array
        GameEventSystem.GetInstance().TriggerEvent("ToggleSelectingUnit");
        // Since the tile system is not linked from the start, find it at the scene
        m_TileSys = FindObjectOfType<TileSystem>();
        // And then access the tile stuff from the system and get reachable tiles
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("ClickedUnit", PlayerClickedTile);
        GameEventSystem.GetInstance().SubscribeToEvent<IUnitAction>("SelectedAction", PressedAction);
    }

    private void OnDisable()
    {
        // This is the only way to remove that green line renderer!
        m_TileSys.ClearPathMarkers();
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("ClickedUnit", PlayerClickedTile);
    }

    void PlayerClickedTile(GameObject _clickedObj)
    {
        //GameObject zeClickedObj = ObserverSystemScript.Instance.GetStoredEventVariable<GameObject>("ClickedUnit");
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
            if (m_AllReachableTileHighlight.Contains(zeTileComponent.GetId()))
            {
                // Then we have to find the path for it!
                m_ReachablePath = m_TileSys.GetPath(m_UnitWalkRef.m_MovementPoints, m_UnitWalkRef.GetUnitStats().CurrentTileID, zeTileComponent.GetId(), m_UnitWalkRef.GetUnitStats().GetTileAttributeOverrides());
                m_TileSys.SetPathMarkers(m_AllReachableTileHighlight.ToArray(), m_ReachablePath);
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
        switch (m_ReachablePath.Length)
        {
            case 0:
                print("Player is trying to walk no path");
                break;
            default:
                m_UnitWalkRef.m_TilePath = m_ReachablePath;
                UnitActionScheduler zeActScheduler = FindObjectOfType<UnitActionScheduler>();
                m_UnitWalkRef.TurnOn();
                zeActScheduler.ScheduleAction(m_UnitWalkRef);
                Destroy(gameObject);
                break;
        }
    }

    /// <summary>
    /// Called when the Player unit manager triggers this event!
    /// </summary>
    /// <param name="_action">The unit action that is supposed to pass in!</param>
    protected virtual void PressedAction(IUnitAction _action)
    {
        m_UnitWalkRef = (UnitWalkAction)_action;
        TileId[] zeReachableTiles = m_TileSys.GetReachableTiles(m_UnitWalkRef.m_MovementPoints, m_UnitWalkRef.GetUnitStats().CurrentTileID, m_UnitWalkRef.GetUnitStats().GetTileAttributeOverrides());
        m_AllReachableTileHighlight = new List<TileId>(zeReachableTiles);
        m_AllReachableTileHighlight.Remove(m_UnitWalkRef.m_UnitStats.CurrentTileID);
        m_TileSys.SetPathMarkers(m_AllReachableTileHighlight.ToArray(), null);
    }
}
