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

    // There are no setters as this should only be changed in the inspector.
    public bool Walkable { get { return m_Walkable; } }

    public int MovementCost { get { return m_MovementCost; } }

    public void Validate()
    {
        m_MovementCost = Mathf.Max(m_MovementCost, 0);
    }

}

public abstract class Hazard : MonoBehaviour
{

    [SerializeField, HideInInspector] private bool m_OwnerInitialized = false;
    [SerializeField, HideInInspector] private Tile m_Owner = null;
    [SerializeField] private HazardAttributes m_Attributes = new HazardAttributes();
    [SerializeField] private bool m_Decay = false;
    [SerializeField] private int m_TurnsToDecay = 3;
    private int m_CurrentTurnsToDecay;

    public Tile Owner
    {
        get { return m_Owner; }
    }

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

    // There are no setters as this should only be changed in the inspector.
    public HazardAttributes Attributes
    {
        get { return m_Attributes; }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        m_Attributes.Validate();
        Decay = m_Decay;
        TurnsToDecay = m_TurnsToDecay;
    }
#endif // UNITY_EDITOR

    public void InitOwner(Tile _owner)
    {
        Assert.IsTrue(m_OwnerInitialized == false, MethodBase.GetCurrentMethod().Name + " - m_Owner can only be called once per Tile!");

        m_Owner = _owner;
        m_OwnerInitialized = true;
    }
    
    private void Start()
    {
    }

}