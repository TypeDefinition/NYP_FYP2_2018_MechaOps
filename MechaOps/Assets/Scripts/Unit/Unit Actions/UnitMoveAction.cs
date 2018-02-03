using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitMoveAction : IUnitAction
{
    [SerializeField] protected int m_MovementPoints = 1;
    [SerializeField] protected MOAnimation_Move m_Animation;

    protected List<TileId> m_TilePath = new List<TileId>();
    protected TileSystem m_TileSystem = null;

    protected bool m_RegisteredAnimationCompleteCallback = false;
    protected bool m_RegisteredReachTileCallback = false;

    public int MovementPoints
    {
        get { return m_MovementPoints; }
        set { m_MovementPoints = Mathf.Max(1, value); }
    }

    public void SetTilePath(TileId[] _tilePath)
    {
        m_TilePath.Clear();
        for (int i = 0; i < _tilePath.Length; ++i)
        {
            m_TilePath.Add(_tilePath[i]);
        }
    }

    public TileId[] GetTilePath() { return m_TilePath.ToArray(); }

    public TileSystem GetTileSystem() { return m_TileSystem; }

    protected virtual void RegisterAnimationCallbacks()
    {
        if (!m_RegisteredAnimationCompleteCallback)
        {
            m_Animation.CompletionCallback += OnAnimationCompleted;
            m_RegisteredAnimationCompleteCallback = true;
        }

        if (!m_RegisteredReachTileCallback)
        {
            m_Animation.ReachedTileCallback += OnReachTile;
            m_RegisteredReachTileCallback = true;
        }
    }

    protected virtual void UnregisterAnimationCallbacks()
    {
        if (m_RegisteredAnimationCompleteCallback)
        {
            m_Animation.CompletionCallback -= OnAnimationCompleted;
            m_RegisteredAnimationCompleteCallback = false;
        }

        if (m_RegisteredReachTileCallback)
        {
            m_Animation.ReachedTileCallback -= OnReachTile;
            m_RegisteredReachTileCallback = false;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        m_TileSystem = GetComponent<UnitStats>().GetGameSystemsDirectory().GetTileSystem();
        Assert.IsNotNull(m_TileSystem, " - m_TileSystem must not be null!");
    }

    public override void StartAction()
    {
        base.StartAction();
        RegisterAnimationCallbacks();
        m_Animation.MovementPath = m_TilePath.ToArray();
        m_Animation.StartAnimation();
    }

    public override void PauseAction()
    {
        base.PauseAction();
        m_Animation.PauseAnimation();
    }

    public override void ResumeAction()
    {
        base.ResumeAction();
        m_Animation.ResumeAnimation();
    }

    public override void StopAction()
    {
        base.StopAction();
        UnregisterAnimationCallbacks();
        m_Animation.StopAnimation();
    }

    protected virtual void OnReachTile(int _reachedTileIndex)
    {
        // Signal that we have moved to a tile.
        GetUnitStats().CurrentTileID = m_TilePath[_reachedTileIndex];
        GameEventSystem.GetInstance().TriggerEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitMovedToTile), m_UnitStats);
    }

    protected override void OnAnimationCompleted()
    {
        base.OnAnimationCompleted();

        // The unit has reached it's destination.
        UnregisterAnimationCallbacks();

        m_ActionState = ActionState.Completed;

        // Sending out an event that this action has ended.
        GameEventSystem.GetInstance().TriggerEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitFinishedAction), m_UnitStats);
        InvokeCompletionCallback();

        CheckIfUnitFinishedTurn();
    }

    public override bool VerifyRunCondition()
    {
        return m_TilePath.Count > 0 && m_UnitStats.IsAlive();
    }

    // Even though we check Assert.IsTrue(VerifyRunCondition()); here,
    // This is not the case for ALL actions. For an action like overwatch,
    // it is perfectly okay for VerifyRunCondition() to return false, since we are not
    // shooting any enemy now. Rather, we are WAITING for some point in time in the future
    // when VerifyRunCondition() returns true. It is also possible for an action like Overwatch
    // to never excecute because no enemies walked into the attack range of the unit.
    protected override void OnTurnOn()
    {
        base.OnTurnOn();
        Assert.IsTrue(VerifyRunCondition());
        DeductActionPoints();
        m_UnitStats.GetGameSystemsDirectory().GetUnitActionScheduler().ScheduleAction(this);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        MovementPoints = m_MovementPoints;
    }
#endif // UNITY_EDITOR
}