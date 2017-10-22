using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created with JSON formatter in mind!
/// </summary>
[System.Serializable]
public class UnitStatsJSON
{
    [Tooltip("Name of the unit")]
    public string m_NameOfUnit;
    [Tooltip("The view range of the unit")]
    public float m_ViewRange;
    [Tooltip("Minimum attack range of the unit")]
    public float m_MinAttackRange;
    [Tooltip("Maximum attack range of the unit")]
    public float m_MaxAttackRange;
    [Tooltip("The maximum movement point that the unit has left!")]
    public int m_MaxMovementPoints;
    [Tooltip("The max healthpoint of the unit")]
    public int m_MaxHealthPt;
    [SerializeField, Tooltip("The health points of the unit. It is serialized to debug from inspector.")]
    protected int m_HealthPt;
    [Tooltip("Max action points of the unit")]
    public int m_MaxActionPt;
    [SerializeField, Tooltip("The action points left for the unit.")]
    public int m_ActionPtLeft;
    [Tooltip("The concealment points of the unit")]
    public int m_ConcealmentPt;
    [Tooltip("The evasion points of the unit")]
    public int m_EvasionPt;
    [Tooltip("The accuracy points of the unit")]
    public int m_AccuracyPt;
    [Tooltip("The Deployment cost of the unit")]
    public int m_DeploymentCost;
    [Tooltip("The Attack points of the unit")]
    public int m_AttackPt;
    [Tooltip("The reference to the class")]
    public GameObject m_UnitStatGO;

    // Debug
    [SerializeField, Tooltip("To check how many movement points left and debugging purpose!")]
    protected int m_MovementPtLeft;

    /// <summary>
    /// The special getter and setter for movement points left
    /// </summary>
    public int MovementPtLeft {
        set {
            // Minimum value is 0 unless u want to do something special
            m_MovementPtLeft = Mathf.Clamp(value, 0, m_MaxMovementPoints);
        }
        get {
            return m_MovementPtLeft;
        }
    }

    /// <summary>
    /// The setter and getter for health points
    /// </summary>
    public int HealthPt
    {
        set
        {
            m_HealthPt = Mathf.Clamp(value, 0, m_MaxHealthPt);
            if (m_HealthPt == 0)
            {
                GameObject.Destroy(m_UnitStatGO);
            }
        }
        get
        {
            return m_HealthPt;
        }
    }
    
    public int ActionPtLeft
    {
        set
        {
            m_ActionPtLeft = Mathf.Clamp(value, 0, m_MaxActionPt);
        }
        get
        {
            return m_ActionPtLeft;
        }
    }
    
}

public class UnitStats : MonoBehaviour {

    [Header("The references of the ")]
    [Tooltip("The unit stats information")]
    public UnitStatsJSON m_UnitStatsJSON = new UnitStatsJSON();

    private void Start()
    {
        // Assign the gameobject name to the unit if there is none for the unit stat!
        if (m_UnitStatsJSON.m_NameOfUnit == null)
        {
            m_UnitStatsJSON.m_NameOfUnit = name;
        }
        m_UnitStatsJSON.m_UnitStatGO = gameObject;
    }

    private void OnDestroy()
    {
        ObserverSystemScript.Instance.StoreVariableInEvent(tag + "IsDead", gameObject);
        // Trigger an event when the unit died
        ObserverSystemScript.Instance.TriggerEvent(tag + "IsDead");
    }
}
