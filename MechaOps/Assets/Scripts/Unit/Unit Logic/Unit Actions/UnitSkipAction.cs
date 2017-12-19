using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mainly to skip the unit's turn!
/// </summary>
public class UnitSkipAction : IUnitAction {

    /// <summary>
    /// Just use up all of it's action points and send the event
    /// </summary>
    public override void StartAction()
    {
        GetUnitStats().CurrentActionPoints -= GetUnitStats().MaxActionPoints;
        GameEventSystem.GetInstance().TriggerEvent<GameObject>("UnitMakeMove", gameObject);
        GameEventSystem.GetInstance().TriggerEvent("UnitFinishAction");
        GetUnitStats().ResetUnitStats();
        m_ActionState = ActionState.Completed;
        if (CompletionCallBack != null)
        {
            CompletionCallBack.Invoke();
        }
    }

    /// <summary>
    /// This function can definitely be run unless the unit has run out of unit action points!
    /// </summary>
    /// <returns></returns>
    public override bool VerifyRunCondition()
    {
        // need to make sure there are more than 1 action points. otherwise this is definitely a bug!
        switch (m_UnitStats.CurrentActionPoints)
        {
            case 0:
                return false;
            default:
                break;
        }
        return true;
    }

    protected override void DeinitializeEvents()
    {
    }

    protected override void EndTurnCallback()
    {
    }

    protected override void InitializeEvents()
    {
    }

    protected override void StartTurnCallback()
    {
    }
}
