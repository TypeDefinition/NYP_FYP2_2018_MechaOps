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
    [SerializeField, Tooltip("Unit walk act ref")]
    protected UnitWalkAction m_WalkAct;

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
        if (!m_WalkAct)
            m_WalkAct = GetComponent<UnitWalkAction>();
    }

    protected virtual void OnEnable()
    {
        // Since thr will only be player units to fight against, we will only wait for player unit died
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("PlayerIsDead", EnemyUnitDied);
    }
    protected virtual void OnDisable()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("PlayerIsDead", EnemyUnitDied);
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
        // this got to be the most headache part
        // TODO: Improve this part as we dont have time to do some of the amazing AI and stick to this shitty AI instead
        // We will get the shortest path to the 1st player unit
        TileSystem zeTileSys = FindObjectOfType<TileSystem>();
        // Maybe we can randomize but we will just get the 1st unit!
        UnitStats zeEnemyStat = m_Planner.m_Stats.EnemyInRange[0].GetComponent<UnitStats>();
        TileId[] zePathToEnemy = zeTileSys.GetPath(999, m_Planner.m_Stats.CurrentTileID, zeEnemyStat.CurrentTileID, m_Planner.m_Stats.GetTileAttributeOverrides());
        // But we will just walk the shortest length of tile to get to the m_EnemyState. Maybe when there is Accuracy point then it will be added in!
        List<TileId> zeTileToWalk = new List<TileId>();
        yield return null;
        foreach (TileId zeTile in zePathToEnemy)
        {
            zeTileToWalk.Add(zeTile);
            // Once that supposed tile is good enough for this unit to attack the enemy!
            if (Vector3.Distance(zeTileSys.GetTile(zeTile).transform.position, zeEnemyStat.transform.position) <= m_AttackAct.MaxAttackRange)
            {
                break;
            }
        }
        yield return null;
        m_WalkAct.m_TilePath = zeTileToWalk.ToArray();
        UnitActionScheduler zeScheduler = FindObjectOfType<UnitActionScheduler>();
        m_WalkAct.TurnOn();
        zeScheduler.ScheduleAction(m_WalkAct);
        WaitForFixedUpdate zeFixedWait = new WaitForFixedUpdate();
        while (m_WalkAct.GetActionState() != IUnitAction.ActionState.Completed)
            yield return zeFixedWait;
        UpdateEnemyInAttack();
        print("Followed the target successfully");
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
            //if (Vector3.Distance(transform.position, zeGO.transform.position) <= m_AttackAct.MaxAttackRange)
            UnitStats zeGoStat = zeGO.GetComponent<UnitStats>();
            int zeTileDist = TileId.GetDistance(zeGoStat.CurrentTileID, m_Planner.m_Stats.CurrentTileID);
            if (zeTileDist <= m_AttackAct.MaxAttackRange && zeTileDist >= m_AttackAct.MinAttackRange)
                m_EnemiesInAttack.Add(zeGO);
        }
        if (m_EnemiesInAttack.Count > 0)
            m_Planner.m_StateData.CurrentStates.Add("TargetAttackInRange");
        else
            m_Planner.m_StateData.CurrentStates.Remove("TargetAttackInRange");
    }

    protected void EnemyUnitDied(GameObject _deadUnit)
    {
        m_EnemiesInAttack.Remove(_deadUnit);
    }
}
