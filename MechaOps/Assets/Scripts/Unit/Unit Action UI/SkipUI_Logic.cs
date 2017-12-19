using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Should be used for skipping the unit's action
/// </summary>
public class SkipUI_Logic : MonoBehaviour {
    [Header("Variables needed")]
    [SerializeField, Tooltip("The animation time for scaling of X")]
    protected float m_AnimTime = 0.3f;
    [Header("Debugging")]
    [SerializeField, Tooltip("The unit's skip action")]
    protected UnitSkipAction m_UnitSkipAct;

    private void OnEnable()
    {
        Vector3 zeScale = transform.localScale;
        float zeOriginalScaleX = zeScale.x;
        zeScale.x = 0;
        transform.localScale = zeScale;
        LeanTween.scaleX(gameObject, zeOriginalScaleX, m_AnimTime);
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
        m_UnitSkipAct = (UnitSkipAction)_action;
    }
}
