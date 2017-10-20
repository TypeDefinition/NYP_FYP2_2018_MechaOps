using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simper unit action for walking!
/// </summary>
public class UnitWalkAction : UnitAction {
    /// <summary>
    /// using it for walking!
    /// </summary>
    public override void UseAction()
    {
        
    }

    // Use this for initialization
    void Start () {
        // Just in case the action name is not included!
        switch (unitActionName.Length)
        {
            case 0:
                unitActionName = "Walk";
                break;
            default:
                break;
        }
    }
}
