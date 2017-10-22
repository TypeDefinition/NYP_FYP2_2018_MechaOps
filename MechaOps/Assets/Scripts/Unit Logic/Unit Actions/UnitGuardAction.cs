using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGuardAction : UnitAction {
    // Use this for initialization
    void Start () {
        if (m_UnitActionName == null)
        {
            m_UnitActionName = "Guard";
        }
        else
        {
            // Just in case the action name is not included!
            switch (m_UnitActionName.Length)
            {
                case 0:
                    m_UnitActionName = "Guard";
                    break;
                default:
                    break;
            }
        }
    }
}
