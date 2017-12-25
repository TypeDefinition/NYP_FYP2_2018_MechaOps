using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will be used to render the tiles of this unit surrounding!
/// </summary>
[DisallowMultipleComponent]
public class PlayerViewScript : ViewScript
{
    [Header("The variables below are shown in the Inspector for debugging purposes.")]
    [SerializeField, Tooltip("Surrounding Tiles that needed to change the ID")]
    protected List<Tile> m_ViewedTiles;
    [SerializeField, Tooltip("Visibility counter of this player unit")]
    protected int m_VisibilityCounter = 0;

    /// <summary>
    /// This will help to render the tiles surrounding of this unit!
    /// </summary>
    public override void SetVisibleTiles()
    {
        ClearVisibleTiles();

        TileId[] surroundingTiles = m_TileSystem.GetSurroundingTiles(m_UnitStats.CurrentTileID, m_UnitStats.ViewRange);
        // And then iterate though the list and increase the tile range
        foreach (TileId tileId in surroundingTiles)
        {
            if (TileId.GetDistance(m_UnitStats.CurrentTileID, tileId) > m_UnitStats.ViewRange)
            {
                continue;
            }

            Tile tile = m_TileSystem.GetTile(tileId);
            // then raycast to that tile to see if it works and get the id
            int layerMask = LayerMask.GetMask("TileDisplay");
            Vector3 rayDirection = tile.transform.position - transform.position;
            RaycastHit hitInfo;
            // if it does not hit anything or ray has already reached it's destination
            if (!Physics.Raycast(transform.position, rayDirection, out hitInfo, rayDirection.magnitude, layerMask) ||
                hitInfo.collider.transform.parent.GetComponent<Tile>().GetTileId().Equals(tileId))
            {
                m_ViewedTiles.Add(tile);
                ++tile.VisibleCounter;
            }
        }
    }

    /// <summary>
    /// Decrease the visibility counter of the tiles that are out of range.
    /// </summary>
    public virtual void ClearVisibleTiles()
    {
        foreach (Tile tile in m_ViewedTiles)
        {
            --tile.VisibleCounter;
        }
        m_ViewedTiles.Clear();
    }

    /// <summary>
    /// Since Player units will always be visible to the players!
    /// </summary>
    public override bool IsVisible()
    {
        if (m_VisibilityCounter == 0)
        {
            return false;
        }
        return true;
    }

    public override void IncreaseVisibility()
    {
        ++m_VisibilityCounter;
        if (m_VisibilityCounter == 1)
        {
            GameEventSystem.GetInstance().TriggerEvent<GameObject>("UnitSeen", gameObject);
        }
    }

    public override void DecreaseVisibility()
    {
        m_VisibilityCounter = Mathf.Max(m_VisibilityCounter - 1, 0);
        if (m_VisibilityCounter == 0)
        {
            GameEventSystem.GetInstance().TriggerEvent<GameObject>("UnitUnseen", gameObject);
        }
    }
}
