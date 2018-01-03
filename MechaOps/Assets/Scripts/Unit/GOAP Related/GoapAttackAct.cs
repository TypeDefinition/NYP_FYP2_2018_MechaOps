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
    [SerializeField, Tooltip("Skip actiopn")]
    protected UnitSkipAction m_SkipAction;

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
        if (!m_SkipAction)
        {
            m_SkipAction = GetComponent<UnitSkipAction>();
        }
        m_ActionCompleted = false;
        m_UpdateRoutine = StartCoroutine(UpdateActRoutine());
    }

    public override IEnumerator UpdateActRoutine()
    {
        WaitForFixedUpdate FixedWait = new WaitForFixedUpdate();
        // If there is no unit, might as well, quit this action!
        if (m_GOAPTargetAct.EnemiesInAttack.Count == 0)
        {
            // it means the unit is not able to attack it!
            m_SkipAction.CompletionCallBack += InvokeActionCompleted;
            m_SkipAction.TurnOn();
            while (!m_ActionCompleted)
                yield return FixedWait;
            m_SkipAction.CompletionCallBack -= InvokeActionCompleted;
        }
        // we picked the target which will be the 1st unit in the range at GoapNearTarget
        // TODO: resolve this quick fix
        // get the actual tile distance
        //int zeTileDistance = TileId.GetDistance(zeTarget.GetComponent<UnitStats>().CurrentTileID, m_Planner.m_Stats.CurrentTileID);
        //// if it is not within the attack range
        //if (m_AttackAct.MaxAttackRange < zeTileDistance)
        //{
        //    // get the tiles furthest away from the unit

        //m_Planner.remove
        //}
        else
        {
            GameObject zeTarget = m_GOAPTargetAct.EnemiesInAttack[0];
            m_AttackAct.SetTarget(zeTarget);
            m_AttackAct.CompletionCallBack += InvokeActionCompleted;
            m_AttackAct.TurnOn();
            while (!m_ActionCompleted)
                yield return FixedWait;
            m_AttackAct.CompletionCallBack -= InvokeActionCompleted;
        }
        print("Finished Attacking");
        m_UpdateRoutine = null;
        yield break;
    }
}
