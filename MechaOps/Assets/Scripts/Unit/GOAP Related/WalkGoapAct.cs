using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WalkGoapAct : IGoapAction {
    [Header("References for WalkGoapAct")]
    [SerializeField, Tooltip("The Walk Unit Action that is needed to link")]
    protected UnitMoveAction m_WalkAct;
    [SerializeField, Tooltip("Unit skip action if unit cannot move!")]
    protected UnitSkipAction m_SkipAction;

    [Header("Debugging purpose for WalkGoapAct")]
    [SerializeField, Tooltip("The Tile to move to")]
    protected TileId m_TileDest;
    [SerializeField, Tooltip("Visible view script")]
    protected ViewScript m_UnitViewScript;
    [SerializeField] bool m_SeenMovingFlag = false;
    [SerializeField] GameEventNames m_EventNames;

    public TileId TileDest
    {
        set
        {
            m_TileDest = value;
        }
    }

    private void Awake()
    {
        m_EventNames = GameSystemsDirectory.GetSceneInstance().GetGameEventNames();
    }

    private void OnEnable()
    {
        GameEventSystem.GetInstance().SubscribeToEvent<UnitStats>(m_EventNames.GetEventName(GameEventNames.GameplayNames.UnitSeen), SeenMoving);
        GameEventSystem.GetInstance().SubscribeToEvent<UnitStats>(m_EventNames.GetEventName(GameEventNames.GameplayNames.UnitUnseen), UnseenMoving);
    }

    private void OnDisable()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<UnitStats>(m_EventNames.GetEventName(GameEventNames.GameplayNames.UnitSeen), SeenMoving);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<UnitStats>(m_EventNames.GetEventName(GameEventNames.GameplayNames.UnitUnseen), UnseenMoving);
    }

    protected override void Start()
    {
        base.Start();
        if (!m_WalkAct)
            m_WalkAct = GetComponent<UnitMoveAction>();
        if (!m_SkipAction)
            m_SkipAction = GetComponent<UnitSkipAction>();
    }

    public override void DoAction()
    {
        print("GOAP doing start walking");
        m_ActionCompleted = false;
        Assert.IsNull(m_UpdateRoutine, "Walk Goap Act is still updating!!!");
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

        // Start following the unit.
        if (m_SeenMovingFlag)
        {
            GameEventSystem.GetInstance().TriggerEvent<GameObject>(m_EventNames.GetEventName(GameEventNames.GameUINames.FollowTarget), m_WalkAct.GetUnitStats().gameObject);
        }

        TileId[] zeTileToWalkTo = null;
        foreach (TileId TileLocation in m_Planner.EnemiesManager.GetOneTileAwayFromEnemyWithoutAGauranteeOfAWalkableTileAtAll())
        {
            Assert.IsFalse(m_Planner.m_Stats.CurrentTileID.Equals(TileLocation), "Update of Tile coordinate has failed at UpdateActRoutine in WalkGoapAct.cs");
            zeTileToWalkTo = m_WalkAct.GetTileSystem().GetPath(99999, m_Planner.m_Stats.CurrentTileID, TileLocation, m_Planner.m_Stats.GetTileAttributeOverrides());
            // have to ensure it is not null and the length is more than 0!
            if (zeTileToWalkTo != null && zeTileToWalkTo.Length > 0)
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

        WaitForFixedUpdate zeFixedUp = new WaitForFixedUpdate();
        // QUICK FIX!
        if (zeAvailablePaths.Length == 0)
        {
            m_SkipAction.CompletionCallBack += InvokeActionCompleted;
            m_SkipAction.TurnOn();
            while (!m_ActionCompleted)
            {
                yield return zeFixedUp;
            }
            m_SkipAction.CompletionCallBack -= InvokeActionCompleted;
            print("Unit skip walking action at WalkGoapAct");
            m_TileDest = null;
            m_UpdateRoutine = null;
            GameEventSystem.GetInstance().TriggerEvent<GameObject>(m_EventNames.GetEventName(GameEventNames.GameUINames.FollowTarget), null);
            yield break;
        }

        m_WalkAct.SetTilePath(zeAvailablePaths);
        m_WalkAct.CompletionCallBack += InvokeActionCompleted;
        m_WalkAct.TurnOn();
        // destination is reached maybe but there is nothing more it can do right now!
        m_TileDest = null;
        while (!m_ActionCompleted)
        {
            yield return zeFixedUp;
        }
        print("Finished walk action");
        m_WalkAct.CompletionCallBack -= InvokeActionCompleted;
        m_TileDest = null;
        m_UpdateRoutine = null;

        // Stop following the unit.
        GameEventSystem.GetInstance().TriggerEvent<GameObject>(m_EventNames.GetEventName(GameEventNames.GameUINames.FollowTarget), null);

        yield break;
    }

    void SeenMoving(UnitStats _unitStat)
    {
        if (_unitStat.gameObject == gameObject && !m_SeenMovingFlag)
        {
            m_SeenMovingFlag = true;
            if (m_UpdateRoutine != null)
            {
                GameEventSystem.GetInstance().TriggerEvent<GameObject>(m_EventNames.GetEventName(GameEventNames.GameUINames.FollowTarget), m_WalkAct.GetUnitStats().gameObject);
            }
        }
    }

    void UnseenMoving(UnitStats _unitStat)
    {
        if (_unitStat.gameObject == gameObject && m_SeenMovingFlag)
        {
            m_SeenMovingFlag = false;
            if (m_UpdateRoutine != null)
            {
                GameEventSystem.GetInstance().TriggerEvent<GameObject>(m_EventNames.GetEventName(GameEventNames.GameUINames.FollowTarget), null);
            }
        }
    }
}
