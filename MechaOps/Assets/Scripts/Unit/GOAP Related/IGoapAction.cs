using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PreConditions
{
    public string m_NeededState;
    public string m_ActionName;
}

public abstract class IGoapAction : MonoBehaviour {
    [Header("References required for IGoapAction ")]
    [SerializeField, Tooltip("The cost to do this action")]
    protected int m_Cost = 2;
    [Tooltip("The action name")]
    public string m_ActName;
    [Tooltip("Possible actions this will lead to!")]
    public PreConditions[] m_Preconditions;
    [SerializeField, Tooltip("GOAP Planner. Will attempt to get component if there is no linking")]
    protected GoapPlanner m_Planner;
    [Tooltip("The result that will be given when this action is done!")]
    public List<string> m_resultsOfThisAct;

    [Header("Debugging for IGoapAction")]
    [SerializeField, Tooltip("Flag to check whether the action is completed")]
    protected bool m_ActionCompleted = false;

    public Coroutine m_UpdateRoutine
    {
        protected set; get;
    }

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
	}

    public abstract void DoAction();

    public abstract IEnumerator UpdateActRoutine();

    public virtual void CheckCurrentState()
    {
    }

    protected virtual void InvokeActionCompleted()
    {
        m_ActionCompleted = true;
    }
}
