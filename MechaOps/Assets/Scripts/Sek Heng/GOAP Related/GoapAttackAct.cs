using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoapAttackAct : IGoapAction {
    [Header("References for GoapAttackAct")]
    [SerializeField, Tooltip("The attack action")]
    protected UnitAttackAction m_AttackAct;
    [SerializeField, Tooltip("It needs the GoapNearTarget")]
    protected GoapNearTarget m_GOAPTargetAct;

    protected override void Start()
    {
        base.Start();
        if (!m_GOAPTargetAct)
            m_GOAPTargetAct = GetComponent<GoapNearTarget>();
    }

    public override void DoAction()
    {
        m_UpdateRoutine = StartCoroutine(UpdateActRoutine());
    }

    public override IEnumerator UpdateActRoutine()
    {
        // If there is no unit, might as well, quit this action!
        if (m_GOAPTargetAct.EnemiesInAttack.Count == 0)
            yield break;
        // we picked the target which will be the 1st unit in the range at GoapNearTarget
        GameObject zeTarget = m_GOAPTargetAct.EnemiesInAttack[0];
        m_AttackAct.SetTarget(zeTarget);
        UnitActionScheduler zeActScheduler = FindObjectOfType<UnitActionScheduler>();
        m_AttackAct.TurnOn();
        zeActScheduler.ScheduleAction(m_AttackAct);
        WaitForFixedUpdate zeFixedWait = new WaitForFixedUpdate();
        while (m_AttackAct.GetActionState() != IUnitAction.ActionState.Completed &&  m_AttackAct.GetActionState() != IUnitAction.ActionState.None)
            yield return zeFixedWait;
        print("Finished Attacking");
        yield break;
    }

    /// <summary>
    ///  To put in the preconditions required!
    /// </summary>
    public override void CheckCurrentState()
    {
        // If there is no target, go according to where those targets are!
        if (!m_AttackAct.GetTargetUnitStats())
        {

        }
    }
}
