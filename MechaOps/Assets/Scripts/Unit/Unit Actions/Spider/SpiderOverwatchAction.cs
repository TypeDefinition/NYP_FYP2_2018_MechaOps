using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class SpiderOverwatchAction : UnitOverwatchAction
{
    [SerializeField] protected MOAnimation_SpiderShoot m_Animation;

    protected bool m_RegisteredAnimationCompleteCallback = false;
    protected bool m_Hit = false;

    protected virtual void RegisterAnimationCompleteCallback()
    {
        if (!m_RegisteredAnimationCompleteCallback)
        {
            m_Animation.CompletionCallback += OnAnimationCompleted;
            m_RegisteredAnimationCompleteCallback = true;
        }
    }

    protected virtual void UnregisterAnimationCompleteCallback()
    {
        if (m_RegisteredAnimationCompleteCallback)
        {
            m_Animation.CompletionCallback -= OnAnimationCompleted;
            m_RegisteredAnimationCompleteCallback = false;
        }
    }

    public override void StartAction()
    {
        base.StartAction();
        RegisterAnimationCompleteCallback();
        m_Hit = CheckIfHit();
        m_Animation.Target = m_TargetUnitStats.gameObject;
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
        UnregisterAnimationCompleteCallback();
        m_Animation.StopAnimation();
    }

    protected override void OnAnimationCompleted()
    {
        m_ActionState = ActionState.Completed;
        UnregisterAnimationCompleteCallback();

        CreateDamageIndicator(m_Hit, m_DamagePoints, m_TargetUnitStats.gameObject);
        if (m_Hit) { m_TargetUnitStats.CurrentHealthPoints -= m_DamagePoints; }

        GameEventSystem.GetInstance().TriggerEvent<UnitStats, UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.AttackedUnit), m_UnitStats, m_TargetUnitStats);
        GameEventSystem.GetInstance().TriggerEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitFinishedAction), m_UnitStats);
        InvokeCompletionCallback();
        Debug.Log("Overwatch Completed!");

        // Unlike most actions, where they occur immediately, Overwatch has 2 states.
        // 1. After turning on. (End turn immediately.)
        // 2. After shooting. (Do not end turn, unless it has used up all action points.)
        if (GetUnitStats().CurrentActionPoints == 0)
        {
            CheckIfUnitFinishedTurn();
        }
    }
}