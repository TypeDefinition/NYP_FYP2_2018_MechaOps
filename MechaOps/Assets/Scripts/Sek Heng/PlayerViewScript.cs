using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will be used to render the tiles of this unit surrounding!
/// </summary>
public class PlayerViewScript : ViewTileScript
{
    [Header("Debugging purpose")]
    [SerializeField, Tooltip("Surrounding Tiles that needed to change the ID")]
    protected List<Tile> m_ViewedTiles;

    /// <summary>
    /// This will help to render the tiles surrounding of this unit!
    /// </summary>
    public override void RenderSurroundingTiles()
    {
        CheckSurroundingTiles();
        // find the tile system if there is none!
        if (!m_TileSys)
            m_TileSys = FindObjectOfType<TileSystem>();
        TileId[] zeAllSurroundingTiles = m_TileSys.GetSurroundingTiles(m_Stats.CurrentTileID, m_Stats.ViewRange);
        // And then iterate though the list and increase the tile range
        foreach (TileId zeTileID in zeAllSurroundingTiles)
        {
            if (TileId.GetDistance(m_Stats.CurrentTileID, zeTileID) <= m_Stats.ViewRange)
            {
                Tile zeTile = m_TileSys.GetTile(zeTileID);
                // then raycast to that tile to see if it works and get the id
                int layerToCastThrough = 1 << LayerMask.NameToLayer("TileDisplay");
                Vector3 zeDirection = zeTile.transform.position - transform.position;
                zeDirection.y = transform.position.y;
                RaycastHit zeHitCast;
                // if it does not hit anything or ray has already reached it's destination
                if (!Physics.Raycast(transform.position, zeDirection, out zeHitCast, zeDirection.magnitude, layerToCastThrough) || zeHitCast.collider.transform.parent.GetComponent<Tile>().GetId().Equals(zeTileID))
                {
                    // see if it is inside the viewedTiles
                    if (!m_ViewedTiles.Contains(zeTile))
                    {
                        m_ViewedTiles.Add(zeTile);
                        ++zeTile.VisibleCounter;
                    }
                }
                else if (m_ViewedTiles.Contains(zeTile))
                {
                    // if the tile is inside the list and something is blocking the view
                    --zeTile.VisibleCounter;
                    m_ViewedTiles.Remove(zeTile);
                }
            }
        }
    }

    /// <summary>
    /// Check if the surrounding tiles is within the range
    /// </summary>
    public virtual void CheckSurroundingTiles()
    {
        if (!m_TileSys)
            m_TileSys = FindObjectOfType<TileSystem>();
        List<Tile> zeTileNeedRemoved = new List<Tile>();
        foreach (Tile zeTile in m_ViewedTiles)
        {
            if (TileId.GetDistance(m_Stats.CurrentTileID, zeTile.GetId()) > m_Stats.ViewRange)
            {
                --zeTile.VisibleCounter;
                zeTileNeedRemoved.Add(zeTile);
            }
        }
        // we have to do this loop twice otherwise there will be an error at the above!
        foreach (Tile zeTile in zeTileNeedRemoved)
        {
            m_ViewedTiles.Remove(zeTile);
        }
    }
}
