using System.Collections;
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
[RequireComponent(typeof(UnitStats)), RequireComponent(typeof(UnitActionHandler))]
public abstract class IUnitAction : MonoBehaviour
{
    public enum ActionState
    {
        None,
        Running,
        Paused,
        Completed,
    }

    // Serialised Variable(s)
    [SerializeField, Tooltip("The component name which will be use to indicate what UI tag to be activated.")]
    private string m_UnitActionName;
    [SerializeField, Tooltip("This is the description of the action.")]
    private string m_UnitActionDescription = "<Placeholder Description>";
    [SerializeField] private bool m_ControllableAction = true;
    [SerializeField, Tooltip("The priority number for this action")]
    private int m_Priority = 0;
    [SerializeField, Tooltip("The action cost.")]
    private int m_ActionCost = 1;
    [SerializeField, Tooltip("Does this action end the turn immediately?")]
    private bool m_EndsTurn = false;
    [SerializeField, Tooltip("How many turns this cooldown has after being turned on.")]
    private int m_CooldownTurns = 0;

    [SerializeField, Tooltip("The Unit Action UI Prefab")]
    private UnitActionUI m_UnitActionUIPrefab = null;
    [SerializeField, Tooltip("The sprite UI for unit's action!")]
    private Sprite m_UnitActionButtonSprite = null;

    // Non-Serialised Variable(s)
    protected UnitActionHandler m_UnitActionHandler = null;
    private bool m_TurnedOn = false;
    protected UnitStats m_UnitStats = null;
    protected GameEventNames m_GameEventNames = null;
    protected GameFlowManager m_GameFlowManager = null;
    protected ActionState m_ActionState = ActionState.None;
    protected bool m_IsUnitTurn = false;

    protected int m_CooldownTurnsLeft = 0;
    // I could just +1 to m_CooldownTurnsLeft in OnTurnOn, but I feel like
    // this is clearer. I understand that his is fully debatable.
    protected bool m_IsFirstTurnSinceTurnedOn = false;

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

    public int CooldownTurns
    {
        get { return m_CooldownTurns; }
    }

    public int CooldownTurnsLeft
    {
        get { return m_CooldownTurnsLeft; }
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

    protected virtual void OnTurnOn()
    {
        m_ActionState = ActionState.None;
        m_CooldownTurnsLeft = m_CooldownTurns;
        m_IsFirstTurnSinceTurnedOn = true;
    }

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

    public UnitActionUI GetUnitActionUI() { return m_UnitActionUIPrefab; }

    public Sprite ActionIconUI { get { return m_UnitActionButtonSprite; } }

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
    protected virtual void InitEvents()
    {
        GameEventSystem.GetInstance().SubscribeToEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.TurnStart), OnTurnStart);
        GameEventSystem.GetInstance().SubscribeToEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.TurnEnd), OnTurnEnd);
    }

    /// <summary>
    /// To unsubscribe from some of the events with the ObserverSystem
    /// </summary>
    protected virtual void DeinitEvents()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.TurnStart), OnTurnStart);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.TurnEnd), OnTurnEnd);
    }

    /// <summary>
    /// Do note that if the Awake function is overriden, you need to call this function or prepare to face annoying bugs.
    /// </summary>
    protected virtual void Awake()
    {
        // If the unit stat is not linked, get the component of it!
        m_GameEventNames = GameSystemsDirectory.GetSceneInstance().GetGameEventNames();
        Assert.IsNotNull(m_GameEventNames);
        m_UnitStats = GetComponent<UnitStats>();
        Assert.IsNotNull(m_UnitStats, MethodBase.GetCurrentMethod().Name + " - The GameObject this script is attached to MUST have a UnitStats Component!");
        m_UnitActionHandler = GetComponent<UnitActionHandler>();
        Assert.IsNotNull(m_UnitActionHandler, MethodBase.GetCurrentMethod().Name + " - The GameObject this script is attached to MUST have a m_UnitActionHandler Component!");
        Assert.IsTrue(m_UnitActionName != null && m_UnitActionName != "", "No name is given to this action. GameObject Name: " + gameObject.name);
        m_GameFlowManager = GameSystemsDirectory.GetSceneInstance().GetGameFlowManager();

        InitEvents();
    }

    protected virtual void OnDestroy()
    {
        DeinitEvents();
    }
    
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

        if (m_UnitStats.GetViewScript().IsVisible() || m_UnitStats.UnitFaction == m_GameFlowManager.PlayerFaction)
        {
            GameEventSystem.GetInstance().TriggerEvent<GameObject>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.FocusOnTarget), m_UnitStats.gameObject);
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

        if (m_UnitStats.GetViewScript().IsVisible() || m_UnitStats.UnitFaction == m_GameFlowManager.PlayerFaction)
        {
            GameEventSystem.GetInstance().TriggerEvent<GameObject>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.FocusOnTarget), m_UnitStats.gameObject);
        }
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

    protected virtual void OnTurnStart(FactionType _factionType)
    {
        if (_factionType == m_UnitStats.UnitFaction)
        {
            m_IsUnitTurn = true;
        }
    }

    protected virtual void OnTurnEnd(FactionType _factionType)
    {
        if (_factionType == m_UnitStats.UnitFaction)
        {
            m_IsUnitTurn = false;
            if (!m_IsFirstTurnSinceTurnedOn)
            {
                m_CooldownTurnsLeft = Mathf.Max(0, m_CooldownTurnsLeft - 1);
            }
            m_IsFirstTurnSinceTurnedOn = false;
        }
    }

    protected void CheckIfUnitFinishedTurn()
    {
        // We cannot finish the turn if it isn't even our turn to begin with.
        if (!m_IsUnitTurn) { return; }

        if (GetUnitStats().CurrentActionPoints == 0 || !m_UnitActionHandler.CheckCanUnitDoAction(m_UnitStats) || m_EndsTurn)
        {
            // tell the player unit manager that it can no longer do any action
            GetUnitStats().CurrentActionPoints = 0;
            GameEventSystem.GetInstance().TriggerEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitFinishedTurn), m_UnitStats);
        }
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        Priority = m_Priority;
        ActionCost = m_ActionCost;
        m_CooldownTurns = Mathf.Max(0, m_CooldownTurns);
    }
#endif // UNITY_EDITOR

}