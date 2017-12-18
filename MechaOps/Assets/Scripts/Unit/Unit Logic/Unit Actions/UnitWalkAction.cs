using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

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
        Assert.IsTrue(m_UnitActionName != null && m_UnitActionName != "", "No name is given to this action");
        m_TileSystem = GetComponent<UnitStats>().GetGameSystemsDirectory().GetTileSystem();
        Assert.IsNotNull(m_TileSystem, " - m_TileSystem must not be null!");
    }

    public override void StopActionUpdate() {}

    public override void StartAction()
    {
        base.StartAction();
        GetUnitStats().CurrentActionPoints -= ActionCost;
        RegisterAnimationCompleteCallback();
        StartWalkAnimation();
    }

    public override void PauseAction()
    {
        m_ActionState = ActionState.Paused;
        UnregisterAnimationCompleteCallback();
        m_WalkAnimation.PauseAnimation();
    }

    public override void ResumeAction()
    {
        m_ActionState = ActionState.Running;
        RegisterAnimationCompleteCallback();
        m_WalkAnimation.ResumeAnimation();
    }

    public override void StopAction()
    {
        m_ActionState = ActionState.Completed;
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
            m_WalkAnimation.CompletionCallback -= this.OnAnimationCompleted;
            m_WalkAnimation.GetMOAnimator().StopCinematicCamera();
            m_ActionState = ActionState.Completed;

            // Sending out an event that this action has ended.
            GameEventSystem.GetInstance().TriggerEvent("UnitFinishAction");
            InvokeCompletionCallback();

            // TODO: This needs to be moved away. It should not be done in the action as
            // A) It would be repetitive code. Why does every action need to check this?
            // B) Just because the CurrentActionPoints are > 0 does not mean that the turn has not ended for this unit.
            // If I have 3 Action Points, and 2 actions, each costing 2 points, then the unit's turn should end when it has
            // only 1 Action Point left.
            if (GetUnitStats().CurrentActionPoints == 0)
            {
                GetUnitStats().ResetUnitStats();
                // tell the player unit manager that it can no longer do any action
                GameEventSystem.GetInstance().TriggerEvent<GameObject>("UnitMakeMove", gameObject);
            }
        }
    }

    protected override void StartTurnCallback() { throw new System.NotImplementedException(); }

    protected override void EndTurnCallback() { throw new System.NotImplementedException(); }

    public override bool VerifyRunCondition() { return m_TilePath.Count > 0; }

    protected override void OnTurnOn()
    {
        base.OnTurnOn();
        Assert.IsTrue(VerifyRunCondition());
        m_UnitStats.GetGameSystemsDirectory().GetUnitActionScheduler().ScheduleAction(this);
    }
}