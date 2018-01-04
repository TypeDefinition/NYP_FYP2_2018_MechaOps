using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
    protected UnitMoveAction m_WalkAct;

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
            m_WalkAct = GetComponent<UnitMoveAction>();
    }

    protected virtual void OnEnable()
    {
        // Since thr will only be player units to fight against, we will only wait for player unit died
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject, bool>("PlayerIsDead", EnemyUnitDestroyed);
        if (!m_Planner)
            m_Planner = GetComponent<GoapPlanner>();
        m_Planner.m_CallbackStartPlan += UpdateEnemyInAttack;
    }
    protected virtual void OnDisable()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject, bool>("PlayerIsDead", EnemyUnitDestroyed);
        if (m_Planner)
            m_Planner.m_CallbackStartPlan -= UpdateEnemyInAttack;
    }

    public override void DoAction()
    {
        m_ActionCompleted = false;
        UpdateEnemyInAttack();
        m_UpdateRoutine = StartCoroutine(UpdateActRoutine());
    }

    public override IEnumerator UpdateActRoutine()
    {
        Assert.IsTrue(m_EnemiesInAttack.Count == 0, "GoapNearTarget has a bug at UpdateActRoutine");
        // we will need to check through the units that are in range then move near to that unit!
        // this got to be the most headache part
        // TODO: Improve this part as we dont have time to do some of the amazing AI and stick to this shitty AI instead
        // We will get the shortest path to the 1st player unit
        int zeEnemyIndex = 0;
        // Maybe we can randomize but we will just get the 1st unit!
        UnitStats zeEnemyStat = m_Planner.EnemiesManager.GetSeenEnemies()[zeEnemyIndex].GetComponent<UnitStats>();
        TileId zeDestinationTileID = zeEnemyStat.CurrentTileID;
        Tile EnemyTile = m_Planner.GameTileSystem.GetTile(zeDestinationTileID);
        Tile DestTile = m_Planner.GameTileSystem.GetTile(zeDestinationTileID);
        // we will get the surrounding tiles and check whether they are available! 
        TileId[] zeTiles = m_Planner.GameTileSystem.GetSurroundingTiles(zeEnemyStat.CurrentTileID, m_AttackAct.MaxAttackRange);
        TileId[] zePathToEnemy = null;
        while (DestTile.HasUnit() || !DestTile.GetIsWalkable())
        {
            foreach (TileId zeTileCheck in zeTiles)
            {
                zeDestinationTileID = zeTileCheck;
                DestTile = m_Planner.GameTileSystem.GetTile(zeDestinationTileID);
                // check is it can be walked as well as getting a direct target to there!
                if (DestTile.GetIsWalkable() && !DestTile.HasUnit() && m_Planner.m_Stats.GetViewScript().RaycastToTile(EnemyTile, DestTile))
                {
                    // find the path to the enemy 
                    zePathToEnemy = m_Planner.GameTileSystem.GetPath(999, m_Planner.m_Stats.CurrentTileID, zeDestinationTileID, m_Planner.m_Stats.GetTileAttributeOverrides());
                    if (zePathToEnemy != null)
                    {
                        break;
                    }
                }
            }
            // if still cannot find any tile
            if (DestTile.HasUnit() || !DestTile.GetIsWalkable())
            {
                // Then we will have to another target!
                zeEnemyStat = m_Planner.EnemiesManager.GetSeenEnemies()[++zeEnemyIndex].GetComponent<UnitStats>();
            }
        }
        Assert.IsNotNull(zePathToEnemy, "Path cannot be found at UpdateRoutine GoapNearTarget!");
        // But we will just walk the shortest length of tile to get to the m_EnemyState. Maybe when there is Accuracy point then it will be added in!
        List<TileId> zeTileToWalk = new List<TileId>();
        foreach (TileId zeTile in zePathToEnemy)
        {
            zeTileToWalk.Add(zeTile);
            Tile TileOfTileID = m_Planner.GameTileSystem.GetTile(zeTile);
            int TileDistance = TileId.GetDistance(m_Planner.GameTileSystem.GetTile(zeTile).GetTileId(), zeEnemyStat.CurrentTileID);
            // Once that supposed tile is good enough for this unit to attack the enemy!
            if (TileDistance <= m_AttackAct.MaxAttackRange && TileDistance >= m_AttackAct.MinAttackRange && m_Planner.m_Stats.GetViewScript().RaycastToTile(TileOfTileID, EnemyTile))
            {
                break;
            }
        }
        m_WalkAct.SetTilePath(zeTileToWalk.ToArray());
        m_WalkAct.CompletionCallBack += InvokeActionCompleted;
        m_WalkAct.TurnOn();
        WaitForFixedUpdate zeFixedWait = new WaitForFixedUpdate();
        while (!m_ActionCompleted)
            yield return zeFixedWait;
        UpdateEnemyInAttack();
        m_WalkAct.CompletionCallBack -= InvokeActionCompleted;
        print("Followed the target successfully");
        m_UpdateRoutine = null;
        yield break;
    }

    /// <summary>
    ///  Overriding this to check if there are any units within the target range!
    ///  If there are already units within this range, then the cost is 0
    /// </summary>
    /// <returns></returns>
    public override int GetCost()
    {
        if (m_EnemiesInAttack.Count > 0)
            return 0;
        return m_Cost;
    }

    protected virtual void UpdateEnemyInAttack()
    {
        m_EnemiesInAttack.Clear();
        // check through the global list of units at the EnemyUnitsManager!
        foreach (UnitStats zeGoStat in m_Planner.EnemiesManager.GetSeenEnemies())
        {
            int zeTileDistance = TileId.GetDistance(zeGoStat.CurrentTileID, m_Planner.m_Stats.CurrentTileID);
            if (zeTileDistance <= m_AttackAct.MaxAttackRange && zeTileDistance >= m_AttackAct.MinAttackRange && m_Planner.m_Stats.GetViewScript().RaycastToTile(m_Planner.GameTileSystem.GetTile(zeGoStat.CurrentTileID)))
            {
                m_EnemiesInAttack.Add(zeGoStat.gameObject);
            }
        }
        if (m_EnemiesInAttack.Count > 0)
        {
            m_Planner.m_StateData.CurrentStates.Add("TargetAttackInRange");
        }
        else
        {
            m_Planner.m_StateData.CurrentStates.Remove("TargetAttackInRange");
        }
    }

    protected void EnemyUnitDestroyed(GameObject _destroyedUnit, bool _destroyedUnitVisible)
    {
        m_EnemiesInAttack.Remove(_destroyedUnit);
    }
}
