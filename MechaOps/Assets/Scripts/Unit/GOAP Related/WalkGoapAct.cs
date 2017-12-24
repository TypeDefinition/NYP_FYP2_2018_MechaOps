﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WalkGoapAct : IGoapAction {
    [Header("References for WalkGoapAct")]
    [SerializeField, Tooltip("The Walk Unit Action that is needed to link")]
    protected UnitMoveAction m_WalkAct;

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
            m_WalkAct = GetComponent<UnitMoveAction>();
    }

    public override void DoAction()
    {
        print("GOAP doing start walking");
        m_ActionCompleted = false;
        m_UpdateRoutine = StartCoroutine(UpdateActRoutine());
    }

    public override IEnumerator UpdateActRoutine()
    {
        // TODO!
        // Set the destination from the EnemyUnitsManager if it has already reaches the destination or if that area got occupied
        if (m_Planner.m_Stats.CurrentTileID.Equals(m_Planner.EnemiesManager.PlayerUnitLocations) || m_WalkAct.GetTileSystem().GetTile(m_Planner.EnemiesManager.PlayerUnitLocations).HasUnit())
            m_Planner.EnemiesManager.UpdateMarker();
        if (m_Planner.m_Stats.CurrentTileID.Equals(m_Planner.EnemiesManager.PlayerUnitLocations))
            Assert.IsTrue(true == false, "Update of Tile coordinate has failed at UpdateActRoutine in WalkGoapAct.cs");
        m_TileDest = m_Planner.EnemiesManager.PlayerUnitLocations;
        // so why not lets just cheat here to get to the closest tile!
        TileId[] zeTileToWalkTo = m_WalkAct.GetTileSystem().GetPath(99999, m_Planner.m_Stats.CurrentTileID, m_TileDest, m_Planner.m_Stats.GetTileAttributeOverrides());
        // from here, we get the walkable tiles!
        int zeCounter = 0;
        //TODO, this will sometimes has null reference
        int zeMaxTileNum = Mathf.Min(zeTileToWalkTo.Length, m_WalkAct.MovementPoints);
        TileId[] zeAvailablePaths = new TileId[zeMaxTileNum];
        foreach (TileId zeTile in zeTileToWalkTo)
        {
            zeAvailablePaths[zeCounter++] = zeTile;
            // it reaches it's max movement points it has to stop!
            if (zeCounter == zeMaxTileNum)
                break;
        }
        m_WalkAct.SetTilePath(zeAvailablePaths);
        m_WalkAct.CompletionCallBack += InvokeActionCompleted;
        m_WalkAct.TurnOn();
        // destination is reached maybe but there is nothing more it can do right now!
        m_TileDest = null;
        WaitForFixedUpdate zeFixedUp = new WaitForFixedUpdate();
        while (!m_ActionCompleted)
            yield return zeFixedUp;
        print("Finished walk action");
        m_WalkAct.CompletionCallBack -= InvokeActionCompleted;
        m_TileDest = null;
        yield break;
    }
}
