using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Should be used for skipping the unit's action
/// </summary>
public class SkipUI_Logic : MonoBehaviour {
    [Header("Debugging")]
    [SerializeField, Tooltip("The unit's skip action")]
    protected UnitSkipAct m_UnitSkipAct;

    private void OnEnable()
    {
        // making sure the player cannot pressed any other unit
        GameEventSystem.GetInstance().TriggerEvent("ToggleSelectingUnit");
        GameEventSystem.GetInstance().SubscribeToEvent<IUnitAction>("SelectedAction", PressedAction);
    }

    private void OnDisable()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<IUnitAction>("SelectedAction", PressedAction);
    }

    public void PressedYes()
    {
        UnitActionScheduler zeActScheduler = FindObjectOfType<UnitActionScheduler>();
        m_UnitSkipAct.TurnOn();
        zeActScheduler.ScheduleAction(m_UnitSkipAct);
        Destroy(gameObject);
    }

    /// <summary>
    /// Pressing no is equivalent to back button
    /// </summary>
    public void PressedNo()
    {
        GameEventSystem.GetInstance().TriggerEvent("ToggleSelectingUnit");
        Destroy(gameObject);
    }

    public void PressedAction(IUnitAction _action)
    {
        m_UnitSkipAct = (UnitSkipAct)_action;
    }
}
