using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// What it does is that the Unit UI Action will store the action from the unit itself!
/// </summary>
public class UnitActionUILogic : MonoBehaviour {
    [Tooltip("To get the unit's action reference")]
    public UnitAction m_unitActionRef;

    /// <summary>
    /// It will only set the UI to be active according to m_unitActionRef.unitActionName if the tag of the UI is the same!
    /// </summary>
    public void ActivateGameObjWithTag()
    {
        GameUI_Manager.Instance.SetTheGameObjTagActive(m_unitActionRef.unitActionName);
    }
}
