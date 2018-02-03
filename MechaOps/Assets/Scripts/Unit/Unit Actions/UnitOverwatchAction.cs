using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitOverwatchAction : UnitAttackAction
{
    protected bool m_CanShoot = true;

    // Events
    protected override void InitEvents()
    {
        base.InitEvents();
        GameEventSystem.GetInstance().SubscribeToEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitMovedToTile), OnUnitMovedToTile);
    }

    protected override void DeinitEvents()
    {
        base.DeinitEvents();
        GameEventSystem.GetInstance().UnsubscribeFromEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitMovedToTile), OnUnitMovedToTile);
    }

    // Other(s)
    protected override void OnTurnOn()
    {
        Debug.Log("Overwatch Begin!");
        base.OnTurnOn();

        DeductActionPoints();
        // Overwatch is a weird action, as it sends 2 UnitFinishedAction events.
        // Once when it is turned on, and once after it shoots.
        // However, the action state is not changed here.
        // The completion callback is also not invoked.
        GameEventSystem.GetInstance().TriggerEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitFinishedAction), m_UnitStats);
        CheckIfUnitFinishedTurn();
    }

    protected override void OnTurnOff()
    {
        base.OnTurnOff();
        m_CanShoot = true;
    }

    // Event Callbacks
    protected override void OnTurnStart(FactionType _factionType)
    {
        base.OnTurnStart(_factionType);

        if (TurnedOn && _factionType == m_UnitStats.UnitFaction)
        {
            TurnOff();
        }
    }

    protected virtual void OnUnitMovedToTile(UnitStats _movedUnit)
    {
        // Ensure that this action is turned on.
        if (!TurnedOn) { return; }
        if (!m_CanShoot) { return; }

        // Ensure that the moved unit is not an ally.
        if (_movedUnit.UnitFaction == m_UnitStats.UnitFaction) { return; }

        // Get the UnitsManager managing this unit.
        UnitsManager unitsManager = null;
        if (m_GameFlowManager.PlayerFaction == m_UnitStats.UnitFaction)
        {
            unitsManager = GameSystemsDirectory.GetSceneInstance().GetPlayerUnitsManager();
        }
        else
        {
            AIUnitsManager[] aiUnitManagers = GameSystemsDirectory.GetSceneInstance().GetAIUnitsManager();
            for (int i = 0; i < aiUnitManagers.Length; ++i)
            {
                if (aiUnitManagers[i].ManagedFaction == m_UnitStats.UnitFaction)
                {
                    unitsManager = aiUnitManagers[i];
                    break;
                }
            }
        }
        Assert.IsNotNull(unitsManager);
        Assert.IsTrue(unitsManager.ManagedFaction == m_UnitStats.UnitFaction);

        // Get Seen Enemies
        List<UnitStats> seenEnemies = unitsManager.GetSeenEnemies();

        bool canSeeMovedUnit = false;
        // Ensure that the unit that just moved is seen.
        for (int i = 0; i < seenEnemies.Count; ++i)
        {
            if (seenEnemies[i] == _movedUnit)
            {
                canSeeMovedUnit = true;
                break;
            }
        }

        // If we cannot see the unit, we cannot shoot.
        if (!canSeeMovedUnit) { return; }

        // If every other condition is met, shoot at the target.
        m_TargetUnitStats = _movedUnit;
        if (VerifyRunCondition())
        {
            Debug.Log("Overwatch Scheduled!");
            m_CanShoot = false;
            m_UnitStats.GetGameSystemsDirectory().GetUnitActionScheduler().ScheduleAction(this);
        }
        else
        {
            m_TargetUnitStats = null;
        }
    }

    /// <summary>
    /// Calculate the hit percentage of this attack.
    /// It should return an int between 1(Inclusive) and 100(Inclusive).
    /// </summary>
    public override int CalculateHitChance()
    {
        int hitChance = (int)((float)base.CalculateHitChance() * 0.75f);
        return Mathf.Clamp(hitChance, 1, 100);
    }

    protected override void Awake()
    {
        base.Awake();
        Assert.IsTrue(EndsTurn, MethodBase.GetCurrentMethod().Name + " - Overwatch action must have m_EndsTurn = true!");
    }
}