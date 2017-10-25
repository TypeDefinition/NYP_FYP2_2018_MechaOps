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

    // There are no setters as this should only be changed in the inspector.
    public HazardAttributes Attributes
    {
        get
        {
            return m_Attributes;
        }
    }

    private void OnValidate()
    {
        m_Attributes.Validate();
    }

    public abstract void ExcecuteHazard(GameObject _unit);

}