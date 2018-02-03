using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class HazardAttributes
{
    [SerializeField] private bool m_Walkable = true;
    [SerializeField] private int m_MovementCost = 0;
    [SerializeField] private string m_Description;

    // There are no setters as this should only be changed in the inspector.
    public bool Walkable { get { return m_Walkable; } }

    public int MovementCost { get { return m_MovementCost; } }

    public string Description { get { return m_Description; } }

    public void Validate()
    {
        m_MovementCost = Mathf.Max(m_MovementCost, 0);
    }
}

public abstract class Hazard : MonoBehaviour
{
    [SerializeField] protected DamageIndicator m_DamageIndicatorPrefab = null;
    [SerializeField] protected GameEventNames m_GameEventNames = null;
    [SerializeField, HideInInspector] private bool m_OwnerInitialized = false;
    [SerializeField, HideInInspector] private Tile m_Owner = null;
    [SerializeField] private HazardAttributes m_Attributes = new HazardAttributes();
    [SerializeField] private bool m_Decay = false;
    [SerializeField] private int m_TurnsToDecay = 3;
    private int m_CurrentTurnsToDecay;
    private FactionType m_FactionTurnWhenCreated = FactionType.None;

    [SerializeField, HideInInspector] private bool m_VisibleToPlayer = false;
    private Void_Bool m_VisibilityCallback = null;
    public Void_Bool VisibilityCallback
    {
        get { return m_VisibilityCallback; }
        set { m_VisibilityCallback = value; }
    }

    public Tile Owner { get { return m_Owner; } }

    public bool Decay
    {
        get { return m_Decay; }
        set { m_Decay = value; }
    }

    public int TurnsToDecay
    {
        get
        {
            return m_TurnsToDecay;
        }
        set
        {
            m_TurnsToDecay = Mathf.Max(1, value);
            m_CurrentTurnsToDecay = Mathf.Clamp(m_CurrentTurnsToDecay, 0, m_TurnsToDecay);
        }
    }

    public int CurrentTurnsToDecay
    {
        get { return m_CurrentTurnsToDecay; }
        set { m_CurrentTurnsToDecay = Mathf.Clamp(value, 0, m_TurnsToDecay); }
    }

    public FactionType FactionTurnWhenCreated
    {
        get { return m_FactionTurnWhenCreated; }
    }

    public bool IsVisibleToPlayer()
    {
        return m_VisibleToPlayer;
    }

    // There are no setters as this should only be changed in the inspector.
    public HazardAttributes Attributes
    {
        get { return m_Attributes; }
    }

    public void InitOwner(Tile _owner)
    {
        Assert.IsTrue(m_OwnerInitialized == false, MethodBase.GetCurrentMethod().Name + " - m_Owner can only be called once per Tile!");

        m_Owner = _owner;
        m_OwnerInitialized = true;
    }

    protected virtual void InitEvents()
    {
        GameEventSystem.GetInstance().SubscribeToEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitMovedToTile), OnUnitMovedToTile);
        GameEventSystem.GetInstance().SubscribeToEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.TurnStart), OnTurnStart);
        GameEventSystem.GetInstance().SubscribeToEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.TurnEnd), OnTurnEnd);
    }

    protected virtual void DeinitEvents()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitMovedToTile), OnUnitMovedToTile);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.TurnStart), OnTurnStart);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.TurnEnd), OnTurnEnd);
    }

    protected virtual void Awake()
    {
        InitEvents();
        m_FactionTurnWhenCreated = GameSystemsDirectory.GetSceneInstance().GetGameFlowManager().CurrentTurnFaction;
    }

    protected virtual void OnDestroy()
    {
        DeinitEvents();
    }

    // Callbacks
    protected abstract void OnUnitMovedToTile(UnitStats _unitStats);

    protected virtual void OnTurnStart(FactionType _faction)
    {
        if (m_Decay && (_faction == m_FactionTurnWhenCreated))
        {
            if ((--CurrentTurnsToDecay) <= 0)
            {
                m_Owner.SetHazardType(HazardType.None);
                m_Owner.LoadHazardType();
            }
        }
    }

    protected virtual void OnTurnEnd(FactionType _faction)
    {
    }

    public virtual void SetVisibleState(bool _visible)
    {
        m_VisibleToPlayer = _visible;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = m_VisibleToPlayer;
        }

        if (m_VisibilityCallback != null)
        {
            m_VisibilityCallback(m_VisibleToPlayer);
        }
    }

    public virtual void CreateDamageIndicator(bool _hit, int _damageValue, GameObject _target)
    {
        Canvas unclickableCanvas = GameSystemsDirectory.GetSceneInstance().GetUnclickableScreenSpaceCanvas();
        DamageIndicator damageIndicator = Instantiate(m_DamageIndicatorPrefab.gameObject, unclickableCanvas.transform).GetComponent<DamageIndicator>();
        damageIndicator.Hit = _hit;
        damageIndicator.DamageValue = _damageValue;
        damageIndicator.Target = _target;
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        m_Attributes.Validate();
        Decay = m_Decay;
        TurnsToDecay = m_TurnsToDecay;
    }
#endif // UNITY_EDITOR
}