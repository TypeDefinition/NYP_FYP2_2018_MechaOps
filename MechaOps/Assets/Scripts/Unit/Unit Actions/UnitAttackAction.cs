﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class UnitAttackAction : IUnitAction
{
    // Serialised Variable(s)
    [SerializeField, Tooltip("Minimum attack range of the unit")]
    protected int m_MinAttackRange = 0;
    [SerializeField, Tooltip("Maximum attack range of the unit")]
    protected int m_MaxAttackRange = 6;
    [SerializeField, Tooltip("The accuracy points of the unit")]
    protected int m_AccuracyPoints = 50;
    [SerializeField, Tooltip("The damage point it dealt")]
    protected int m_DamagePoints = 2;
    // Shoot Animation Variable(s)
    [SerializeField] protected DamageIndicator m_DamageIndicatorPrefab = null;

    // Non-Serialised Variable(s)
    protected UnitStats m_TargetUnitStats = null;

    public int MinAttackRange
    {
        get { return m_MinAttackRange; }
        set { m_MinAttackRange = Mathf.Clamp(value, 0, MaxAttackRange); }
    }

    public int MaxAttackRange
    {
        get { return m_MaxAttackRange; }
        set
        {
            m_MaxAttackRange = Mathf.Max(0, value);
            m_MinAttackRange = Mathf.Min(m_MaxAttackRange, m_MinAttackRange);
        }
    }

    public int AccuracyPoints
    {
        get { return m_AccuracyPoints; }
        set { m_AccuracyPoints = Mathf.Max(0, value); }
    }

    public int DamagePoints
    {
        get { return m_DamagePoints; }
        set { m_DamagePoints = Mathf.Max(0, value); }
    }

    public virtual void SetTarget(GameObject _target)
    {
        Assert.IsNotNull(_target, MethodBase.GetCurrentMethod().Name + " - _target is null!");
        m_TargetUnitStats = _target.GetComponent<UnitStats>();
        Assert.IsNotNull(m_TargetUnitStats, MethodBase.GetCurrentMethod().Name + " - _target has no UnitStats!");
    }

    public void RemoveTarget()
    {
        m_TargetUnitStats = null;
    }

    public UnitStats GetTargetUnitStats()
    {
        return m_TargetUnitStats;
    }
    
    public override bool VerifyRunCondition()
    {
        // Exit Checks
        if (m_UnitStats.IsAlive() == false) { return false; }
        if (m_TargetUnitStats == null) { return false; }
        if (m_TargetUnitStats.IsAlive() == false) { return false; }
        int distanceToTarget = TileId.GetDistance(m_TargetUnitStats.CurrentTileID, GetUnitStats().CurrentTileID);
        if (distanceToTarget < MinAttackRange) { return false; }
        if (distanceToTarget > MaxAttackRange) { return false; }

        return true;
    }

    public virtual void CreateDamageIndicator(bool _hit, int _damageValue, GameObject _target)
    {
        Canvas unclickableCanvas = GameSystemsDirectory.GetSceneInstance().GetUnclickableScreenSpaceCanvas();
        DamageIndicator damageIndicator = Instantiate(m_DamageIndicatorPrefab.gameObject, unclickableCanvas.transform).GetComponent<DamageIndicator>();
        damageIndicator.Hit = _hit;
        damageIndicator.DamageValue = _damageValue;
        damageIndicator.Target = _target;
    }

    /// <summary>
    /// Calculate the hit percentage of this attack.
    /// It should return an int between 1(Inclusive) and 100(Inclusive).
    /// </summary>
    public virtual int CalculateHitChance()
    {
        int distanceToTarget = TileId.GetDistance(m_TargetUnitStats.CurrentTileID, GetUnitStats().CurrentTileID);
        int optimalDistance = MinAttackRange;
        float hitChance = 1.0f - (distanceToTarget - optimalDistance) / (MaxAttackRange - optimalDistance);
        hitChance *= 100.0f;
        hitChance -= (float)m_TargetUnitStats.EvasionPoints;
        hitChance += (float)m_AccuracyPoints;

        return Mathf.Clamp((int)hitChance, 1, 100);
    }

    /// <summary>
    /// This function rolls a random number between 1 (inclusive) to 101 (exclusive).
    /// It returns true of the random number is lower or equal to CalculateHitChance().
    /// </summary>
    /// <returns></returns>
    protected virtual bool CheckIfHit()
    {
        return Random.Range(1, 101) <= CalculateHitChance();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        MinAttackRange = m_MinAttackRange;
        MaxAttackRange = m_MaxAttackRange;
        AccuracyPoints = m_AccuracyPoints;
        DamagePoints = m_DamagePoints;
    }
#endif
}