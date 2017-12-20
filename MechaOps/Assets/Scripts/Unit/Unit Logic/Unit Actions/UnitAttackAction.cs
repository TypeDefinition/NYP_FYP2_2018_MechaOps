using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class UnitAttackAction : IUnitAction
{
    // Serialised Variable(s)
    [SerializeField, Tooltip("Minimum attack range of the unit")]
    protected int m_MinAttackRange;
    [SerializeField, Tooltip("Maximum attack range of the unit")]
    protected int m_MaxAttackRange;
    [SerializeField, Tooltip("The accuracy points of the unit")]
    protected int m_AccuracyPoints;
    [SerializeField, Tooltip("The damage point it dealt")]
    protected int m_DamagePoints;

    // Non-Serialised Variable(s)
    protected UnitStats m_TargetUnitStats;

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

    /// <summary>
    /// Optimization will have to come later as this will need to be expanded upon!
    /// </summary>
    /// <param name="_other">The opposing target</param>
    /// <returns>Dont know yet!</returns>
    public override void StartAction()
    {
        base.StartAction();
    }

    public override void StopAction()
    {
        base.StopAction();
    }

    public override void PauseAction()
    {
        base.PauseAction();
    }

    public override void ResumeAction()
    {
        base.ResumeAction();
    }

    protected override void OnTurnOn()
    {
        Assert.IsTrue(VerifyRunCondition());
    }

    protected override void OnTurnOff()
    {
        base.OnTurnOff();
    }

    protected override void StartTurnCallback() {}

    protected override void EndTurnCallback() {}

    protected override void InitializeEvents() {}

    protected override void DeinitializeEvents() {}

    public override bool VerifyRunCondition()
    {
        // Exit Checks
        if (m_TargetUnitStats == null) { return false; }
        if (m_TargetUnitStats.IsAlive() == false) { return false; }
        int distanceToTarget = TileId.GetDistance(m_TargetUnitStats.CurrentTileID, GetUnitStats().CurrentTileID);
        if (distanceToTarget < MinAttackRange) { return false; }
        if (distanceToTarget > MaxAttackRange) { return false; }

        // Check if can see enemy (Our View Range as well as teammate scouting)
        if (distanceToTarget > GetUnitStats().ViewRange) { return false; }

        return true;
    }

    /// <summary>
    /// Calculate the hit percentage of this attack.
    /// It should return an int between 1(Inclusive) and 100(Inclusive).
    /// </summary>
    protected abstract int CalculateHitChance();

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
    protected void OnValidate()
    {
        MinAttackRange = m_MinAttackRange;
        MaxAttackRange = m_MaxAttackRange;
        AccuracyPoints = m_AccuracyPoints;
        DamagePoints = m_DamagePoints;
    }
#endif

}