using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitActionScheduler : MonoBehaviour
{
    private List<IUnitAction> m_ScheduledActions = new List<IUnitAction>();
    private IUnitAction m_CurrentAction = null;

    private void AddActionToSchedule(IUnitAction _action)
    {
        Assert.IsFalse(_action == null, MethodBase.GetCurrentMethod().Name + " - _action should never be null!");

        if (m_ScheduledActions == null)
        {
            m_ScheduledActions.Add(_action);
            return;
        }

        // Insertion Sort
        int actionPriority = _action.Priority;
        for (int i = 0; i < m_ScheduledActions.Count; ++i)
        {
            if (actionPriority > m_ScheduledActions[i].Priority)
            {
                m_ScheduledActions.Insert(i, _action);
                return;
            }
        }

        m_ScheduledActions.Add(_action);
    }

    private IUnitAction GetNextAction()
    {
        if (m_ScheduledActions.Count == 0)
        {
            return null;
        }

        IUnitAction result = m_ScheduledActions[0];
        m_ScheduledActions.RemoveAt(0);
        return result;
    }
    
    private bool CheckActionValid(IUnitAction _action)
    {
        if (_action == null)
        {
            return false;
        }

        if (_action.TurnedOn == false)
        {
            return false;
        }

        if (_action.GetActionState() == IUnitAction.ActionState.Completed)
        {
            m_ScheduledActions.Remove(_action);
            _action.TurnOff();
            m_CurrentAction = null;
            return false;
        }

        if (_action.VerifyRunCondition() == false)
        {
            return false;
        }

        return true;
    }

    private void Update()
    {
        if (!CheckActionValid(m_CurrentAction))
        {
            m_CurrentAction = GetNextAction();
        }

        if (m_CurrentAction == null)
        {
            return;
        }

        switch (m_CurrentAction.GetActionState())
        {
            case IUnitAction.ActionState.None:
                m_CurrentAction.StartAction();
                break;
            case IUnitAction.ActionState.Running:
                // Do nothing. Let the action run.
                break;
            case IUnitAction.ActionState.Paused:
                m_CurrentAction.ResumeAction();
                break;
            case IUnitAction.ActionState.Completed:
                Assert.IsTrue(false, MethodBase.GetCurrentMethod().Name + " - m_CurrentAction's Action State should not be Completed!");
                break;
            default:
                Assert.IsTrue(false, MethodBase.GetCurrentMethod().Name + " - m_CurrentAction's Action State is unknown!");
                break;
        }
    }

    // Interface Function(s)
    public void ScheduleAction(IUnitAction _action)
    {
        Assert.IsTrue(CheckActionValid(_action), MethodBase.GetCurrentMethod().Name + " - Cannot schedule an invalid action!");
        AddActionToSchedule(_action);
    }

}