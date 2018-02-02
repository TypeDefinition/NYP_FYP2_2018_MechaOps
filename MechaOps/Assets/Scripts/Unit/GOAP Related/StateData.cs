using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This will be doing some state checking then report back to GoapPlanner or anyone that is listening to it's function!
/// </summary>
public class StateData : MonoBehaviour {
    [Header("Debugging purpose")]
    [SerializeField, Tooltip("Own unit stat")]
    protected UnitStats m_Stat;
    [SerializeField, Tooltip("The Attacker stats")]
    protected UnitStats m_AttackerStat;
    [SerializeField, Tooltip("Goap Planner")]
    protected GoapPlanner m_Planner;
    [SerializeField, Tooltip("The Game Event asset")]
    protected GameEventNames m_EventNameAsset;

    protected HashSet<string> m_CurrentStates = new HashSet<string>();

    public HashSet<string> CurrentStates
    {
        get
        {
            return m_CurrentStates;
        }
    }

    protected virtual void Awake()
    {
        m_Planner = GetComponent<GoapPlanner>();
    }

    protected virtual void OnEnable()
    {
        m_Stat = GetComponent<UnitStats>();
        if (!m_EventNameAsset)
        {
            m_EventNameAsset = GameSystemsDirectory.GetSceneInstance().GetGameEventNames();
        }
        // Use Game Event System instead. -Terry
        GameEventSystem.GetInstance().SubscribeToEvent<UnitStats, UnitStats>(m_EventNameAsset.GetEventName(GameEventNames.GameplayNames.AttackedUnit), GetAttackerGO);
    }


    protected virtual void OnDisable()
    {
        // Use Game Event System instead. -Terry
        GameEventSystem.GetInstance().UnsubscribeFromEvent<UnitStats, UnitStats>(m_EventNameAsset.GetEventName(GameEventNames.GameplayNames.AttackedUnit), GetAttackerGO);
    }

    /// <summary>
    /// Getting the attacking 
    /// </summary>
    /// <param name="_attacker"></param>
    public void GetAttackerGO(UnitStats _attacker)
    {
        m_AttackerStat = _attacker;
        m_CurrentStates.Add("UnderAttack");
    }

    public void GetAttackerGO(UnitStats _attacker, UnitStats _victim)
    {
        if (_victim.gameObject == gameObject)
        {
            GetAttackerGO(_attacker);
        }
    }

    public void StartInitState()
    {
        if (m_Planner.EnemiesManager.GetSeenEnemies().Count > 0)
        {
            m_CurrentStates.Add("TargetInView");
        }
        else
            CurrentStates.Remove("TargetInView");
    }
}
