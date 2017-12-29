using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GoapSHSMAttackAction : GoapAttackAct {
    public override IEnumerator UpdateActRoutine()
    {
        // If there is no unit, might as well, quit this action!
        Assert.IsFalse(m_GOAPTargetAct.EnemiesInAttack.Count == 0, "GoapSHSMAttackAction is having bug as there is no enemies");
        // we picked the target which will be the 1st unit in the range at GoapNearTarget
        GameObject zeTarget = m_GOAPTargetAct.EnemiesInAttack[0];
        // will need to set the gameobject of the tile since that is the requirement
        m_AttackAct.SetTarget(m_Planner.GameTileSystem.GetTile(zeTarget.GetComponent<UnitStats>().CurrentTileID).gameObject);
        m_AttackAct.TurnOn();
        WaitForFixedUpdate FixedWait = new WaitForFixedUpdate();
        m_AttackAct.CompletionCallBack += InvokeActionCompleted;
        while (!m_ActionCompleted)
            yield return FixedWait;
        m_AttackAct.CompletionCallBack -= InvokeActionCompleted;
        print("Finished Attacking");
        m_UpdateRoutine = null;
        yield break;
    }

}
