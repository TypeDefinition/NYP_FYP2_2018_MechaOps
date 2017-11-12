using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoapAttackAct : IGoapAction {
    [Header("References for GoapAttackAct")]
    [SerializeField, Tooltip("The attack action")]
    protected UnitAttackAction m_AttackAct;

    public override void DoAction()
    {
        m_UpdateRoutine = StartCoroutine(UpdateActRoutine());
    }

    public override IEnumerator UpdateActRoutine()
    {
        print("Finished Attacking");
        yield return new WaitForSeconds(1.5f);
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
