using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitDestroyedAction : IUnitAction
{
    [SerializeField] protected MOAnimation_UnitDestroy m_Animation;

    bool m_RunCondition = false;
    bool m_RegisteredAnimationCompleteCallback = false;
    bool m_EventsInitialized = false;

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

    protected override void InitializeEvents()
    {
        if (m_EventsInitialized) { return; }
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject, bool>(tag + "IsDead", OnUnitDestroyed);
        m_EventsInitialized = true;
    }

    protected override void DeinitializeEvents()
    {
        if (!m_EventsInitialized) { return; }
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject, bool>(tag + "IsDead", OnUnitDestroyed);
        m_EventsInitialized = false;
    }

    protected override void Awake()
    {
        base.Awake();
        Assert.IsFalse(ControllableAction, MethodBase.GetCurrentMethod().Name + " - UnitDestroyedAction must be a non controllable action!");
        Assert.IsTrue(ActionCost == 0, MethodBase.GetCurrentMethod().Name + " - UnitDestroyedAction's ActionCost must be 0!");

        // Turn on this action immediately.
        TurnOn();
    }

    protected override void OnTurnOn()
    {
        base.OnTurnOn();
        InitializeEvents();
    }

    protected override void OnTurnOff()
    {
        base.OnTurnOff();
        DeinitializeEvents();
    }

    protected virtual void OnDestroy() { DeinitializeEvents(); }

    public override void StartAction()
    {
        base.StartAction();
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
        InvokeCompletionCallback();
    }

    public void OnUnitDestroyed(GameObject _go, bool _destroyedUnitVisible)
    {
        if (_go != gameObject) { return; }

        DeinitializeEvents();
        m_Animation.UnitVisible = _destroyedUnitVisible;
        m_RunCondition = true;
        Assert.IsTrue(VerifyRunCondition());
        m_UnitStats.GetGameSystemsDirectory().GetUnitActionScheduler().ScheduleAction(this);
    }

    public override bool VerifyRunCondition() { return m_RunCondition; }

    protected override void StartTurnCallback() { throw new System.NotImplementedException(); }

    protected override void EndTurnCallback() { throw new System.NotImplementedException(); }
}