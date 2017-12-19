using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanzerShootAction : UnitAttackAction
{
    protected MOAnimation_PanzerAttack m_AttackAnimation;

    protected bool m_RegisteredAnimationCompleteCallback = false;

    protected virtual void RegisterAnimationCompleteCallback()
    {
        if (!m_RegisteredAnimationCompleteCallback)
        {
            m_AttackAnimation.CompletionCallback += OnAnimationCompleted;
            m_RegisteredAnimationCompleteCallback = true;
        }
    }

    protected virtual void UnregisterAnimationCompleteCallback()
    {
        if (m_RegisteredAnimationCompleteCallback)
        {
            m_AttackAnimation.CompletionCallback -= OnAnimationCompleted;
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
        m_AttackAnimation.PauseAnimation();
    }

    public override void ResumeAction()
    {
        base.ResumeAction();
        m_AttackAnimation.ResumeAnimation();
    }

    public override void StopAction()
    {
        base.StopAction();
        UnregisterAnimationCompleteCallback();
        m_AttackAnimation.StopAnimation();
    }

    protected void StartShootingAnimation()
    {
        m_AttackAnimation.Hit = CheckIfHit();
        m_AttackAnimation.Target = m_TargetUnitStats.gameObject;
        m_AttackAnimation.StartAnimation();
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

        if (m_AttackAnimation.Hit) { m_TargetUnitStats.CurrentHealthPoints -= m_DamagePoints; }
        // Invoke the Target's Unit Stat's HealthDropCallback.
        if (m_TargetUnitStats.m_HealthDropCallback != null) { m_TargetUnitStats.m_HealthDropCallback(m_UnitStats); }

        GameEventSystem.GetInstance().TriggerEvent("UnitFinishAction");
        InvokeCompletionCallback();

        CheckIfUnitFinishedTurn();
    }
}