using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class PanzerShootAction : UnitAttackAction
{
    [SerializeField] protected MOAnimation_PanzerShoot m_Animation;

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
        StartShootingAnimation();
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

    protected void StartShootingAnimation()
    {
        m_Animation.Hit = CheckIfHit();
        m_Animation.Target = m_TargetUnitStats.gameObject;
        m_Animation.StartAnimation();
    }

    protected override int CalculateHitChance()
    {
        int distanceToTarget = TileId.GetDistance(m_TargetUnitStats.CurrentTileID, GetUnitStats().CurrentTileID);
        int optimalDistance = (MaxAttackRange - MinAttackRange) >> 1;
        float hitChance = (float)Mathf.Abs(optimalDistance - distanceToTarget) / (float)optimalDistance;
        hitChance *= 100.0f;
        hitChance -= (float)m_TargetUnitStats.EvasionPoints;
        hitChance += (float)m_AccuracyPoints;

        return Mathf.Clamp((int)hitChance, 1, 100);
    }

    protected override void OnAnimationCompleted()
    {
        m_ActionState = ActionState.Completed;
        UnregisterAnimationCompleteCallback();

        if (m_Animation.Hit) { m_TargetUnitStats.CurrentHealthPoints -= m_DamagePoints; }
        // Invoke the Target's Unit Stat's HealthDropCallback.
        if (m_TargetUnitStats.m_HealthDropCallback != null) { m_TargetUnitStats.m_HealthDropCallback(m_UnitStats); }

        GameEventSystem.GetInstance().TriggerEvent("UnitFinishAction");
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
        m_UnitStats.GetGameSystemsDirectory().GetUnitActionScheduler().ScheduleAction(this);
    }
}