using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitGuardAction : IUnitAction {

    private bool m_EnemyTurnOver = false;

    /// <summary>
    /// This is meant for guarding!
    /// </summary>
    /// <returns></returns>
    public override void StartAction()
    {
        m_UpdateOfUnitAction = StartCoroutine(UpdateActionRoutine());
        ObserverSystemScript.Instance.StoreVariableInEvent("UnitMakeMove", gameObject);
        ObserverSystemScript.Instance.TriggerEvent("UnitMakeMove");
    }

    /// <summary>
    /// To popup some UI icons that guard mode is activated!
    /// </summary>
    /// <returns></returns>
    public override IEnumerator UpdateActionRoutine()
    {
        // Needs to at least pop up the UI icon to indicate that it is in guard mode!
        m_UpdateOfUnitAction = null;
        yield break;
    }

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

        // subscribe for turn start and turn end.
    }

    protected override void DeinitializeEvents()
    {
        throw new System.NotImplementedException();

        // unsubscribe for turn start and turn end.
    }

    public override bool VerifyRunCondition()
    {
        return !m_EnemyTurnOver;
    }

}
