﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// This will be used to render this unit invisible! inherit from PlayerViewScript to be lazy
/// </summary>
[DisallowMultipleComponent]
public class AIViewScript : ViewScript
{
    protected MeshRenderer[] m_AllRenderers = null; // The array of MeshRenderer inside this unit.
    protected bool m_AlwaysRender = false;

    protected override void InitEvents()
    {
        base.InitEvents();
        GameEventSystem.GetInstance().SubscribeToEvent<UnitStats, bool>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitDead), OnUnitDead);
    }

    protected override void DeinitEvents()
    {
        base.DeinitEvents();
        GameEventSystem.GetInstance().UnsubscribeFromEvent<UnitStats, bool>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitDead), OnUnitDead);
    }

    protected override void Awake()
    {
        m_AllRenderers = GetComponentsInChildren<MeshRenderer>();
        base.Awake();
    }

    // Callbacks
    protected override void OnUnitsSpawned()
    {
        m_UnitStats.GetUnitInfoDisplay().gameObject.SetActive(m_VisibilityCount > 0);
    }

    protected virtual void OnUnitDead(UnitStats _deadUnit, bool _deadUnitVisible)
    {
        if (_deadUnit != m_UnitStats) { return; }

        m_AlwaysRender = true;
        // ensuring that the renderer will be set from inactive to active
        if (!m_AllRenderers[0].enabled)
        {
            foreach (MeshRenderer renderer in m_AllRenderers)
            {
                renderer.enabled = true;
            }
        }
    }

    /// <summary>
    /// Decrease the counter for number of units looking at this.
    /// If no units is looking at this, it becomes invisible.
    /// </summary>
    public override void DecreaseVisibility()
    {
        --m_VisibilityCount;
        Assert.IsTrue(m_VisibilityCount >= 0);
        if (m_VisibilityCount == 0)
        {
            foreach (MeshRenderer renderer in m_AllRenderers) { renderer.enabled = m_AlwaysRender; }

            m_UnitStats.GetUnitInfoDisplay().gameObject.SetActive(false);
            GameEventSystem.GetInstance().TriggerEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitUnseen), m_UnitStats);
        }
    }

    /// <summary>
    /// Increase the counter for number of units looking at this.
    /// If no units is looking at this, it becomes invisible.
    /// </summary>
    public override void IncreaseVisibility()
    {
        ++m_VisibilityCount;
        Assert.IsTrue(m_VisibilityCount >= 0);
        if (m_VisibilityCount == 1)
        {
            foreach (MeshRenderer renderer in m_AllRenderers) { renderer.enabled = true; }

            m_UnitStats.GetUnitInfoDisplay().gameObject.SetActive(true);
            GameEventSystem.GetInstance().TriggerEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitSeen), m_UnitStats);
        }
    }
}