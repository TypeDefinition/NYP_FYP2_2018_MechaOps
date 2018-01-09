using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public abstract class UnitActionUI : TweenUI_Scale
{
    // Action Name & Description
    [SerializeField] protected TextMeshProUGUI m_ActionNameText;
    [SerializeField] protected TextMeshProUGUI m_ActionDescriptionText;

    // Confirm and Cancel Button
    [SerializeField] protected Button m_ConfirmButton = null;
    [SerializeField] protected Button m_CancelButton = null;

    // Non-Serialised Variable(s)
    protected GameEventNames m_GameEventNames = null;

    protected virtual void InitEvents()
    {
        GameEventSystem.GetInstance().SubscribeToEvent<IUnitAction>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.SelectedAction), SetUnitAction);
    }

    protected virtual void DeinitEvents()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<IUnitAction>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.SelectedAction), SetUnitAction);
    }

    protected override void Awake()
    {
        base.Awake();

        // Make sure the player cannot pressed any other unit.
        m_GameEventNames = GameSystemsDirectory.GetSceneInstance().GetGameEventNames();

        InitEvents();
    }

    protected virtual void Start()
    {
        GameEventSystem.GetInstance().TriggerEvent<bool>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.ToggleSelectingUnit), false);
    }

    protected virtual void OnDestroy()
    {
        DeinitEvents();
    }

    protected abstract void SetUnitAction(IUnitAction _action);

    public abstract void PressedConfirm();

    public virtual void PressedCancel()
    {
        GameEventSystem.GetInstance().TriggerEvent<bool>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.ToggleSelectingUnit), true);
        Destroy(gameObject);
    }

    protected virtual void UpdateActionInfo(IUnitAction _action)
    {
        // Set the name and description.
        m_ActionNameText.text = _action.UnitActionName;

        string actionCostText = string.Format("Action Cost: {0}", _action.ActionCost);
        string endsTurnText = _action.EndsTurn ? "Ends Turn: Yes" : "Ends Turn: No";

        m_ActionDescriptionText.text = actionCostText + " " + endsTurnText + "\n" + _action.UnitActionDescription;
    }

#if UNITY_EDITOR
    protected virtual void OnValdiate()
    {
        m_TweenAnimationDuration = Mathf.Max(0.0f, m_TweenAnimationDuration);
    }
#endif
}