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

        // Insertion Sort
        for (int i = 0; i < m_ScheduledActions.Count; ++i)
        {
            if (_action.Priority > m_ScheduledActions[i].Priority)
            {
                m_ScheduledActions.Insert(i, _action);
                return;
            }
        }

        m_ScheduledActions.Add(_action);
    }

    private IUnitAction GetNextAction()
    {
        if (m_ScheduledActions.Count == 0) { return null; }

        IUnitAction result = m_ScheduledActions[0];
        m_ScheduledActions.RemoveAt(0);
        return result;
    }

    private bool ValidateScheduledAction(IUnitAction _action)
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
            return false;
        }

        if (_action.VerifyRunCondition() == false)
        {
            return false;
        }

        return true;
    }

    private bool CheckCurrentActionValid()
    {
        if (m_CurrentAction == null) { return false; }

        if (m_CurrentAction.TurnedOn == false) { return false; }

        if (m_CurrentAction.VerifyRunCondition() == false) { return false; }

        return true;
    }

    private void UpdateCurrentAction()
    {
        if (m_CurrentAction)
        {
            if (m_CurrentAction.GetActionState() == IUnitAction.ActionState.Completed)
            {
                m_CurrentAction.TurnOff();
                m_CurrentAction = GetNextAction();
            }
            // If the current action is no longer valid, turn it off.
            else if (CheckCurrentActionValid() == false)
            {
                if (m_CurrentAction.GetActionState() == IUnitAction.ActionState.Running || m_CurrentAction.GetActionState() == IUnitAction.ActionState.Paused)
                {
                    m_CurrentAction.StopAction();
                }

                m_CurrentAction.TurnOff();
                m_CurrentAction = GetNextAction();
            }
            // If there is an action with higher priority, run that first.
            else if (m_ScheduledActions.Count > 0 && m_ScheduledActions[0].Priority > m_CurrentAction.Priority)
            {
                // Get the next action.
                IUnitAction nextAction = m_ScheduledActions[0];
                m_ScheduledActions.RemoveAt(0);

                // Pause the current action.
                m_CurrentAction.PauseAction();
                AddActionToSchedule(m_CurrentAction);
                m_CurrentAction = nextAction;
            }
        }
        else
        {
            m_CurrentAction = GetNextAction();
        }
    }

    private void Update()
    {
        UpdateCurrentAction();

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
        if (ValidateScheduledAction(_action))
        {
            AddActionToSchedule(_action);
        }
        else
        {
            _action.TurnOff();
        }
    }
}