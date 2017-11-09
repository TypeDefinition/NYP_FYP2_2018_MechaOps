using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitOverwatchAction : UnitAttackAction
{
    private bool m_EnemyTurnOver = false;

    // Use this for initialization
    void Start ()
    {
        Assert.IsTrue(m_UnitActionName != null, MethodBase.GetCurrentMethod().Name + " - m_UnitActionName is null!");
    }

    protected override void OnTurnOn()
    {
        m_EnemyTurnOver = false;
    }

    protected override void OnTurnOff()
    {
        m_EnemyTurnOver = false;
    }

    public override void StartAction()
    {
        throw new System.NotImplementedException();
    }

    protected virtual void OnUnitMoveCallback(/*UnitMovementData*/)
    {
        // Check if unit is enemy.
        // Check if enemy is seen.
    }

    protected override void StartTurnCallback()
    {
        throw new System.NotImplementedException();

        // If PlayerTurnStart,
        m_EnemyTurnOver = false;
    }

    protected override void EndTurnCallback()
    {
        throw new System.NotImplementedException();

        // If EnemyTurnOver,
        m_EnemyTurnOver = true;
    }

    protected override void InitializeEvents()
    {
        throw new System.NotImplementedException();

        // Subscribe to turn start event.
        // Subscribe to unit move event.
    }

    protected override void DeinitializeEvents()
    {
        throw new System.NotImplementedException();

        // unsubscribe to turn start event.
        // unsubscribe to unit move event.
    }

    public override bool VerifyRunCondition()
    {
        throw new System.NotImplementedException();

        // Check if can see enemy (Our View Range as well as teammate scouting)

        return false;
    }
}
