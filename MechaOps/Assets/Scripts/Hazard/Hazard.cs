using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private HazardAttributes m_Attributes = new HazardAttributes();
    [SerializeField] private bool m_Decay = false;
    [SerializeField] private int m_TurnsToDecay = 3;
    private int m_CurrentTurnsToDecay;
    [SerializeField] private HazardDisplay m_DisplayObject = null;

    public bool Decay
    {
        get { return m_Decay; }
        set { m_Decay = value; }
    }

    public int TurnsToDecay
    {
        get { return m_TurnsToDecay; }
        set { m_TurnsToDecay = Mathf.Max(1, value); }
    }

    public HazardDisplay DisplayObject
    {
        get { return m_DisplayObject; }
    }

    // There are no setters as this should only be changed in the inspector.
    public HazardAttributes Attributes
    {
        get
        {
            return m_Attributes;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        m_Attributes.Validate();
        Decay = m_Decay;
        TurnsToDecay = m_TurnsToDecay;
    }
#endif // UNITY_EDITOR

    public abstract void ExcecuteHazard(GameObject _unit);

}