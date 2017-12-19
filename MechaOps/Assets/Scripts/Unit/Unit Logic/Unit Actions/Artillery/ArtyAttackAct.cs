using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For the artillery unit attack action logic
/// </summary>
public class ArtyAttackAct : UnitAttackAction {
    [Header("Variables for ArtyAttackAct")]
    [SerializeField, Tooltip("The Exploding radius of attack")]
    protected int m_ExplodeRadius = 1;

    [Header("Debugging for ArtyAttackAct")]
    [SerializeField, Tooltip("TileId target")]
    protected TileId m_TargetTile;
    [SerializeField, Tooltip("Surrounding tile of the target going to be set by the Arty Attack act")]
    protected TileId[] m_ExplodingTiles;
    
    /// <summary>
    /// Getter for m_ExplodeRadius
    /// </summary>
    public int ExplodeRadius
    {
        get
        {
            return m_ExplodeRadius;
        }
    }

    /// <summary>
    /// to set the exploding tiles
    /// </summary>
    /// <param name="_tiles">the array of tiles that are going to explode</param>
    public void SetExplodingTiles(TileId[] _tiles)
    {
        m_ExplodingTiles = _tiles;
    }

    public override void SetTarget(GameObject _target)
    {
        m_TargetTile = _target.GetComponent<Tile>().GetTileId();
    }

    /// <summary>
    /// Since it will be different from the normal unit attack action!
    /// </summary>
    /// <returns></returns>
    public override IEnumerator UpdateActionRoutine()
    {
        TileSystem zeTileSys = FindObjectOfType<TileSystem>();
        GetUnitStats().CurrentActionPoints -= ActionCost;

        // TODO: the animation of the artillery bombing
        // TODO: the calculation of the artillery accuracy
        // then loop through the tileID array and make the units in it explode!
        m_ExplodingTiles = zeTileSys.GetSurroundingTiles(m_TargetTile, m_ExplodeRadius);
        foreach (TileId zeTileID in m_ExplodingTiles)
        {
            Tile zeTile = zeTileSys.GetTile(zeTileID);
            // if the unit is not null
            if (zeTile.Unit)
            {
                UnitStats zeVictimStat = zeTile.Unit.GetComponent<UnitStats>();
                zeVictimStat.CurrentHealthPoints -= m_DamagePoints;
                if (zeVictimStat.m_HealthDropCallback != null)
                    zeVictimStat.m_HealthDropCallback.Invoke(m_UnitStats);
            }
        }
        switch (GetUnitStats().CurrentActionPoints)
        {
            case 0:
                GetUnitStats().ResetUnitStats();
                GameEventSystem.GetInstance().TriggerEvent<GameObject>("UnitMakeMove", gameObject);
                break;
            default:
                break;
        }
        GameEventSystem.GetInstance().TriggerEvent("UnitFinishAction");
        m_ActionState = ActionState.Completed;
        m_AnimationCompleted = false;
        m_UpdateOfUnitAction = null;
        yield break;
    }

    public override bool VerifyRunCondition()
    {
        if (m_TargetTile == null)
            return false;
        int distanceToTarget = TileId.GetDistance(m_TargetTile, GetUnitStats().CurrentTileID);
        if (distanceToTarget > m_MaxAttackRange || distanceToTarget < m_MinAttackRange)
        {
            return false;
        }
        return true;
    }
}
