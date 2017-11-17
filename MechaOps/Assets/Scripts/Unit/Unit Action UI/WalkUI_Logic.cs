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
        // TODO: Use another system aside from ObserverSystem for better optimization. maybe.
        m_UnitWalkRef = ObserverSystemScript.Instance.GetStoredEventVariable<UnitWalkAction>("SelectedAction");
        ObserverSystemScript.Instance.RemoveTheEventVariableNextFrame("SelectedAction");
        // For now, it will just pick the 1st enemy in the array
        ObserverSystemScript.Instance.TriggerEvent("ToggleSelectingUnit");
        // Since the tile system is not linked from the start, find it at the scene
        m_TileSys = FindObjectOfType<TileSystem>();
        // And then access the tile stuff from the system and get reachable tiles
        TileId[] zeReachableTiles = m_TileSys.GetReachableTiles(m_UnitWalkRef.m_MovementPoints, m_UnitWalkRef.GetUnitStats().CurrentTileID, m_UnitWalkRef.GetUnitStats().GetTileAttributeOverrides());
        m_AllReachableTileHighlight = new List<TileId>(zeReachableTiles);
        m_TileSys.SetPathMarkers(zeReachableTiles, null);
        //ObserverSystemScript.Instance.SubscribeEvent("ClickedUnit", PlayerClickedTile);
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("ClickedUnit", PlayerClickedTile);
    }

    private void OnDisable()
    {
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
            if (!m_UnitWalkRef.m_UnitStats.CurrentTileID.Equals(zeTileComponent.GetId()) && m_AllReachableTileHighlight.Contains(zeTileComponent.GetId()))
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
        ObserverSystemScript.Instance.TriggerEvent("ToggleSelectingUnit");
        // Since this UI is not needed anymore!
        Destroy(gameObject);
    }

    public void PressedConfirm()
    {
        // then the unit will walk that path! if the path exists
        if (m_ReachablePath != null)
        {
            m_UnitWalkRef.m_TilePath = m_ReachablePath;
            UnitActionScheduler zeActScheduler = FindObjectOfType<UnitActionScheduler>();
            m_UnitWalkRef.TurnOn();
            zeActScheduler.ScheduleAction(m_UnitWalkRef);
            Destroy(gameObject);
        }
    }
}
