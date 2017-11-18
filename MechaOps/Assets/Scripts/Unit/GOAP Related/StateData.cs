using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This will be doing some state checking then report back to GoapPlanner or anyone that is listening to it's function!
/// </summary>
public class StateData : MonoBehaviour {
    [Header("Debugging purpose")]
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
        // Since there is no need for 
        UnitStats zeStat = GetComponent<UnitStats>();
        // need to listen for being attacked!
        zeStat.m_HealthDropCallback += GetAttackerGO;
    }


    protected virtual void OnDisable()
    {
        UnitStats zeStat = GetComponent<UnitStats>();
        if (zeStat)
        // need to listen for being attacked!
            zeStat.m_HealthDropCallback -= GetAttackerGO;
    }

    /// <summary>
    /// Getting the attacking 
    /// </summary>
    /// <param name="_attacker"></param>
    protected void GetAttackerGO(UnitStats _attacker)
    {
        m_AttackerStat = _attacker;
        m_CurrentStates.Add("UnderAttack");
    }
}
