using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// To replace the traditional state machine!
/// If u wish for slightly different behavior, this class needs to be overriden
/// </summary>
public class GoapPlanner : MonoBehaviour
{
    protected class GoapNode
    {
        public GoapNode m_parent;
        public IGoapAction m_action;

        public int m_fCost = 0;

        public GoapNode(GoapNode _parent, IGoapAction _act)
        {
            m_parent = _parent;
            m_action = _act;
            m_fCost = _act.GetCost();
            if (m_parent != null)
                m_fCost += _parent.m_fCost;
        }

        /// <summary>
        /// Comparing and checking whether the nodes will be the same!
        /// </summary>
        /// <param name="_other">The other node to compare to</param>
        /// <returns>True if they are the same!</returns>
        public bool Equals(GoapNode _other)
        {
            // We can compare the cost of the nodes to see if they are the same but it will not do them justice! but certainly the fastest way!
            if (m_fCost != _other.m_fCost)
                return false;
            // So we will have to compare parent with parent and check! Need the number of parents are there!
            int zeTotalChildCount = 0;
            // since thr can be duplicates
            List<string> m_ActName = new List<string>();
            return true;
        }
    }

    [Header("Variables and references required for GOAP Planner")]
    [Tooltip("All of the GOAP Goals. Linking is not required here")]
    public IGoapGoal[] m_AllGoapGoals;
    [Tooltip("All of the GOAP actions. Linking is not required here")]
    public IGoapAction[] m_AllGoapActions;
    [Tooltip("The unit stats. Will try to get component of this if there is no linking")]
    public UnitStats m_Stats;
    [Tooltip("The StateData of this current unit")]
    public StateData m_StateData;

    [Header("Debugging Purpose for GoapPlanning")]
    [SerializeField, Tooltip("The indicator to check if it has finished making a move!")]
    protected bool m_FinishMoving = false;
    [SerializeField, Tooltip("The flag to know if it is under attacked")]
    protected bool m_UnderAttack = false;

    // Basically the name of the goap action as key, the reference to GoapAction as value
    protected Dictionary<string, IGoapAction> m_DictGoapAct = new Dictionary<string, IGoapAction>();
    protected Dictionary<string, IGoapGoal> m_DictGoapGoal = new Dictionary<string, IGoapGoal>();

    /// <summary>
    /// Whenever the planner is starting, this will be called!
    /// </summary>
    public Void_Void m_CallbackStartPlan;

    // Use this for initialization
    protected virtual void Start()
    {
        m_AllGoapActions = GetComponents<IGoapAction>();
        if (!m_Stats)
            m_Stats = GetComponent<UnitStats>();
        if (!m_StateData)
            m_StateData = GetComponent<StateData>();
        // We will need to put the actions into dictionary
        foreach (IGoapAction zeAct in m_AllGoapActions)
        {
            m_DictGoapAct.Add(zeAct.m_ActName, zeAct);
        }
        foreach (IGoapGoal zeGoal in m_AllGoapGoals)
        {
            m_DictGoapGoal.Add(zeGoal.m_GoapName, zeGoal);
        }

        m_CallbackStartPlan += m_StateData.StartInitState;
    }

    protected void FinishMakingMove()
    {
        m_FinishMoving = true;
    }

    public virtual IEnumerator StartPlanning()
    {
        ObserverSystemScript.Instance.SubscribeEvent("UnitMakeMove", FinishMakingMove);
        m_FinishMoving = false;
        // TODO: Probably need coroutine but not now
        GoapNode zeCheapestActNode = null;
        IGoapGoal zeCurrentGoal;
        List<IGoapAction> zeListOfActToDo = null;
        WaitForSeconds zeAmountOfTimeWait = new WaitForSeconds(0.5f);
        // We check it's current state
        while (m_Stats.CurrentActionPoints > 0 && !m_FinishMoving)
        {
            yield return zeAmountOfTimeWait;
            if (zeCheapestActNode == null)
            {
                if (m_CallbackStartPlan != null)
                    m_CallbackStartPlan.Invoke();
                // We will be following how the Design looks like now
                // TODO
                switch (m_UnderAttack)
                {
                    case true:
                        // Follow this special goal which is to find out whether it is able to defeat the enemy!
                        zeCurrentGoal = m_DictGoapGoal["AttackGoal"];
                        break;
                    default:
                        // We will proceed as normal
                        if (m_StateData.CurrentStates.Contains("TargetInView"))
                        {
                            zeCurrentGoal = m_DictGoapGoal["AttackGoal"];
                        }
                        else
                        {
                            // this unit did not see any player units, so move to Before that, check whether is it near the marker
                            if (m_Stats.CurrentTileID.Equals(EnemyUnitManager.Instance.TilePlayerUnits))
                            {
                                EnemyUnitManager.Instance.UpdateMarker();
                            }
                            // Then move towards there!
                            zeCurrentGoal = m_DictGoapGoal["WalkGoal"];
                        }
                        // So we will have this list of actions!
                        yield return null;
                        break;
                }
                zeCheapestActNode = GetTheCheapestAction(zeCurrentGoal);
                zeListOfActToDo = GetActsFromNode(zeCheapestActNode);
            }
            else
            {
                foreach (IGoapAction zeAct in zeListOfActToDo)
                {
                    zeAct.DoAction();
                    yield return zeAct.m_UpdateRoutine;
                    if (m_FinishMoving)
                        break;
                }
                zeCheapestActNode = null;
            }
            yield return null;
        }
        ObserverSystemScript.Instance.UnsubscribeEvent("UnitMakeMove", FinishMakingMove);
        yield break;
    }

    public IGoapAction GetGoapAct(string _ActName)
    {
        IGoapAction zeAct = null;
        m_DictGoapAct.TryGetValue(_ActName, out zeAct);
        return zeAct;
    }

    /// <summary>
    /// To get the cheapest node out of all this list!
    /// </summary>
    /// <param name="_setOfNodes"></param>
    /// <returns></returns>
    protected GoapNode GetCheapestNode(List<GoapNode> _setOfNodes)
    {
        if (_setOfNodes.Count == 0)
            return null;
        GoapNode zeCheapestNode = _setOfNodes[0];
        for (int num = 1; num < _setOfNodes.Count; ++num)
        {
            if (zeCheapestNode.m_fCost > _setOfNodes[num].m_fCost)
            {
                zeCheapestNode = _setOfNodes[num];
            }
        }
        return zeCheapestNode;
    }

    protected GoapNode GetTheCheapestAction(IGoapGoal _goal)
    {
        GoapNode zeCheapestActNode = null;
        List<GoapNode> openset = new List<GoapNode>();
        //List<GoapNode> closedset = new List<GoapNode>();
        foreach (IGoapAction zeAct in m_AllGoapActions)
        {
            openset.Add(new GoapNode(null, zeAct));
        }
        while (openset.Count > 0)
        {
            // I think the get cheapest node is unnecessary
            GoapNode zeNodeToActOn = GetCheapestNode(openset);
            if (zeNodeToActOn.m_action.m_Preconditions.Length > 0)
            {
                bool ActionNotPossible = false;
                foreach (PreConditions zeActNeeded in zeNodeToActOn.m_action.m_Preconditions)
                {
                    // If does not contain the state.
                    if (!m_StateData.CurrentStates.Contains(zeActNeeded.m_NeededState))
                    {
                        // if thr is previous action and that action can fulfill the condition
                        if (zeNodeToActOn.m_parent != null && zeNodeToActOn.m_parent.m_action.m_resultsOfThisAct.Contains(zeActNeeded.m_NeededState))
                            continue;
                        ActionNotPossible = true;
                    }
                }
                // This means the action is not workable! we will remove this node and continue to the next loop!
                if (ActionNotPossible)
                {
                    openset.Remove(zeNodeToActOn);
                    continue;
                }
            }
            // if the result of the action does not contain what the goal wants and make sure there is no parent upon it
            if (!zeNodeToActOn.m_action.m_resultsOfThisAct.Contains(_goal.m_GoapName))
            {
                if (zeNodeToActOn.m_parent == null)
                {
                    // we will add to the OPENSET since we are bruteforcing our way through
                    List<IGoapAction> zeActsWithoutNodeActs = SubsetTheAction(m_AllGoapActions, zeNodeToActOn);
                    // from here, we will add it to the openset!
                    foreach (IGoapAction zeGoapAct in zeActsWithoutNodeActs)
                    {
                        openset.Add(new GoapNode(zeNodeToActOn, zeGoapAct));
                    }
                }
            }
            else
            {
                // this node is the cheaper or there is no other node is act on!
                if ((zeCheapestActNode != null && zeCheapestActNode.m_fCost > zeNodeToActOn.m_fCost) || zeCheapestActNode == null)
                {
                    zeCheapestActNode = zeNodeToActOn;
                }
            }
            openset.Remove(zeNodeToActOn);
        }
        Assert.IsNotNull(zeCheapestActNode, "Can't find any action! Fix this bug!");
        return zeCheapestActNode;
    }

    protected List<IGoapAction> GetActsFromNode(GoapNode _node)
    {
        List<IGoapAction> zeActToDo = new List<IGoapAction>();
        while (_node != null)
        {
            zeActToDo.Add(_node.m_action);
            _node = _node.m_parent;
        }
        zeActToDo.Reverse();
        return zeActToDo;
    }

    List<IGoapAction> SubsetTheAction(IGoapAction[] _listOfActs, GoapNode _notRequiredAct)
    {
        List<IGoapAction> zeNewList = new List<IGoapAction>(_listOfActs);
        List<IGoapAction> zeNotRequiredActs = GetListOfActs(_notRequiredAct);
        foreach (IGoapAction zeNoNeedAct in zeNotRequiredActs)
        {
            zeNewList.Remove(zeNoNeedAct);
        }
        return zeNewList;
    }

    List<IGoapAction> GetListOfActs(GoapNode _goapNode)
    {
        GoapNode zeRefNode = _goapNode;   
        List<IGoapAction> zeNewList = new List<IGoapAction>();
        while (zeRefNode != null)
        {
            zeNewList.Add(zeRefNode.m_action);
            zeRefNode = zeRefNode.m_parent;
        }
        return zeNewList;
    }
}