using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class IGoapGoal {
    [Header("Variables require for IGoapGoal")]
    [SerializeField, Tooltip("The Priority of this goal over the others in case there are multiple goals available. The planner will start comparing the priorities of the goals")]
    protected int m_Priority;
    [Tooltip("The name of this goal")]
    public string m_GoapName;
    
    public int Priority
    {
        get
        {
            return m_Priority;
        }
    }
}
