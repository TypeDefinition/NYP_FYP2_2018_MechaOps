using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGuardAction : UnitAction {
    // Use this for initialization
    void Start () {
        if (unitActionName == null)
        {
            unitActionName = "Guard";
        }
        else
        {
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
}
