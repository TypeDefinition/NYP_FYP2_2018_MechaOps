using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackAction : UnitAction
{
    public override void UseAction()
    {
        
    }

    // Use this for initialization
    void Start () {
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
