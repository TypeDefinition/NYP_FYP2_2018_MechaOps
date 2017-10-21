using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackAction : UnitAction
{
    /// <summary>
    /// Optimization will have to come later as this will need to be expanded upon!
    /// </summary>
    /// <param name="other">The opposing target</param>
    /// <returns>Dont know yet!</returns>
    public override bool UseAction(GameObject other)
    {
        UnitStatsGameObj otherUnitStatGO = other.GetComponent<UnitStatsGameObj>();
        if (otherUnitStatGO)
        {
            // Just attack lol
            otherUnitStatGO.unitStatStuff.healthPt -= unitStatGO.unitStatStuff.attackPt;
            return true;
        }
        return false;
    }

    // Use this for initialization
    void Start () {
        if (unitActionName == null)
        {
            unitActionName = "Attack";
        }
        else
        {
            // Just in case the action name is not included!
            switch (unitActionName.Length)
            {
                case 0:
                    unitActionName = "Attack";
                    break;
                default:
                    break;
            }
        }
    }
}
