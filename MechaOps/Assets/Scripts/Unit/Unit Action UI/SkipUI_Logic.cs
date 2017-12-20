using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Should be used for skipping the unit's action
/// </summary>
public class SkipUI_Logic : MonoBehaviour
{
    [Header("Variables needed")]
    [SerializeField, Tooltip("The animation time for scaling of X")]
    protected float m_AnimationTime = 0.3f;
    [Header("Debugging")]
    [SerializeField, Tooltip("The unit's skip action")]
    protected UnitSkipAction m_UnitSkipAct;

    // Action Name & Description
    [SerializeField] protected TextMeshProUGUI m_ActionNameText;
    [SerializeField] protected TextMeshProUGUI m_ActionDescriptionText;

    private void OnEnable()
    {
        Vector3 zeScale = transform.localScale;
        float zeOriginalScaleX = zeScale.x;
        zeScale.x = 0;
        transform.localScale = zeScale;
        LeanTween.scaleX(gameObject, zeOriginalScaleX, m_AnimationTime);
        // making sure the player cannot pressed any other unit
        GameEventSystem.GetInstance().TriggerEvent("ToggleSelectingUnit");
        GameEventSystem.GetInstance().SubscribeToEvent<IUnitAction>("SelectedAction", SetUnitAction);
    }

    private void OnDisable()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<IUnitAction>("SelectedAction", SetUnitAction);
    }

    public void PressedConfirm()
    {
        UnitActionScheduler zeActScheduler = FindObjectOfType<UnitActionScheduler>();
        m_UnitSkipAct.TurnOn();
        zeActScheduler.ScheduleAction(m_UnitSkipAct);
        Destroy(gameObject);
    }

    /// <summary>
    /// Pressing no is equivalent to back button
    /// </summary>
    public void PressedCancel()
    {
        GameEventSystem.GetInstance().TriggerEvent("ToggleSelectingUnit");
        Destroy(gameObject);
    }

    public void SetUnitAction(IUnitAction _action)
    {
        m_UnitSkipAct = (UnitSkipAction)_action;

        // Set the name and description.
        m_ActionNameText.text = _action.UnitActionName;
        m_ActionDescriptionText.text = _action.UnitActionDescription;
    }
}
