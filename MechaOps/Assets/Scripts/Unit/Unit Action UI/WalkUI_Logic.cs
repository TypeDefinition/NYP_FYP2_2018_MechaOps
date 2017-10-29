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
    [Tooltip("A fanciful highlight of reachable tiles. Maybe use a material for now")]
    public Material m_HighlightedTileMat;

    [Header("Debugging references")]
    [Tooltip("The array of tiles that the unit can reach")]
    public List<TileId> m_AllReachableTileHighlight;
    [Tooltip("The array of original material of the reachable tiles")]
    public Material[] m_OriginalTileMaterials;
    [Tooltip("The Reference to unit walk action")]
    public UnitWalkAction m_UnitWalkRef;

    private void OnEnable()
    {
        // TODO: Use another system aside from ObserverSystem for better optimization. maybe.
        m_UnitWalkRef = ObserverSystemScript.Instance.GetStoredEventVariable<UnitWalkAction>(tag);
        ObserverSystemScript.Instance.RemoveTheEventVariableNextFrame(tag);
        // For now, it will just pick the 1st enemy in the array
        ObserverSystemScript.Instance.TriggerEvent("ToggleSelectingUnit");
        // And then access the tile stuff from the system and get reachable tiles
        m_AllReachableTileHighlight = new List<TileId>(m_TileSys.GetReachableTiles(m_UnitWalkRef.m_MovementPoints, m_UnitWalkRef.m_UnitStatGO.m_CurrentTileID, m_UnitWalkRef.m_UnitStatGO.m_TileAttributeOverrides));
        // TODO: make a fanciful UI or highlight for the reachable tiles.
        // TODO: Cant really highlight the reachable tiles
        //foreach (TileId zeTile in m_AllReachableTileHighlight)
        //{
        //    MeshRenderer zeTileRenderer = zeTile
        //}
        ObserverSystemScript.Instance.SubscribeEvent("ClickedUnit", PlayerClickedTile);
    }

    private void OnDisable()
    {
        ObserverSystemScript.Instance.UnsubscribeEvent("ClickedUnit", PlayerClickedTile);
    }

    void PlayerClickedTile()
    {
        GameObject zeClickedObj = ObserverSystemScript.Instance.GetStoredEventVariable<GameObject>("ClickedUnit");
        if (zeClickedObj.tag == "TileBase")
        {
            Tile zeTileComponent = zeClickedObj.GetComponent<Tile>();
            if (m_AllReachableTileHighlight.Contains(zeTileComponent.GetId()))
            {
                // Then we have to find the path for it!
                m_UnitWalkRef.m_TilePath = m_TileSys.GetPath(m_UnitWalkRef.m_MovementPoints, m_UnitWalkRef.m_UnitStatGO.m_CurrentTileID, zeTileComponent.GetId(), m_UnitWalkRef.m_UnitStatGO.m_TileAttributeOverrides);
                gameObject.SetActive(false);
            }
        }
    }
}
