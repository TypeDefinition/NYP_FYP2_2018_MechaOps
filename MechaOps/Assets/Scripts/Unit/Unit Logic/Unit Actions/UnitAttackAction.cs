using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitAttackAction : IUnitAction
{
    [SerializeField, Tooltip("Minimum attack range of the unit")]
    protected int m_MinAttackRange;
    [SerializeField, Tooltip("Maximum attack range of the unit")]
    protected int m_MaxAttackRange;
    [SerializeField, Tooltip("The accuracy points of the unit")]
    protected int m_AccuracyPoints;
    [SerializeField, Tooltip("The damage point it dealt")]
    protected int m_DamagePoints;

    [Header("These variables are shown in the Inspector for debugging purposes.")]

    [SerializeField, Tooltip("The unit stats of the target")]
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
        m_UpdateOfUnitAction = StartCoroutine(UpdateActionRoutine());
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
        // m_ActionScheduler.ScheduleAction(this);
    }

    protected override void OnTurnOff()
    {
        base.OnTurnOff();
    }

    // Use this for initialization
    void Start ()
    {
        Assert.IsTrue(m_UnitActionName != null, MethodBase.GetCurrentMethod().Name + " - m_UnitActionName is null!");
    }

    protected override void StartTurnCallback()
    {
    }

    protected override void EndTurnCallback()
    {
    }

    protected override void InitializeEvents()
    {
    }

    protected override void DeinitializeEvents()
    {
    }

    public override bool VerifyRunCondition()
    {
        // Exit Checks
        if (m_TargetUnitStats == null) { return false; }
        if (m_TargetUnitStats.IsAlive() == false) { return false; }
        int distanceToTarget = TileId.GetDistance(m_TargetUnitStats.CurrentTileID, GetUnitStats().CurrentTileID);
        if (distanceToTarget > m_MaxAttackRange) { return false; }

        // Check if can see enemy (Our View Range as well as teammate scouting)

        return true;
    }

    /// <summary>
    /// This will always be true regardless since there is nothing to see the accuracy
    /// </summary>
    /// <returns></returns>
    protected virtual bool CheckIfHit()
    {
        return true;
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