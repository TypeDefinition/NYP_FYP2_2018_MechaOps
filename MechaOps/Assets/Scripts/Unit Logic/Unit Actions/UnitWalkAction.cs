using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simper unit action for walking!
/// </summary>
public class UnitWalkAction : UnitAction {
    public override bool UseAction()
    {
        m_UpdateOfUnitAction = StartCoroutine(UpdateActionRoutine());
        return true;
    }

    /// <summary>
    /// What is needed here will be the updating of the unit movement and calling for the A* search manager to get a path!.
    /// Followed by the animation!
    /// </summary>
    /// <returns></returns>
    public override IEnumerator UpdateActionRoutine()
    {
        m_UpdateOfUnitAction = null;
        ObserverSystemScript.Instance.StoreVariableInEvent("UnitMakeMove", gameObject);
        ObserverSystemScript.Instance.TriggerEvent("UnitMakeMove");
        yield break;
    }

    // Use this for initialization
    void Start () {
        if (m_UnitActionName == null)
        {
            m_UnitActionName = "Walk";
        }
        else
        {
            // Just in case the action name is not included!
            switch (m_UnitActionName.Length)
            {
                case 0:
                    m_UnitActionName = "Walk";
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// This gets stopped by an action called Overwatch. Maybe
    /// </summary>
    public override void StopActionUpdate()
    {
        base.StopActionUpdate();
    }
}
