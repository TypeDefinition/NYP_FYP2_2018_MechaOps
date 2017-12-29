using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent]
public abstract class ViewScript : MonoBehaviour
{
    // Serialized Variable(s)
    [SerializeField] protected UnitStats m_UnitStats = null;

    // Non-Serialized Variable(s)
    protected TileSystem m_TileSystem = null;
    protected GameSystemsDirectory m_GameSystemsDirectory = null;

    protected virtual void Awake()
    {
        if (!m_UnitStats)
        {
            m_UnitStats = GetComponent<UnitStats>();
        }
        Assert.IsTrue(m_UnitStats != null, MethodBase.GetCurrentMethod().Name + " - m_UnitStats must not be null!");
        m_GameSystemsDirectory = m_UnitStats.GetGameSystemsDirectory();
        Assert.IsTrue(m_GameSystemsDirectory != null, MethodBase.GetCurrentMethod().Name + " - m_GameSystemsDirectory must not be null!");
        m_TileSystem = m_GameSystemsDirectory.GetTileSystem();
        Assert.IsTrue(m_TileSystem != null, MethodBase.GetCurrentMethod().Name + " - m_TileSystem must not be null!");
    }

    public virtual void SetVisibleTiles() {}

    public virtual void IncreaseVisibility() {}

    public virtual void DecreaseVisibility() {}

    public abstract bool IsVisible();

    public virtual bool RaycastToTile(Tile _tile)
    {
        // do a raycast between this gamobject and other go
        int layerMask = LayerMask.GetMask("TileDisplay");
        Vector3 rayDirection = _tile.transform.position - transform.position;
        Ray ray = new Ray(transform.position, rayDirection);
        RaycastHit[] hitInfo = Physics.RaycastAll(ray, rayDirection.magnitude, layerMask);
        Tile currentTile = m_GameSystemsDirectory.GetTileSystem().GetTile(m_UnitStats.CurrentTileID);
        foreach (RaycastHit hit in hitInfo)
        {
            TileDisplay tileDisplay = hit.collider.GetComponent<TileDisplay>();
            Assert.IsNotNull(tileDisplay, MethodBase.GetCurrentMethod().Name + " - Hit GameObject does not have a TileDisplay Component.");
            if (currentTile != tileDisplay.GetOwner() && _tile != tileDisplay.GetOwner())
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// It doesn't depends on it's own tile. it will simulate this unit is at the Origin Tile!
    /// </summary>
    /// <param name="_TargetTile">Target tile</param>
    /// <param name="_OriginTile">Origin tile!</param>
    /// <returns></returns>
    public virtual bool RaycastToTile(Tile _TargetTile, Tile _OriginTile)
    {
        int layerMask = LayerMask.GetMask("TileDisplay");
        // Simulate if the unit is at that tile!
        Vector3 UnitTileAtTile = _OriginTile.transform.position;
        UnitTileAtTile.y = transform.position.y;
        Vector3 rayDirection = _TargetTile.transform.position - UnitTileAtTile;
        Ray ray = new Ray(UnitTileAtTile, rayDirection);
        RaycastHit[] hitInfo = Physics.RaycastAll(ray, rayDirection.magnitude, layerMask);
        Tile currentTile = m_GameSystemsDirectory.GetTileSystem().GetTile(m_UnitStats.CurrentTileID);
        foreach (RaycastHit hit in hitInfo)
        {
            TileDisplay tileDisplay = hit.collider.GetComponent<TileDisplay>();
            Assert.IsNotNull(tileDisplay, MethodBase.GetCurrentMethod().Name + " - Hit GameObject does not have a TileDisplay Component.");
            if (currentTile != tileDisplay.GetOwner() && _TargetTile != tileDisplay.GetOwner())
            {
                return false;
            }
        }
        return true;
    }

    public virtual void Initialise() {}
}