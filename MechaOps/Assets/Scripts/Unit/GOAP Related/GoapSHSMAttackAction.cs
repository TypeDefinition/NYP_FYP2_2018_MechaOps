using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GoapSHSMAttackAction : GoapAttackAct {
    public override IEnumerator UpdateActRoutine()
    {
        WaitForFixedUpdate FixedWait = new WaitForFixedUpdate();
        // If there is no unit, might as well, quit this action!
        if (m_GOAPTargetAct.EnemiesInAttack.Count == 0)
        {
            // it means the unit is not able to attack it!
            m_SkipAction.CompletionCallBack += InvokeActionCompleted;
            // TODO: Have to StartAction then skip action will work!
            m_SkipAction.StartAction();
            while (!m_ActionCompleted)
                yield return FixedWait;
            m_SkipAction.CompletionCallBack -= InvokeActionCompleted;
            print("Finished Skipping action");
        }
        else
        {
            // we picked the target which will be the 1st unit in the range at GoapNearTarget
            GameObject zeTarget = m_GOAPTargetAct.EnemiesInAttack[0];
            // will need to set the gameobject of the tile since that is the requirement
            m_AttackAct.SetTarget(m_Planner.GameTileSystem.GetTile(zeTarget.GetComponent<UnitStats>().CurrentTileID).gameObject);
            m_AttackAct.CompletionCallBack += InvokeActionCompleted;
            m_AttackAct.TurnOn();
            while (!m_ActionCompleted)
                yield return FixedWait;
            m_AttackAct.CompletionCallBack -= InvokeActionCompleted;
            print("Finished Attacking");
            m_UpdateRoutine = null;
        }
        yield break;
    }

}
