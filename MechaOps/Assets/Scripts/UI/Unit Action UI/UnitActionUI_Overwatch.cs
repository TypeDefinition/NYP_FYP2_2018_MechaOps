using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionUI_Overwatch : UnitActionUI
{
    protected UnitOverwatchAction m_UnitAction;

    public override void PressedConfirm()
    {
        m_UnitAction.TurnOn();
        GameEventSystem.GetInstance().TriggerEvent(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitStartAction));
        Destroy(gameObject);
    }

    protected override void SetUnitAction(IUnitAction _action)
    {
        m_UnitAction = (UnitOverwatchAction)_action;
        UpdateActionInfo(_action);
    }
}