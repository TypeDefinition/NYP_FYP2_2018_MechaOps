#define GOAP_AI
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Mainly to iterate and update all of the enemy units 1 by 1
/// </summary>
public class AIUnitsManager : UnitsManager
{
    [SerializeField, Tooltip("Help to check the surrounding tiles of the array. The larger the value, the longer it searches!")]
    int m_CheckSurroundingUnitRadius = 2;
    // Non-Serialised Variable(s)
    List<TileId> m_OneTileAwayFromEnemyLocation = new List<TileId>();
    IEnumerator m_UpdateCoroutine = null;

    public List<TileId> GetOneTileAwayFromEnemyWithoutAGauranteeOfAWalkableTileAtAll() { return m_OneTileAwayFromEnemyLocation; }

    protected override void Awake()
    {
        base.Awake();
        InitEvents();
        m_OwnUnitDeadInTurnCallback += UnitDiedWhileUpdate;
    }

    protected virtual void OnDestroy()
    {
        DeinitEvents();
    }

    // Gameplay
    protected override void OnGameOver(FactionType _winner)
    {
        base.OnGameOver(_winner);
        if (m_UpdateCoroutine != null)
        {
            StopCoroutine(m_UpdateCoroutine);
            m_UpdateCoroutine = null;
        }
    }

    protected override void TurnStart(FactionType _factionType)
    {
        if (_factionType != m_ManagedFaction) { return; }
        m_UpdateCoroutine = UpdateCoroutine();
        StartCoroutine(m_UpdateCoroutine);
    }

    protected override void UnitFinishedTurn(UnitStats _unit) {}
    protected override void UnitFinishedAction(UnitStats _unit) {}

    /// <summary>
    /// The coroutine to update the manager!
    /// </summary>
    /// <returns></returns>
    protected IEnumerator UpdateCoroutine()
    {
        UpdateMarkers();
        GetManagedUnitsFromUnitTracker();

#if GOAP_AI
        foreach (UnitStats unit in m_ManagedUnits)
        {
            GoapPlanner goapPlanner = unit.GetComponent<GoapPlanner>();
            if (goapPlanner == null) { continue; }
            Assert.IsNotNull(goapPlanner, MethodBase.GetCurrentMethod().Name + " - GOAPPlanner not found in managed unit!");

            // wait till the update is finish then proceed to the next unit
            IEnumerator aiCoroutine = goapPlanner.StartPlanning();
            Coroutine AIRoutine = StartCoroutine(aiCoroutine);
            //yield return aiCoroutine;
            yield return AIRoutine;
        }
#endif

        GetManagedUnitsFromUnitTracker();
        foreach (UnitStats unit in m_ManagedUnits)
        {
            unit.ResetActionPoints();
        }

        Debug.Log("Finish AI Manager's Turn");
        m_UpdateCoroutine = null;
        GameEventSystem.GetInstance().TriggerEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.TurnEnd), m_ManagedFaction);
        yield break;
    }

    protected bool CheckIsTileWalkable(TileAttributeOverride[] _tileAttributeOverrides, TileType _tileType, ref bool _result)
    {
        for (int i = 0; i < _tileAttributeOverrides.Length; ++i)
        {
            if (_tileAttributeOverrides[i].GetTileType() == _tileType)
            {
                _result = _tileAttributeOverrides[i].Walkable;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Meant to re-update the position of the player units gathered if the enemy unit has reached the current marker
    /// </summary>
    public void UpdateMarkers()
    {
        // Get all the alive enemy units.
        List<UnitStats> aliveEnemies = new List<UnitStats>();
        for (int i = 0; i < (int)FactionType.Num_FactionType; ++i)
        {
            if ((FactionType)i == m_ManagedFaction) { continue; }

            UnitStats[] units = m_UnitsTracker.GetAliveUnits((FactionType)i);
            for (int j = 0; j < units.Length; ++j)
            {
                aliveEnemies.Add(units[j]);
            }
        }

        // No idea what the fuck you were doing here, but this sure as shit ain't enemy locations.
        // What happens if within a 1 tile radius there isn't a walkable tile?
        m_OneTileAwayFromEnemyLocation.Clear();
        foreach (UnitStats enemy in aliveEnemies)
        {
            TileAttributeOverride[] tileAttributeOverrides = enemy.GetTileAttributeOverrides();

            // we only need the 1 radius tile!
            TileId[] surroundingTiles = m_TileSystem.GetSurroundingTiles(enemy.CurrentTileID, m_CheckSurroundingUnitRadius);
            for (int i = 0; i < surroundingTiles.Length; ++i)
            {
                Tile tile = m_TileSystem.GetTile(surroundingTiles[i]);
                bool tileIsWalkable = tile.GetIsWalkable();
                CheckIsTileWalkable(tileAttributeOverrides, tile.GetTileType(), ref tileIsWalkable);

                if (tile.GetIsWalkable() && !tile.HasUnit())
                {
                    m_OneTileAwayFromEnemyLocation.Add(surroundingTiles[i]);
                }
            }
        }
    }

    /// <summary>
    /// Deal with the overwatch stuff
    /// </summary>
    /// <param name="_unitStat"></param>
    protected void UnitDiedWhileUpdate(UnitStats _unitStat)
    {
        if (m_UpdateCoroutine != null)
        {
            // reset the turns
            StopUpdate();
            TurnStart(m_ManagedFaction);
        }
    }

    protected void StopUpdate()
    {
        if (m_UpdateCoroutine != null)
        {
            StopCoroutine(m_UpdateCoroutine);
        }
    }
}