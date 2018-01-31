using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Skip the unit's turn!
/// </summary>
public class UnitSkipAction : IUnitAction
{
    /// <summary>
    /// Just use up all of it's action points and send the event.
    /// </summary>
    public override void StartAction()
    {
        Assert.IsTrue(VerifyRunCondition(), MethodBase.GetCurrentMethod().Name + " - VerifyRunCondition() should always return true! I don't even know why I bother checking. I mean, how is it even possible to not be able to do nothing?");

        m_ActionState = ActionState.Completed;
        GameEventSystem.GetInstance().TriggerEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitFinishedAction), m_UnitStats);
        InvokeCompletionCallback();

        CheckIfUnitFinishedTurn();
    }

    protected override void OnTurnOn()
    {
        base.OnTurnOn();
        Assert.IsTrue(VerifyRunCondition());
        DeductActionPoints();
        m_UnitStats.GetGameSystemsDirectory().GetUnitActionScheduler().ScheduleAction(this);
    }

    /// <summary>
    /// This function can definitely be run no matter what.
    /// </summary>
    /// <returns></returns>
    public override bool VerifyRunCondition() { return true; }
}