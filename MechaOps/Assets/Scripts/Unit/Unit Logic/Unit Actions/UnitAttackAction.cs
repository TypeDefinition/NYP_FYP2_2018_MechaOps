﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitAttackAction : IUnitAction
{
    [Header("The References and variables needed for Unit Attack")]
    [SerializeField, Tooltip("Minimum attack range of the unit")]
    private int m_MinAttackRange;
    [SerializeField, Tooltip("Maximum attack range of the unit")]
    private int m_MaxAttackRange;
    [SerializeField, Tooltip("The accuracy points of the unit")]
    private int m_AccuracyPoints;
    [SerializeField, Tooltip("The damage point it dealt")]
    private int m_DamagePoints;

    [SerializeField, Tooltip("The unit stat of the target")]
    private UnitStats m_TargetUnitStats;

    public int MinAttackRange {
        get { return m_MinAttackRange; }
        set { m_MinAttackRange = Mathf.Clamp(value, 0, MaxAttackRange); }
    }

    public int MaxAttackRange {
        get
        {
            return m_MaxAttackRange;
        }
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

    public void SetTarget(GameObject _target)
    {
        Assert.IsTrue(_target != null, MethodBase.GetCurrentMethod().Name + " - _target is null!");
        m_TargetUnitStats = _target.GetComponent<UnitStats>();
        Assert.IsTrue(m_TargetUnitStats != false, MethodBase.GetCurrentMethod().Name + " - _target has no UnitStats!");
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

    /// <summary>
    /// To do raycasting and calculation. Along with the animation required.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator UpdateActionRoutine()
    {
        m_ActionState = ActionState.Running;
        // TODO: Do some complex calculation and animation for this
        m_TargetUnitStats.CurrentHealthPoints -= m_DamagePoints;
        // Thinking of a way to implement it
        m_UpdateOfUnitAction = null;
        --GetUnitStats().CurrentActionPoints;
        switch (GetUnitStats().CurrentActionPoints)
        {
            case 0:
                GetUnitStats().ResetUnitStat();
                ObserverSystemScript.Instance.StoreVariableInEvent("UnitMakeMove", gameObject);
                ObserverSystemScript.Instance.TriggerEvent("UnitMakeMove");
                break;
            default:
                break;
        }
        ObserverSystemScript.Instance.TriggerEvent("UnitFinishAction");
        m_ActionState = ActionState.Completed;
        yield break;
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
        if (m_TargetUnitStats == null)
        {
            return false;
        }

        if (m_TargetUnitStats.IsAlive() == false)
        {
            return false;
        }

        int distanceToTarget = TileId.GetDistance(m_TargetUnitStats.CurrentTileID, GetUnitStats().CurrentTileID);
        if (distanceToTarget > m_MaxAttackRange)
        {
            return false;
        }

        // Check if can see enemy (Our View Range as well as teammate scouting)

        return true;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        MinAttackRange = m_MinAttackRange;
        MaxAttackRange = m_MaxAttackRange;
        AccuracyPoints = m_AccuracyPoints;
        DamagePoints = m_DamagePoints;
    }
#endif

}