using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ActionState
{
    None,
    Running,
    Paused,
    Completed,
    Num_ActionState,
}

/// <summary>
/// To ensure that no one will inherit from it!
/// Provide a base class for 
/// </summary>
[ExecuteInEditMode]
public abstract class IUnitAction : MonoBehaviour
{
    [Header("[ Values and References for abstract Unit Action ]")]
    [Tooltip("The sprite UI for unit's action!")]
    public Sprite m_ActionIconUI;
    [Tooltip("The action cost. For now it will always be 1 but this will be for expandability sake")]
    public int m_ActionCost = 1;
    [Tooltip("The component name which will be use to indicate what UI tag to be activated.")]
    public string m_UnitActionName;
    [Tooltip("The priority number for this action")]
    public int m_Priority;
    [Tooltip("The current state that this action update is in")]
    public ActionState m_ActionState = ActionState.None;
    [Tooltip("The specific action UI")]
    public GameObject m_UnitActionUI;

    [Header("[ Debugging purpose sake ]")]
    [SerializeField, Tooltip("The unit stats")]
    public UnitStats m_UnitStatGO;
    [SerializeField, Tooltip("The flag to indicates whether is it activated")]
    protected bool m_Activated = false;

    /// <summary>
    /// Most if not all, unit actions will need animation and some sort of delay
    /// </summary>
    protected Coroutine m_UpdateOfUnitAction;
    /// <summary>
    /// The getter and setter for m_Activated. The setter is meant to do something special
    /// </summary>
    protected bool Activated
    {
        set
        {
            m_Activated = value;
        }
        get
        {
            return m_Activated;
        }
    }

    /// <summary>
    /// To subscribe to some of the events with the ObserverSystem
    /// </summary>
    protected virtual void InitializeEvents()
    {

    }

    /// <summary>
    /// To unsubscribe from some of the events with the ObserverSystem
    /// </summary>
    protected virtual void DeinitializeEvents()
    {

    }

    /// <summary>
    /// Do note that if the Awake function is written anew at other children, U need to call this function or prepare to face annoying bug.
    /// </summary>
    protected virtual void Awake()
    {
        // If the unit stat is not linked, get the component of it!
        if (!m_UnitStatGO)
            m_UnitStatGO = GetComponent<UnitStats>();
    }

    /// <summary>
    /// The function to start the coroutine of updating.
    /// It is virtual just in case it needs to be overridden.
    /// </summary>
    public virtual void StartUpdating()
    {
        m_UpdateOfUnitAction = StartCoroutine(UpdateActionRoutine());
    }

    /// <summary>
    /// Since not all action needs to be stopped,
    /// this will be a virtual function ready to be inherited
    /// </summary>
    public virtual void StopActionUpdate()
    {

    }

    /// <summary>
    /// The function should do when turn has started
    /// </summary>
    public virtual void StartTurn()
    {

    }

    /// <summary>
    /// The function that this should be doing when turn has ended
    /// </summary>
    public virtual void EndTurn()
    {

    }

    /// <summary>
    /// Check if this action is running
    /// </summary>
    /// <returns>True if current state is Running</returns>
    public bool VerifyRunCondition()
    {
        switch (m_ActionState)
        {
            case ActionState.Running:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// The actual update of the unit action
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator UpdateActionRoutine()
    {
        m_UpdateOfUnitAction = null;
        yield break;
    }

    public virtual bool StartAction()
    {
        return false;
    }
    public virtual bool StartAction(GameObject _other)
    {
        return false;
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
}