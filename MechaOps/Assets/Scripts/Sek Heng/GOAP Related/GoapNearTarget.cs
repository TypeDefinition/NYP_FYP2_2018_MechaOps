using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// So that the AI will be attack the opposing unit in range.
/// It will move near to the viewed unit. Hopefully it works
/// </summary>
public class GoapNearTarget : IGoapAction
{
    [Header("Variables required for GoapNearTarget")]
    [SerializeField, Tooltip("Unit Attack Action reference")]
    protected UnitAttackAction m_AttackAct;

    [Header("Debugging purpose")]
    [SerializeField, Tooltip("The list of units that are within the attackin range")]
    protected List<GameObject> m_EnemiesInAttack = new List<GameObject>();

    public List<GameObject> EnemiesInAttack
    {
        get
        {
            return m_EnemiesInAttack;
        }
    }

    protected override void Start()
    {
        base.Start();
        if (!m_AttackAct)
            m_AttackAct = GetComponent<UnitAttackAction>();
    }

    public override void DoAction()
    {
        UpdateEnemyInAttack();
        m_UpdateRoutine = StartCoroutine(UpdateActRoutine());
    }

    public override IEnumerator UpdateActRoutine()
    {
        if (m_EnemiesInAttack.Count > 0)
            yield break;
        // we will need to check through the units that are in range then move near to that unit!
        print("Tracked the target successfully");
        yield break;
    }

    /// <summary>
    ///  Overriding this to check if there are any units within the target range!
    ///  If there are already units within this range, then the cost is 0
    /// </summary>
    /// <returns></returns>
    public override int GetCost()
    {
        UpdateEnemyInAttack();
        if (m_EnemiesInAttack.Count > 0)
            return 0;
        return m_Cost;
    }

    protected virtual void UpdateEnemyInAttack()
    {
        m_EnemiesInAttack.Clear();
        foreach (GameObject zeGO in m_Planner.m_Stats.EnemyInRange)
        {
            if (Vector3.Distance(transform.position, zeGO.transform.position) <= m_AttackAct.MaxAttackRange)
                m_EnemiesInAttack.Add(zeGO);
        }
        if (m_EnemiesInAttack.Count > 0)
            m_Planner.m_CurrentStates.Add("TargetAttackInRange");
        else
            m_Planner.m_CurrentStates.Remove("TargetAttackInRange");
    }

#if UNITY_EDITOR
    protected void OnDrawGizmos()
    {
        if (m_AttackAct)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_AttackAct.MaxAttackRange);
        }
    }
#endif
}
