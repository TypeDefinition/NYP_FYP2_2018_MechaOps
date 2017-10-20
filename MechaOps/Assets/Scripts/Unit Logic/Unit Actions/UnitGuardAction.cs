using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGuardAction : UnitAction {
    public override void UseAction()
    {
        throw new System.NotImplementedException();
    }

    // Use this for initialization
    void Start () {
        // Just in case the action name is not included!
        switch (unitActionName.Length)
        {
            case 0:
                unitActionName = "Guard";
                break;
            default:
                break;
        }
    }
}
