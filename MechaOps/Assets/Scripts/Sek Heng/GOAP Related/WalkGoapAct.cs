﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkGoapAct : IGoapAction {
    [Header("References for WalkGoapAct")]
    [SerializeField, Tooltip("The Walk Unit Action that is needed to link")]
    protected UnitWalkAction m_WalkAct;

    [Header("Debugging purpose for WalkGoapAct")]
    [SerializeField, Tooltip("The Tile to move to")]
    protected TileId m_TileDest;

    public TileId TileDest
    {
        set
        {
            m_TileDest = value;
        }
    }

    protected override void Start()
    {
        base.Start();
        if (!m_WalkAct)
            m_WalkAct = GetComponent<UnitWalkAction>();
    }

    public override void DoAction()
    {
        print("GOAP doing start walking");
        m_UpdateRoutine = StartCoroutine(UpdateActRoutine());
    }

    public override IEnumerator UpdateActRoutine()
    {
        //m_Planner.m_Stats.CurrentActionPoints--;
        // Set the destination from the EnemyUnitManager if there is no destination!
        if (m_TileDest == null)
            m_TileDest = EnemyUnitManager.Instance.TilePlayerUnits;
        // so why not lets just cheat here to get to the closest tile!
        TileId[] zeTileToWalkTo = m_WalkAct.m_TileSys.GetPath(99999, m_Planner.m_Stats.CurrentTileID, m_TileDest, m_Planner.m_Stats.GetTileAttributeOverrides());
        yield return null;
        // from here, we get the walkable tiles!
        int zeCounter = 0;
        TileId[] zeAvailablePaths = new TileId[m_WalkAct.m_MovementPoints];
        foreach (TileId zeTile in zeTileToWalkTo)
        {
            zeAvailablePaths[zeCounter++] = zeTile;
            // it reaches it's max movement points it has to stop!
            if (zeCounter == m_WalkAct.m_MovementPoints)
                break;
        }
        m_WalkAct.m_TilePath = zeAvailablePaths;
        UnitActionScheduler zeActScheduler = FindObjectOfType<UnitActionScheduler>();
        m_WalkAct.TurnOn();
        zeActScheduler.ScheduleAction(m_WalkAct);
        // destination is reached maybe but there is nothing more it can do right now!
        m_TileDest = null;
        WaitForFixedUpdate zeFixedUp = new WaitForFixedUpdate();
        while (m_WalkAct.GetActionState() != IUnitAction.ActionState.Completed)
            yield return zeFixedUp;
        print("Finished walk action");
        yield break;
    }
}