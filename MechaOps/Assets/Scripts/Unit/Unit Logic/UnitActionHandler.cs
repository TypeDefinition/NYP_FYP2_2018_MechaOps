using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To get all of the actions and check whether the unit will be able to make another move by comparing against the all of the actions
/// </summary>
public class UnitActionHandler : MonoBehaviour
{
    protected IUnitAction[] m_AllActions;

    public IUnitAction[] AllActions { get { return m_AllActions; } }

	void Awake ()
    {
        m_AllActions = GetComponentsInChildren<IUnitAction>();
    }

    /// <summary>
    /// Helps to check whether the unit can make any move according to all of it's actions
    /// </summary>
    /// <param name="_UnitStat">Unit Stat to check for the actions points</param>
    /// <returns>true when 1 of the actions can be used</returns>
    public bool CheckCanUnitDoAction(UnitStats _UnitStat)
    {
        foreach (IUnitAction action in m_AllActions)
        {
            if (action.ControllableAction == false) { continue; }
            if (action.ActionCost <= _UnitStat.CurrentActionPoints) { return true; }
        }

        return false;
    }
}