using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created with JSON formatter in mind!
/// </summary>
[System.Serializable]
public class UnitStatsJSON
{
    [Tooltip("The view range of the unit")]
    public float viewRange;
    [Tooltip("Minimum attack range of the unit")]
    public float minAttackRange;
    [Tooltip("Maximum attack range of the unit")]
    public float maxAttackRange;
    [Tooltip("The maximum movement point that the unit has left!")]
    public int maxMovementPt;
    /// <summary>
    /// The special getter and setter for movement points left
    /// </summary>
    public int movementPtLeft
    {
        set
        {
            // Minimum value is 0 unless u want to do something special
            m_movementPtLeft = Mathf.Clamp(value, 0, maxMovementPt);
        }
        get
        {
            return m_movementPtLeft;
        }
    }
    [SerializeField, Tooltip("To check how many movement points left and debugging purpose!")]
    protected int m_movementPtLeft;
    [Tooltip("The max healthpoint of the unit")]
    public int maxHealthPt;
    /// <summary>
    /// The setter and getter for health points
    /// </summary>
    public int healthPt
    {
        set
        {
            m_healthPt = Mathf.Clamp(value, 0, maxHealthPt);
        }
        get
        {
            return m_healthPt;
        }
    }
    [SerializeField, Tooltip("The health points of the unit. It is serialized to debug from inspector.")]
    protected int m_healthPt;
    [Tooltip("Max action points of the unit")]
    public int maxActionPt;
    public int actionPtLeft
    {
        set
        {
            m_actionPtLeft = Mathf.Clamp(value, 0, maxActionPt);
        }
        get
        {
            return m_actionPtLeft;
        }
    }
    [SerializeField, Tooltip("The action points left for the unit.")]
    public int m_actionPtLeft;
    [Tooltip("The concealment points of the unit")]
    public int concealmentPt;
    [Tooltip("The evasion points of the unit")]
    public int evasionPt;
    [Tooltip("The accuracy points of the unit")]
    public int accuracyPt;
    [Tooltip("The Deployment cost of the unit")]
    public int deploymentCost;
    [Tooltip("The Attack points of the unit")]
    public int attackPt;
}

public class UnitStatsGameObj : MonoBehaviour {
    public UnitStatsJSON unitStatStuff = new UnitStatsJSON();
}
