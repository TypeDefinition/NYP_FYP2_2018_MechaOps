using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Should be used for skipping the unit's action
/// </summary>
public class UnitActionUI_Skip : UnitActionUI
{
    protected UnitSkipAction m_UnitAction;

    public override void PressedConfirm()
    {
        m_UnitAction.TurnOn();
        GameEventSystem.GetInstance().TriggerEvent(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitStartAction));
        Destroy(gameObject);
    }

    protected override void SetUnitAction(IUnitAction _action)
    {
        m_UnitAction = (UnitSkipAction)_action;
        UpdateActionInfo(_action);
    }
}