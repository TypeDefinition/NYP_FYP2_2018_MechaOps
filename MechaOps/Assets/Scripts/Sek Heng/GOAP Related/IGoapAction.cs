using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GoapActEffect
{
    public string m_EffectName;
    public int m_EffectCost;
}

public abstract class IGoapAction : MonoBehaviour {
    [Header("References required for IGoapAction ")]
    [SerializeField, Tooltip("The cost to do this action")]
    protected int m_Cost = 2;
    [Tooltip("The action name")]
    public string m_ActName;
    [Tooltip("If there is any other action required, put the name down")]
    public string[] m_NamePrecActs;
    [SerializeField, Tooltip("GOAP Planner. Will attempt to get component if there is no linking")]
    protected GoapPlanner m_Planner;
    [SerializeField, Tooltip("The list of effects for this action")]
    protected GoapActEffect[] m_AllEffect;

    public Coroutine m_UpdateRoutine
    {
        protected set; get;
    }

    /// <summary>
    /// This will be needed to add to 
    /// </summary>
    protected Dictionary<string, GoapActEffect> m_Effects = new Dictionary<string, GoapActEffect>();
    protected Dictionary<string, IGoapAction> m_Preconditions = new Dictionary<string, IGoapAction>();

    /// <summary>
    /// Getter for cost
    /// </summary>
    /// <returns></returns>
    public virtual int GetCost()
    {
        return m_Cost;
    }

    // Use this for initialization
    protected virtual void Start () {
        if (!m_Planner)
            m_Planner = GetComponent<GoapPlanner>();

        //foreach (string zePreconditionAct in m_NamePrecActs)
        //{
        //    IGoapAction zeAct = m_Planner.GetGoapAct(zePreconditionAct);
        //    if (zeAct)
        //    {
        //        m_Preconditions.Add(zePreconditionAct, zeAct);
        //    }
        //}
        foreach (GoapActEffect zeEffect in m_AllEffect)
        {
            m_Effects.Add(zeEffect.m_EffectName, zeEffect);
        }
	}

    public abstract void DoAction();

    public abstract IEnumerator UpdateActRoutine();

    //public abstract bool CheckIfInCondition();

    public virtual void CheckCurrentState()
    {
    }
}
