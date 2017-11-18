using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// To ensure that no one will inherit from it!
/// Provide a base class for 
/// </summary>
[ExecuteInEditMode]
public abstract class IUnitAction : MonoBehaviour
{
    public enum ActionState
    {
        None,
        Running,
        Paused,
        Completed,
    }

    [SerializeField, Tooltip("The component name which will be use to indicate what UI tag to be activated.")]
    protected string m_UnitActionName;
    [SerializeField, Tooltip("The flag to indicates whether is it turned on.")]
    private bool m_TurnedOn = false;
    [SerializeField, Tooltip("The priority number for this action")]
    private int m_Priority;
    [SerializeField, Tooltip("The action cost. For now it will always be 1 but this will be for expandability sake")]
    private int m_ActionCost = 1;
    [SerializeField, Tooltip("Does this action end the turn immediately?")]
    private bool m_EndsTurn = false;
    [SerializeField, Tooltip("The current state that this action update is in")]
    protected ActionState m_ActionState = ActionState.None;
    [SerializeField, Tooltip("The name of the handler to be accessed later")]
    protected string m_NameOfAnim;

    [SerializeField, Tooltip("The specific action UI")]
    private GameObject m_UnitActionUI;
    [Header("[ Values and References for abstract Unit Action ]")]
    [SerializeField, Tooltip("The sprite UI for unit's action!")]
    private Sprite m_ActionIconUI;

    [Header("[ Debugging purpose sake ]")]
    [Tooltip("The unit stats")]
    public UnitStats m_UnitStats;
    [SerializeField, Tooltip("The animation handler")]
    protected AnimationHandler m_AnimHandler;
    [SerializeField, Tooltip("The flag to check is the animation done")]
    protected bool m_AnimDone = false;

    /// <summary>
    /// Most if not all, unit actions will need animation and some sort of delay
    /// </summary>
    protected Coroutine m_UpdateOfUnitAction;

    protected Void_Void m_CompletedCallBack;

    protected string UnitActionName
    {
        get { return m_UnitActionName; }
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

    protected virtual void OnTurnOn() { }

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

    public Void_Void CompletedCallBack
    {
        set
        {
            m_CompletedCallBack = value;
        }
        get
        {
            return m_CompletedCallBack;
        }
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
    /// Do note that if the Awake function is written anew at other children, U need to call this function or prepare to face annoying bug.
    /// </summary>
    protected virtual void Awake()
    {
        // If the unit stat is not linked, get the component of it!
        if (!m_UnitStats)
        {
            m_UnitStats = GetComponent<UnitStats>();
        }
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
    public virtual void StopActionUpdate() {}

    /// <summary>
    /// The actual update of the unit action
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator UpdateActionRoutine()
    {
        m_UpdateOfUnitAction = null;
        yield break;
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
        // Access the animation handler or crash and burn
        if (!m_AnimHandler)
        {
            m_AnimHandler = m_UnitStats.GetAnimHandler(m_NameOfAnim);
        }
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

    protected virtual void CallAnimDone()
    {
        m_AnimDone = true;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Priority = m_Priority;
        ActionCost = m_ActionCost;
    }
#endif // UNITY_EDITOR

}