using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// For the artillery unit attack action logic
/// </summary>
public class SHSMShootAction : UnitAttackAction
{
    [SerializeField] protected MOAnimation_SHSMShoot m_Animation = null;

    protected TileSystem m_TileSystem = null;

    protected TileId m_TargetedTile;
    protected TileId m_HitTileId;
    protected int m_TargetRadius = 1;

    protected bool m_RegisteredAnimationCompleteCallback = false;

    protected virtual void RegisterAnimationCompleteCallback()
    {
        if (!m_RegisteredAnimationCompleteCallback)
        {
            m_Animation.CompletionCallback += this.OnAnimationCompleted;
            m_RegisteredAnimationCompleteCallback = true;
        }
    }

    protected virtual void UnregisterAnimationCompleteCallback()
    {
        if (m_RegisteredAnimationCompleteCallback)
        {
            m_Animation.CompletionCallback -= OnAnimationCompleted;
            m_RegisteredAnimationCompleteCallback = false;
        }
    }

    public int GetTargetRadius()
    {
        return m_TargetRadius;
    }

    protected override void Awake()
    {
        base.Awake();
        m_TileSystem = m_UnitStats.GetGameSystemsDirectory().GetTileSystem();
        Assert.IsNotNull(m_TileSystem, MethodBase.GetCurrentMethod().Name + " - TileSystem not found!");
    }

    public override void SetTarget(GameObject _target)
    {
        m_TargetedTile = _target.GetComponent<Tile>().GetTileId();
        Assert.IsNotNull(m_TargetedTile, MethodBase.GetCurrentMethod().Name + " - _target does not have a Tile Component");
    }

    public override void StartAction()
    {
        base.StartAction();
        RegisterAnimationCompleteCallback();
        StartShootingAnimation();
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
        UnregisterAnimationCompleteCallback();
        m_Animation.StopAnimation();
    }

    protected void StartShootingAnimation()
    {
        m_HitTileId = CalculateHitTile();
        m_Animation.TargetTile = m_TileSystem.GetTile(m_HitTileId);
        m_Animation.StartAnimation();
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
        m_UnitStats.GetGameSystemsDirectory().GetUnitActionScheduler().ScheduleAction(this);
    }

    public override bool VerifyRunCondition()
    {
        if (m_TargetedTile == null) { return false; }
        int distanceToTarget = TileId.GetDistance(m_TargetedTile, GetUnitStats().CurrentTileID);
        if (distanceToTarget > m_MaxAttackRange || distanceToTarget < m_MinAttackRange) { return false; }

        return true;
    }

    protected override void OnAnimationCompleted()
    {
        m_ActionState = ActionState.Completed;
        UnregisterAnimationCompleteCallback();

        Tile hitTile = m_TileSystem.GetTile(m_HitTileId);
        Assert.IsNotNull(hitTile, MethodBase.GetCurrentMethod().Name + " - hitTile not found!");
        hitTile.SetHazardType(HazardType.Fire);
        hitTile.GetHazard().TurnsToDecay = 3;
        hitTile.GetHazard().Decay = true;

        if (hitTile.HasUnit())
        {
            UnitStats hitUnitStats = hitTile.Unit.GetComponent<UnitStats>();
            hitUnitStats.CurrentHealthPoints -= m_DamagePoints;
            hitUnitStats.InvokeHealthDropCallback(m_UnitStats);
        }

        GameEventSystem.GetInstance().TriggerEvent("UnitFinishAction");
        InvokeCompletionCallback();

        CheckIfUnitFinishedTurn();
    }

    protected TileId CalculateHitTile()
    {
        TileId[] possibleHitTiles = m_TileSystem.GetSurroundingTiles(m_TargetedTile, m_TargetRadius);
        Assert.IsTrue(possibleHitTiles != null && possibleHitTiles.Length > 0, " - No possible tiles!");

        return possibleHitTiles[Random.Range(0, possibleHitTiles.Length)];
    }

    protected override int CalculateHitChance()
    {
        throw new System.NotImplementedException();
    }
}