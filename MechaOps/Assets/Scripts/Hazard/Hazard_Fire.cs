using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard_Fire : Hazard
{
    [SerializeField] private int m_Damage = 1;
    UnitStats m_UnitOnTile = null;
    ParticleSystem m_ParticleSystem = null;

    public int Damage
    {
        get { return m_Damage; }
        set { m_Damage = Mathf.Max(0, value); }
    }

    // Callbacks
    protected override void OnUnitMovedToTile(UnitStats _unitStats)
    {
        base.OnUnitMovedToTile(_unitStats);
        if (_unitStats.CurrentTileID.Equals(Owner.GetTileId()) && _unitStats.IsAlive())
        {
            m_UnitOnTile = _unitStats;
            m_UnitOnTile.CurrentHealthPoints -= Damage;
        }
    }

    protected override void OnTurnStart(FactionType _faction)
    {
        base.OnTurnStart(_faction);
    }

    protected override void OnTurnEnd(FactionType _faction)
    {
        base.OnTurnEnd(_faction);

        if (m_UnitOnTile == null)
        {
            return;
        }

        if (m_UnitOnTile.CurrentTileID.Equals(Owner.GetTileId()) == false || m_UnitOnTile.IsAlive() == false)
        {
            m_UnitOnTile = null;
            return;
        }

        if (_faction == FactionTurnWhenCreated)
        {
            m_UnitOnTile.CurrentHealthPoints -= Damage;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        m_ParticleSystem = GetComponent<ParticleSystem>();
    }

    public virtual void Update()
    {
        // This cannot be done is SetVisibleState as SetVisibleState can get called multiple times per frame,
        // since all tiles are 'unseen' before being reseen.
        // This will cause the particle system to clear its particles even though at the end of the frame the tile is seen.
        if (m_ParticleSystem != null && m_ParticleSystem.isPlaying != IsVisibleToPlayer())
        {
            if (IsVisibleToPlayer())
            {
                m_ParticleSystem.Play();
            }
            else
            {
                m_ParticleSystem.Stop();
                m_ParticleSystem.Clear();
            }
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        Damage = m_Damage;
    }
#endif // UNITY_EDITOR
}