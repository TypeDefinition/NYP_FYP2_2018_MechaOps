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

    protected void Awake()
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
}