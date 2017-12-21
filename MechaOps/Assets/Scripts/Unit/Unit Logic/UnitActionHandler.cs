using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To get all of the actions and check whether the unit will be able to make another move by comparing against the all of the actions
/// </summary>
public class UnitActionHandler : MonoBehaviour {
    [Header("Debugging for UnitActionHandler")]
    [SerializeField, Tooltip("All of the actions of this unit")]
    protected IUnitAction[] m_AllAvailableActions;

    public IUnitAction[] AllAvailableActions
    {
        get
        {
            return m_AllAvailableActions;
        }
    }

	// Use this for initialization
	void Awake () {
        m_AllAvailableActions = GetComponentsInChildren<IUnitAction>();
    }

    /// <summary>
    /// Helps to check whether the unit can make any move according to all of it's actions
    /// </summary>
    /// <param name="_UnitStat">Unit Stat to check for the actions points</param>
    /// <returns>true when 1 of the actions can be used</returns>
    public bool CheckIsUnitMakeMove(UnitStats _UnitStat)
    {
        foreach (IUnitAction zeAction in m_AllAvailableActions)
        {
            if (zeAction.ActionCost <= _UnitStat.CurrentActionPoints)
            {
                return true;
            }
        }
        return false;
    }
}
