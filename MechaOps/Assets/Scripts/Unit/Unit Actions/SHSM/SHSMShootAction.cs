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
    [SerializeField] protected int m_TargetRadius = 1;
    [SerializeField] protected int m_NumHitTiles = 4;

    protected TileSystem m_TileSystem = null;

    protected TileId m_TargetedTile;
    protected List<TileId> m_HitTileIds;

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

    public void SetTargetRadius(int _targetRadius)
    {
        m_TargetRadius = Mathf.Max(1, _targetRadius);
    }

    public int GetTargetRadius()
    {
        return m_TargetRadius;
    }

    public void SetNumHitTiles(int _numHitTiles)
    {
        m_NumHitTiles = Mathf.Max(0, _numHitTiles);
    }

    public int GetNumHitTiles()
    {
        return m_NumHitTiles;
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
        m_HitTileIds = CalculateHitTiles();

        List<Tile> tileList = new List<Tile>();
        for (int i = 0; i < m_HitTileIds.Count; ++i)
        {
            tileList.Add(m_TileSystem.GetTile(m_HitTileIds[i]));
        }

        m_Animation.TargetTiles = tileList.ToArray();
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
        DeductActionPoints();
        m_UnitStats.GetGameSystemsDirectory().GetUnitActionScheduler().ScheduleAction(this);
    }

    public override bool VerifyRunCondition()
    {
        if (m_UnitStats.IsAlive() == false) { return false; }
        if (m_TargetedTile == null) { return false; }
        int distanceToTarget = TileId.GetDistance(m_TargetedTile, GetUnitStats().CurrentTileID);
        if (distanceToTarget > m_MaxAttackRange || distanceToTarget < m_MinAttackRange) { return false; }

        return true;
    }

    protected override void OnAnimationCompleted()
    {
        // Deal attack effects.
        for (int i = 0; i < m_HitTileIds.Count; ++i)
        {
            Tile hitTile = m_TileSystem.GetTile(m_HitTileIds[i]);
            Assert.IsNotNull(hitTile, MethodBase.GetCurrentMethod().Name + " - hitTile not found!");
            hitTile.SetHazardType(HazardType.Fire);
            hitTile.GetHazard().TurnsToDecay = 3;
            hitTile.GetHazard().CurrentTurnsToDecay = hitTile.GetHazard().TurnsToDecay;
            hitTile.GetHazard().Decay = true;

            if (hitTile.HasUnit())
            {
                UnitStats hitUnitStats = hitTile.Unit.GetComponent<UnitStats>();

                m_Animation_DamageIndicator.Hit = true;
                m_Animation_DamageIndicator.DamageValue = m_DamagePoints;
                m_Animation_DamageIndicator.Target = hitUnitStats.gameObject;
                m_Animation_DamageIndicator.StartAnimation();

                hitUnitStats.CurrentHealthPoints -= m_DamagePoints;
                GameEventSystem.GetInstance().TriggerEvent<UnitStats, UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.AttackedUnit), m_UnitStats, hitUnitStats);
            }
        }

        m_ActionState = ActionState.Completed;
        UnregisterAnimationCompleteCallback();
        
        GameEventSystem.GetInstance().TriggerEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitFinishedAction), m_UnitStats);
        InvokeCompletionCallback();
        
        CheckIfUnitFinishedTurn();
    }

    protected List<TileId> CalculateHitTiles()
    {
        TileId[] possibleHitTiles = m_TileSystem.GetSurroundingTiles(m_TargetedTile, m_TargetRadius);
        Assert.IsTrue(possibleHitTiles != null && possibleHitTiles.Length > 0, " - No possible tiles!");

        List<TileId> randomPool = new List<TileId>();
        for (int i = 0; i < possibleHitTiles.Length; ++i)
        {
            randomPool.Add(possibleHitTiles[i]);
        }

        List<TileId> result = new List<TileId>();
        for (int i = 0; (i < m_NumHitTiles) && (randomPool.Count > 0); ++i)
        {
            int randomNumber = Random.Range(0, randomPool.Count);
            result.Add(randomPool[randomNumber]);
            randomPool.RemoveAt(randomNumber);
        }

        return result;
    }

    public override int CalculateHitChance()
    {
        throw new System.NotImplementedException();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        SetTargetRadius(m_TargetRadius);
        SetNumHitTiles(m_NumHitTiles);
    }
#endif // UNITY_EDITOR
}