using System.Collections;
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
        //Tile locatedLocation = null;
        
        // TODO!
        //foreach (TileId TileLocation in m_Planner.EnemiesManager.PlayerUnitLocations)
        //{
        //    locatedLocation = m_WalkAct.GetTileSystem().GetTile(TileLocation);
        //    if (!m_Planner.m_Stats.CurrentTileID.Equals(m_Planner.EnemiesManager.PlayerUnitLocations) && !locatedLocation.HasUnit())
        //    {
        //        m_TileDest = TileLocation;
        //        break;
        //    }
        //    else
        //    {
        //        locatedLocation = null;
        //    }
        //}
        //Assert.IsNotNull(locatedLocation, "Update of Tile coordinate has failed at UpdateActRoutine in WalkGoapAct.cs");
        //if (m_Planner.m_Stats.CurrentTileID.Equals(m_Planner.EnemiesManager.PlayerUnitLocations))
        //    Assert.IsTrue(true == false, "Update of Tile coordinate has failed at UpdateActRoutine in WalkGoapAct.cs");
        // so why not lets just cheat here to get to the closest tile!
        TileId[] zeTileToWalkTo = null;
        foreach (TileId TileLocation in m_Planner.EnemiesManager.GetOneTileAwayFromEnemyWithoutAGauranteeOfAWalkableTileAtAll())
        {
            Assert.IsFalse(m_Planner.m_Stats.CurrentTileID.Equals(TileLocation), "Update of Tile coordinate has failed at UpdateActRoutine in WalkGoapAct.cs");
            zeTileToWalkTo = m_WalkAct.GetTileSystem().GetPath(99999, m_Planner.m_Stats.CurrentTileID, TileLocation, m_Planner.m_Stats.GetTileAttributeOverrides());
            if (zeTileToWalkTo != null)
            {
                m_TileDest = TileLocation;
                break;
            }
        }
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
