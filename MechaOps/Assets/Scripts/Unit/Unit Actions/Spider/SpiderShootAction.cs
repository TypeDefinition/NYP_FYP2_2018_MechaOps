using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class SpiderShootAction : UnitAttackAction
{
    [SerializeField] protected MOAnimation_SpiderShoot m_Animation;

    protected bool m_Hit = false;
    protected bool m_RegisteredAnimationCompleteCallback = false;

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

    public override int CalculateHitChance()
    {
        return Mathf.Clamp(base.CalculateHitChance() + 10, 1, 100);
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

        CheckIfUnitFinishedTurn();
    }

    // Even though we check Assert.IsTrue(VerifyRunCondition()); here,
    // This is not the case for ALL actions. For an action like overwatch,
    // it is perfectly okay for VerifyRunCondition() to return false, since we are not
    // shooting any enemy now. Rather, we are WAITING for some point in time in the future
    // when VerifyRunCondition() returns true. It is also possible for an action like Overwatch
    // to never excecute because no enemies walked into the attack range of the unit.
    protected override void OnTurnOn()
    {
        base.OnTurnOn();
        Assert.IsTrue(VerifyRunCondition());
        DeductActionPoints();
        m_UnitStats.GetGameSystemsDirectory().GetUnitActionScheduler().ScheduleAction(this);
    }
}