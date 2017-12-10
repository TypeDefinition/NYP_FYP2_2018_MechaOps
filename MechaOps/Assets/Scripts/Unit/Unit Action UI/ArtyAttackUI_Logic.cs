using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The logic for artillery attack UI
/// </summary>
public class ArtyAttackUI_Logic : TweenUI_Scale {
    [Header("Debugging for ArtyAttackUI")]
    [SerializeField, Tooltip("Unit's attack action")]
    protected UnitAttackAction m_AttckAct;

    private void OnEnable()
    {
        // Animate the UI when enabled
        AnimateUI();

        GameEventSystem.GetInstance().TriggerEvent("ToggleSelectingUnit");
        GameEventSystem.GetInstance().SubscribeToEvent<IUnitAction>("SelectedAction", PressedAction);
    }

    private void OnDisable()
    {
        
    }



    /// <summary>
    /// When pressing the back button
    /// </summary>
    public void Back()
    {
        // ensure that the player will be able to click on unit again!
        GameEventSystem.GetInstance().TriggerEvent("ToggleSelectingUnit");
        // there is no point in keeping this UI anymore so destroy it!
        Destroy(gameObject);
    }

    public void ConfirmAttacl()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// To get the event message from the GameEventSystem
    /// </summary>
    /// <param name="_act"></param>
    public void PressedAction(IUnitAction _act)
    {
        m_AttckAct = _act as UnitAttackAction;
    }
}
