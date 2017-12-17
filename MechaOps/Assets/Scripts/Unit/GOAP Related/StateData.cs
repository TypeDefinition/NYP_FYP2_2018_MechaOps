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

    protected HashSet<string> m_CurrentStates = new HashSet<string>();

    public HashSet<string> CurrentStates
    {
        get
        {
            return m_CurrentStates;
        }
    }

    protected virtual void OnEnable()
    {
        m_Stat = GetComponent<UnitStats>();
        // need to listen for being attacked!
        m_Stat.m_HealthDropCallback += GetAttackerGO;
    }


    protected virtual void OnDisable()
    {
        if (m_Stat)
            // need to listen for being attacked!
            m_Stat.m_HealthDropCallback -= GetAttackerGO;
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

    public void StartInitState()
    {
        if (m_Stat.EnemiesInRange.Count > 0)
        {
            m_CurrentStates.Add("TargetInView");
        }
        else
            CurrentStates.Remove("TargetInView");
    }
}
