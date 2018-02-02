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
    [SerializeField] protected int m_VisibilityCount = 0; // This is the counter for the number of units spotting this unit.
    protected TileSystem m_TileSystem = null;
    protected GameSystemsDirectory m_GameSystemsDirectory = null;
    protected GameEventNames m_GameEventNames = null;

    public int GetVisibilityCount() { return m_VisibilityCount; }

    protected virtual void InitEvents()
    {
        GameEventSystem.GetInstance().SubscribeToEvent(m_GameEventNames.GetEventName(GameEventNames.SpawnSystemNames.UnitsSpawned), OnUnitsSpawned);
    }

    protected virtual void DeinitEvents()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent(m_GameEventNames.GetEventName(GameEventNames.SpawnSystemNames.UnitsSpawned), OnUnitsSpawned);
    }

    protected virtual void Awake()
    {
        Assert.IsTrue(m_UnitStats != null, MethodBase.GetCurrentMethod().Name + " - m_UnitStats must not be null!");
        m_GameSystemsDirectory = GameSystemsDirectory.GetSceneInstance();
        Assert.IsTrue(m_GameSystemsDirectory != null, MethodBase.GetCurrentMethod().Name + " - m_GameSystemsDirectory must not be null!");
        m_TileSystem = m_GameSystemsDirectory.GetTileSystem();
        Assert.IsTrue(m_TileSystem != null, MethodBase.GetCurrentMethod().Name + " - m_TileSystem must not be null!");
        m_GameEventNames = m_GameSystemsDirectory.GetGameEventNames();

        InitEvents();
    }

    protected virtual void OnDestroy()
    {
        DeinitEvents();
    }

    protected abstract void OnUnitsSpawned();

    public abstract void IncreaseVisibility();

    public abstract void DecreaseVisibility();

    /// <summary>
    /// This function only checks if the unit is visible to the player!
    /// To find out if this unit is visible to the opposing force,
    /// use GetVisibilityCount() > 0.
    /// </summary>
    /// <returns></returns>
    public abstract bool IsVisibleToPlayer();

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
}