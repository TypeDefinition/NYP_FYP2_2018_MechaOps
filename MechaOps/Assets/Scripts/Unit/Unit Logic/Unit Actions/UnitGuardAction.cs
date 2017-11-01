using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGuardAction : IUnitAction {
    /// <summary>
    /// This is meant for guarding!
    /// </summary>
    /// <returns></returns>
    public override bool StartAction()
    {
        m_UpdateOfUnitAction = StartCoroutine(UpdateActionRoutine());
        ObserverSystemScript.Instance.StoreVariableInEvent("UnitMakeMove", gameObject);
        ObserverSystemScript.Instance.TriggerEvent("UnitMakeMove");
        return true;
    }

    /// <summary>
    /// To popup some UI icons that guard mode is activated!
    /// </summary>
    /// <returns></returns>
    public override IEnumerator UpdateActionRoutine()
    {
        // Needs to at least pop up the UI icon to indicate that it is in guard mode!
        m_UpdateOfUnitAction = null;
        yield break;
    }

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
