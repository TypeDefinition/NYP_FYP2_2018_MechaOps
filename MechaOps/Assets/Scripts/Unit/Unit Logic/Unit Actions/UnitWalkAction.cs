using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

/// <summary>
/// A simper unit action for walking!
/// </summary>
public class UnitWalkAction : IUnitAction
{
    public int m_MovementPoints;
    [SerializeField, Tooltip("The walking animation handler")]
    protected MOAnimation_Move m_WalkAnimation;

    protected List<TileId> m_TilePath = new List<TileId>();
    protected TileSystem m_TileSystem = null;

    protected bool m_RegisteredAnimationCompleteCallback = false;

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

    protected override void InitializeEvents() { throw new System.NotImplementedException(); }

    protected override void DeinitializeEvents() { throw new System.NotImplementedException(); }

    protected virtual void RegisterAnimationCompleteCallback()
    {
        if (!m_RegisteredAnimationCompleteCallback)
        {
            m_WalkAnimation.CompletionCallback += OnAnimationCompleted;
            m_RegisteredAnimationCompleteCallback = true;
        }
    }

    protected virtual void UnregisterAnimationCompleteCallback()
    {
        if (m_RegisteredAnimationCompleteCallback)
        {
            m_WalkAnimation.CompletionCallback -= OnAnimationCompleted;
            m_RegisteredAnimationCompleteCallback = false;
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
        RegisterAnimationCompleteCallback();
        StartWalkAnimation();
    }

    public override void PauseAction()
    {
        base.PauseAction();
        m_WalkAnimation.PauseAnimation();
    }

    public override void ResumeAction()
    {
        base.ResumeAction();
        m_WalkAnimation.ResumeAnimation();
    }

    public override void StopAction()
    {
        base.StopAction();
        UnregisterAnimationCompleteCallback();
        m_WalkAnimation.StopAnimation();
    }

    protected void StartWalkAnimation()
    {
        Tile destinationTile = m_TileSystem.GetTile(m_TilePath[0]);
        m_WalkAnimation.Destination = destinationTile.gameObject.transform.position;
        m_WalkAnimation.StartAnimation();
    }

    protected override void OnAnimationCompleted()
    {
        base.OnAnimationCompleted();

        // Signal that we have moved to a tile.
        GetUnitStats().CurrentTileID = m_TilePath[0];
        GameEventSystem.GetInstance().TriggerEvent<GameObject>("UnitMovedToTile", gameObject);

        // Move on to the next tile.
        m_TilePath.RemoveAt(0);
        if (m_ActionState != ActionState.Running) { return; }

        if (m_TilePath.Count > 0)
        {
            StartWalkAnimation();
        }
        else
        {
            // The unit has reached it's destination.
            UnregisterAnimationCompleteCallback();
            m_WalkAnimation.GetMOAnimator().StopCinematicCamera();
            m_ActionState = ActionState.Completed;

            // Sending out an event that this action has ended.
            GameEventSystem.GetInstance().TriggerEvent("UnitFinishAction");
            InvokeCompletionCallback();

            CheckIfUnitFinishedTurn();
        }
    }

    protected override void StartTurnCallback() { throw new System.NotImplementedException(); }

    protected override void EndTurnCallback() { throw new System.NotImplementedException(); }

    public override bool VerifyRunCondition() { return m_TilePath.Count > 0; }

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
        m_UnitStats.GetGameSystemsDirectory().GetUnitActionScheduler().ScheduleAction(this);
    }
}