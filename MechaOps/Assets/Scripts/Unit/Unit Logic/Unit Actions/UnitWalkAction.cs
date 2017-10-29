using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simper unit action for walking!
/// </summary>
public class UnitWalkAction : UnitAction {
    [Header("References and variables needed for unit walk")]
    [Tooltip("The number of tiles it can move")]
    public int m_MovementPoints;
    [Tooltip("The speed to move from 1 point to another. For animation purpose.")]
    public float m_Speed = 10.0f;

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
        m_UnitStatGO.m_UnitStatsJSON.CurrentActionPoints--;
        switch (m_UnitStatGO.m_UnitStatsJSON.CurrentActionPoints)
        {
            case 0:
                // tell the player unit manager that it can no longer do any action
                break;
            default:
                break;
        }
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
