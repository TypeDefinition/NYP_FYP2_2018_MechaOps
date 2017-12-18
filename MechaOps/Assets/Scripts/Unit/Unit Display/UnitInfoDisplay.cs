﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// meant to just display the stats UI only!
/// </summary>
public class UnitInfoDisplay : TweenUI_Scale
{
    [SerializeField] protected HealthBar m_HealthBar = null;
    [SerializeField] protected ActionPointsCounter m_ActionPointsCounter = null;
    [Tooltip("This offset is added to the unit's world space position when converting from the unit's world space to screen space position.")]
    [SerializeField] protected Vector3 m_UnitWorldPositionOffset = new Vector3(0.0f, 2.5f, 0.0f);

    protected UnitStats m_UnitStats = null;

    public HealthBar GetHealthBar() { return m_HealthBar; }
    public ActionPointsCounter GetActionPointsCounter() { return m_ActionPointsCounter; }

    public void SetUnitStats(UnitStats _unitStats) { m_UnitStats = _unitStats; }
    public UnitStats GetUnitStats() { return m_UnitStats; }

    public Vector3 GetUnitWorldPositionOffset() { return m_UnitWorldPositionOffset; }

    private void StatsChangeCallback(UnitStats _unitStats)
    {
        // Update Health Points
        m_HealthBar.MaxHealthPoints = _unitStats.MaxHealthPoints;
        m_HealthBar.CurrentHealthPoints = _unitStats.CurrentHealthPoints;

        // Update Action Points
        m_ActionPointsCounter.MaxActionPoints = _unitStats.MaxActionPoints;
        m_ActionPointsCounter.CurrentActionPoints = _unitStats.CurrentActionPoints;
    }

    private void OnEnable() { AnimateUI(); }

    private void OnDisable() {}

    protected override void Awake()
    {
        base.Awake();
        Assert.IsTrue(m_HealthBar != null, MethodBase.GetCurrentMethod().Name + " - m_HealthBar must not be null!");
        Assert.IsTrue(m_ActionPointsCounter != null, MethodBase.GetCurrentMethod().Name + " - m_ActionPointsCounter must not be null!");
    }

    protected void Update()
    {
        if (m_UnitStats != null)
        {
            // Update Position
            Canvas screenSpaceCanvas = m_UnitStats.GetGameSystemsDirectory().GetScreenSpaceCanvas();
            Camera gameCamera = m_UnitStats.GetGameSystemsDirectory().GetGameCamera();
            Vector3 screenPoint = gameCamera.WorldToScreenPoint(m_UnitStats.gameObject.transform.position + m_UnitWorldPositionOffset);
            transform.position = screenPoint;
        }
    }

#if UNITY_EDITOR
    private void OnValidate() {}
#endif // UNITY_EDITOR

}