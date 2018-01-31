using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitDestroyedAction : IUnitAction
{
    // Serialised Variable(s)
    [SerializeField] protected MOAnimation_UnitDestroy m_Animation;

    // Non-Serialised Variable(s)
    protected bool m_RunCondition = false;
    protected bool m_RegisteredAnimationCompleteCallback = false;

    protected virtual void RegisterAnimationCompletionCallback()
    {
        if (!m_RegisteredAnimationCompleteCallback)
        {
            m_Animation.CompletionCallback += OnAnimationCompleted;
        }
    }

    protected virtual void UnregisterAnimationCompletionCallback()
    {
        if (m_RegisteredAnimationCompleteCallback)
        {
            m_Animation.CompletionCallback -= OnAnimationCompleted;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        Assert.IsFalse(ControllableAction, MethodBase.GetCurrentMethod().Name + " - UnitDestroyedAction must be a non controllable action!");
        Assert.IsTrue(ActionCost == 0, MethodBase.GetCurrentMethod().Name + " - UnitDestroyedAction's ActionCost must be 0!");
        Assert.IsTrue(EndsTurn, MethodBase.GetCurrentMethod().Name + " - UnitDestroyedAction's ActionCost must end turn!");

        // Turn on this action immediately.
        TurnOn();
    }

    // Do not call DeductActionPoints on turn on. Call it in StartAction instead.
    protected override void OnTurnOn()
    {
        m_ActionState = ActionState.None;
        m_CooldownTurnsLeft = m_CooldownTurns;
        m_IsFirstTurnSinceTurnedOn = true;
    }

    public override void StartAction()
    {
        base.StartAction();
        DeductActionPoints();
        RegisterAnimationCompletionCallback();
        m_Animation.StartAnimation();
    }

    public override void PauseAction()
    {
        base.PauseAction();
        m_Animation.PauseAnimation();
    }

    public override void ResumeAction()
    {
        base.ResumeAction();
        m_Animation.ResumeAnimation();
    }

    public override void StopAction()
    {
        base.StopAction();
        UnregisterAnimationCompletionCallback();
        m_Animation.StopAnimation();
    }

    protected override void OnAnimationCompleted()
    {
        m_ActionState = ActionState.Completed;
        UnregisterAnimationCompletionCallback();

        // Sending out an event that this action has ended.
        GameEventSystem.GetInstance().TriggerEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitFinishedAction), m_UnitStats);
        InvokeCompletionCallback();

        CheckIfUnitFinishedTurn();
    }

    protected override void OnUnitDead(UnitStats _deadUnit, bool _dead)
    {
        if (!TurnedOn) { return; }
        if (_deadUnit != m_UnitStats) { return; }

        m_Animation.UnitVisible = _dead;
        m_RunCondition = true;
        Assert.IsTrue(VerifyRunCondition());
        m_UnitStats.GetGameSystemsDirectory().GetUnitActionScheduler().ScheduleAction(this);
    }

    public override bool VerifyRunCondition() { return m_RunCondition; }
}