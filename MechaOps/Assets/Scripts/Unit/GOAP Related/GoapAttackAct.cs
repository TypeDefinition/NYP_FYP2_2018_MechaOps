using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoapAttackAct : IGoapAction {
    [Header("References for GoapAttackAct")]
    [SerializeField, Tooltip("The attack action")]
    protected UnitAttackAction m_AttackAct;
    [SerializeField, Tooltip("It needs the GoapNearTarget")]
    protected GoapNearTarget m_GOAPTargetAct;
    [SerializeField, Tooltip("Walk action that needs to be refactored soon")]
    protected UnitMoveAction m_MoveAct;

    protected override void Start()
    {
        base.Start();
        if (!m_GOAPTargetAct)
            m_GOAPTargetAct = GetComponent<GoapNearTarget>();
    }

    public override void DoAction()
    {
        if (!m_MoveAct)
        {
            m_MoveAct = GetComponent<UnitMoveAction>();
        }
        m_UpdateRoutine = StartCoroutine(UpdateActRoutine());
    }

    public override IEnumerator UpdateActRoutine()
    {
        // If there is no unit, might as well, quit this action!
        if (m_GOAPTargetAct.EnemiesInAttack.Count == 0)
            yield break;
        // we picked the target which will be the 1st unit in the range at GoapNearTarget
        GameObject zeTarget = m_GOAPTargetAct.EnemiesInAttack[0];
        // TODO: resolve this quick fix
        // get the actual tile distance
        //int zeTileDistance = TileId.GetDistance(zeTarget.GetComponent<UnitStats>().CurrentTileID, m_Planner.m_Stats.CurrentTileID);
        //// if it is not within the attack range
        //if (m_AttackAct.MaxAttackRange < zeTileDistance)
        //{
        //    // get the tiles furthest away from the unit
            
        //m_Planner.remove
        //}
        //else
        {
            m_AttackAct.SetTarget(zeTarget);
            m_AttackAct.TurnOn();
            WaitForFixedUpdate zeFixedWait = new WaitForFixedUpdate();
            bool zeWaitForComplete = false;
            m_AttackAct.CompletionCallBack += () => zeWaitForComplete = true;
            while (!zeWaitForComplete)
                yield return zeFixedWait;
            m_AttackAct.CompletionCallBack = null;
        }
        print("Finished Attacking");
        m_UpdateRoutine = null;
        yield break;
    }
}
