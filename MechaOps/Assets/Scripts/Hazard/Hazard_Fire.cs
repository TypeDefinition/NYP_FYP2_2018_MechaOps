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
        if (m_UnitOnTile == null)
        {
            // The unit just moved onto this.
            if (_unitStats.CurrentTileID.Equals(Owner.GetTileId()) && _unitStats.IsAlive())
            {
                m_UnitOnTile = _unitStats;
                m_UnitOnTile.CurrentHealthPoints -= Damage;
                CreateDamageIndicator(true, Damage, m_UnitOnTile.gameObject);
            }
        }
        else
        {
            // The unit is moving away.
            if (m_UnitOnTile == _unitStats && !_unitStats.CurrentTileID.Equals(Owner.GetTileId()))
            {
                m_UnitOnTile = null;
            }
        }
    }

    protected override void OnTurnStart(FactionType _faction)
    {
        if (m_UnitOnTile == null)
        {
            base.OnTurnStart(_faction);
            return;
        }

        if (!m_UnitOnTile.CurrentTileID.Equals(Owner.GetTileId()) ||
            !m_UnitOnTile.IsAlive())
        {
            m_UnitOnTile = null;
            base.OnTurnStart(_faction);
            return;
        }

        if (_faction != FactionTurnWhenCreated)
        {
            base.OnTurnStart(_faction);
            return;
        }

        m_UnitOnTile.CurrentHealthPoints -= Damage;
        CreateDamageIndicator(true, Damage, m_UnitOnTile.gameObject);
        base.OnTurnStart(_faction);
    }

    protected override void Awake()
    {
        base.Awake();
        m_ParticleSystem = GetComponent<ParticleSystem>();
    }

    protected virtual void Start()
    {
        if (Owner.Unit != null)
        {
            UnitStats unitStats = Owner.Unit.GetComponent<UnitStats>();
            if (unitStats != null)
            {
                m_UnitOnTile = unitStats;
            }
        }
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