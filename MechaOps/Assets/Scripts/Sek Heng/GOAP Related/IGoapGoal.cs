using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IGoapGoal : MonoBehaviour {
    [Header("Variables and references required for IGoapGoal")]
    [SerializeField, Tooltip("The Priority of this goal over the others in case there are multiple goals available. The planner will start comparing the priorities of the goals")]
    protected int m_Priority;
    [Tooltip("The name of the actions required to complete this goal")]
    public string[] m_ActNameNeeded;
    [Tooltip("The name of this goal")]
    public string m_GoapName;
    
    public int Priority
    {
        get
        {
            return m_Priority;
        }
    }

    /// <summary>
    /// To achieve the actions required to complete this goal!
    /// </summary>
    /// <returns>Returns the valid number of actions required for this goal to be achieved</returns>
    //public abstract IGoapAction[] TrackValidAct();
}
