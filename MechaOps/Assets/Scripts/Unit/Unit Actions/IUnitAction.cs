﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

/*
 * NOTE ABOUT COMPLETION CALLBACK!
 * 
 * An MOAnimation's CompletionCallback is to signal that the ANIMATION has finished.
 * Therefore we pass MOAnimation's CompletionCallback into the Animator.
 * This is because an MOAnimation is pretty much tied to a MOAnimator.
 * 
 * An IUnitAction's CompletionCallback is to signal that the ACTION has finished.
 * Therefore we DO NOT pass IUnitAction's CompletionCallback to the MOAnimation!
 * Just because an ANIMATION has finished, it does not mean that the IUnitAction has finished!
 * What IUnitAction passes to MOAnimation is its OnAnimationCompleted() function!
*/
[RequireComponent(typeof(UnitStats))]
public abstract class IUnitAction : MonoBehaviour
{
    public enum ActionState
    {
        None,
        Running,
        Paused,
        Completed,
    }

    [SerializeField] private bool m_ControllableAction = true;

    [SerializeField, Tooltip("The component name which will be use to indicate what UI tag to be activated.")]
    protected string m_UnitActionName;
    [SerializeField, Tooltip("This is the description of the action.")]
    protected string m_UnitActionDescription = "<Placeholder Description>";
    [SerializeField, Tooltip("The flag to indicates whether is it turned on.")]
    private bool m_TurnedOn = false;
    [SerializeField, Tooltip("The priority number for this action")]
    private int m_Priority;
    [SerializeField, Tooltip("The action cost.")]
    private int m_ActionCost = 1;
    [SerializeField, Tooltip("Does this action end the turn immediately?")]
    private bool m_EndsTurn = false;
    [SerializeField, Tooltip("The current state that this action update is in")]
    protected ActionState m_ActionState = ActionState.None;

    [SerializeField, Tooltip("The specific action UI")]
    private GameObject m_UnitActionUI;
    [Header("[ Values for Unit Action ]")]
    [SerializeField, Tooltip("The sprite UI for unit's action!")]
    private Sprite m_ActionIconUI;
    [SerializeField, Tooltip("Unit action handler")]
    protected UnitActionHandler m_UnitActionHandler;

    [Tooltip("The unit stats")]
    protected UnitStats m_UnitStats;

    private Void_Void m_CompletionCallBack;

    public bool ControllableAction
    {
        get { return m_ControllableAction; }
    }

    public string UnitActionName
    {
        get { return m_UnitActionName; }
    }

    public string UnitActionDescription
    {
        get { return m_UnitActionDescription; }
    }

    public void TurnOn()
    {
        m_TurnedOn = true;
        OnTurnOn();
    }

    public void TurnOff()
    {
        m_TurnedOn = false;
        OnTurnOff();
    }

    protected virtual void OnTurnOn() { m_ActionState = ActionState.None; }

    protected virtual void OnTurnOff() { m_ActionState = ActionState.None; }

    public bool TurnedOn { get { return m_TurnedOn; } }

    public int Priority
    {
        get { return m_Priority; }
        set { m_Priority = Mathf.Max(0, value); }
    }

    public int ActionCost
    {
        get { return m_ActionCost; }
        set { m_ActionCost = Mathf.Max(0, value); }
    }

    public bool EndsTurn
    {
        get { return m_EndsTurn; }
        set { m_EndsTurn = value; }
    }

    // Those that have no setters is because they should not have to change during runtime.
    // They should only be set in the inspector. If I am wrong and they do need to be changed, add the setters.
    public ActionState GetActionState() { return m_ActionState; }

    public GameObject UnitActionUI { get { return m_UnitActionUI; } }

    public Sprite ActionIconUI { get { return m_ActionIconUI; } }

    public void SetUnitStats(UnitStats _unitStats)
    {
        m_UnitStats = _unitStats;
    }

    public UnitStats GetUnitStats()
    {
        return m_UnitStats;
    }

    public Void_Void CompletionCallBack
    {
        set
        {
            m_CompletionCallBack = value;
        }
        get
        {
            return m_CompletionCallBack;
        }
    }

    protected void InvokeCompletionCallback()
    {
        if (m_CompletionCallBack != null) { m_CompletionCallBack(); }
    }

    /// <summary>
    /// To subscribe to some of the events with the ObserverSystem
    /// </summary>
    protected abstract void InitializeEvents();

    /// <summary>
    /// To unsubscribe from some of the events with the ObserverSystem
    /// </summary>
    protected abstract void DeinitializeEvents();

    /// <summary>
    /// Do note that if the Awake function is overriden, you need to call this function or prepare to face annoying bugs.
    /// </summary>
    protected virtual void Awake()
    {
        // If the unit stat is not linked, get the component of it!
        m_UnitStats = GetComponent<UnitStats>();
        m_UnitActionHandler = GetComponent<UnitActionHandler>();
        Assert.IsNotNull(m_UnitStats, MethodBase.GetCurrentMethod().Name + " - The GameObject this script is attached to MUST have a UnitStats Component!");
        Assert.IsNotNull(m_UnitActionHandler, MethodBase.GetCurrentMethod().Name + " - The GameObject this script is attached to MUST have a m_UnitActionHandler Component!");
        Assert.IsTrue(m_UnitActionName != null && m_UnitActionName != "", "No name is given to this action. GameObject Name: " + gameObject.name);
    }

    /// <summary>
    /// The function should do when turn has started
    /// </summary>
    protected abstract void StartTurnCallback();

    /// <summary>
    /// The function that this should be doing when turn has ended
    /// </summary>
    protected abstract void EndTurnCallback();

    /// <summary>
    /// Check if the condition for this action to run is met.
    /// </summary>
    /// <returns>True if the condition for this action to run is met.</returns>
    public abstract bool VerifyRunCondition();

    public virtual void StartAction()
    {
        m_ActionState = ActionState.Running;
        GetUnitStats().CurrentActionPoints -= ActionCost;
        GetUnitStats().CurrentActiveAction = this;
    }

    /// <summary>
    /// The function that pauses the update of this action.
    /// It is virtual as other actions have different way of pausing
    /// </summary>
    public virtual void PauseAction()
    {
        m_ActionState = ActionState.Paused;
    }

    /// <summary>
    /// The function that resumes the update of this action.
    /// Not the same as StartAction() because StartAction completely resets the action!
    /// </summary>
    public virtual void ResumeAction()
    {
        m_ActionState = ActionState.Running;
    }

    /// <summary>
    /// The function that stops the update of the action.
    /// Not to be confused with PauseAction()!
    /// </summary>
    public virtual void StopAction()
    {
        m_ActionState = ActionState.Completed;
    }

    protected virtual void OnAnimationCompleted() {}

    protected void CheckIfUnitFinishedTurn()
    {
        if (GetUnitStats().CurrentActionPoints == 0 || !m_UnitActionHandler.CheckIsUnitMakeMove(m_UnitStats) || m_EndsTurn)
        {
            // tell the player unit manager that it can no longer do any action
            GameEventSystem.GetInstance().TriggerEvent<GameObject>("UnitMakeMove", gameObject);
        }
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        Priority = m_Priority;
        ActionCost = m_ActionCost;
    }
#endif // UNITY_EDITOR

}