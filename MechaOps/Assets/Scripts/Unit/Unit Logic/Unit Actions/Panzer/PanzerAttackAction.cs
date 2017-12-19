using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Is to use the special attack helper in it's attack calculation
public class PanzerAttackAction : UnitAttackAction
{
    [SerializeField, Tooltip("Unit Attack Animation. TODO, make it more generic")]
    protected MOAnimation_PanzerAttack m_AttackAnimation;

    public override IEnumerator UpdateActionRoutine()
    {
        m_ActionState = ActionState.Running;
        // TODO: Do some complex calculation and animation for this
        GetUnitStats().CurrentActionPoints -= ActionCost;
        m_AttackAnimation.Hit = CheckIfHit();
        m_AttackAnimation.Target = m_TargetUnitStats.gameObject;
        m_AttackAnimation.CompletionCallback = OnAnimationCompleted;
        WaitForFixedUpdate zeFixedWait = new WaitForFixedUpdate();
        m_AttackAnimation.StartAnimation();
        while (!m_AnimationCompleted)
        {
            yield return zeFixedWait;
        }
        switch (m_AttackAnimation.Hit)
        {
            case true:
                m_TargetUnitStats.CurrentHealthPoints -= m_DamagePoints;
                break;
            default:
                break;
        }
        // if there is anyone calling for it, if there is no such function thr
        if (m_TargetUnitStats.m_HealthDropCallback != null)
            m_TargetUnitStats.m_HealthDropCallback.Invoke(m_UnitStats);
        // Thinking of a way to implement it
        switch (GetUnitStats().CurrentActionPoints)
        {
            case 0:
                GetUnitStats().ResetUnitStats();
                GameEventSystem.GetInstance().TriggerEvent<GameObject>("UnitMakeMove", gameObject);
                break;
            default:
                break;
        }
        GameEventSystem.GetInstance().TriggerEvent("UnitFinishAction");
        m_ActionState = ActionState.Completed;
        m_AttackAnimation.CompletionCallback -= OnAnimationCompleted;
        m_AnimationCompleted = false;
        m_UpdateOfUnitAction = null;
        if (CompletionCallBack != null)
        {
            CompletionCallBack.Invoke();
        }
        yield break;
    }

    protected int CalculateHitChance()
    {
        return 0;
    }

    protected override bool CheckIfHit()
    {
        int zeHitChance = UnitGameHelper.CalculateAttackHitChance_Tank(TileId.GetDistance(m_UnitStats.CurrentTileID, m_TargetUnitStats.CurrentTileID), MinAttackRange, MaxAttackRange, m_TargetUnitStats.EvasionPoints);
        //if (AccuracyPoints > )
        // not sure about Accuracy points. and unsure if this works
        if (zeHitChance > 50)
            return true;
        return false;
    }
}
