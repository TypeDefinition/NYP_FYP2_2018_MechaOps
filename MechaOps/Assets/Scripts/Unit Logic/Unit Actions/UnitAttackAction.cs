using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackAction : UnitAction
{
    /// <summary>
    /// Optimization will have to come later as this will need to be expanded upon!
    /// </summary>
    /// <param name="_other">The opposing target</param>
    /// <returns>Dont know yet!</returns>
    public override bool UseAction(GameObject _other)
    {
        UnitStats otherUnitStatGO = _other.GetComponent<UnitStats>();
        if (otherUnitStatGO)
        {
            // Just attack lol
            // Damage should be tied to action, not unit.
            // otherUnitStatGO.m_UnitStatsJSON.HealthPt -= m_UnitStatGO.m_UnitStatsJSON.m_AttackPt;
            ObserverSystemScript.Instance.StoreVariableInEvent("UnitMakeMove", gameObject);
            ObserverSystemScript.Instance.TriggerEvent("UnitMakeMove");
            return true;
        }
        return false;
    }

    // Use this for initialization
    void Start () {
        if (m_UnitActionName == null)
        {
            m_UnitActionName = "Attack";
        }
        else
        {
            // Just in case the action name is not included!
            switch (m_UnitActionName.Length)
            {
                case 0:
                    m_UnitActionName = "Attack";
                    break;
                default:
                    break;
            }
        }
    }
}