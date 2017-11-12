using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    [Header("Variables and references required for GOAP Planner")]
    [Tooltip("All of the GOAP Goals. Linking is not required here")]
    public IGoapGoal[] m_AllGoapGoals;
    [Tooltip("All of the GOAP actions. Linking is not required here")]
    public IGoapAction[] m_AllGoapActions;
    [Tooltip("The unit stats. Will try to get component of this if there is no linking")]
    public UnitStats m_Stats;

    [Header("Debugging Purpose for GoapPlanning")]
    [SerializeField, Tooltip("The flag to know if it is under attacked")]
    protected bool m_UnderAttack = false;
    [SerializeField, Tooltip("The health before it is attacked. This will be updated whenever it is it's turn. This will need to be changed")]
    public int m_BeforeAttackHP;

    // Basically the name of the goap action as key, the reference to GoapAction as value
    protected Dictionary<string, IGoapAction> m_DictGoapAct = new Dictionary<string, IGoapAction>();
    protected Dictionary<string, IGoapGoal> m_DictGoapGoal = new Dictionary<string, IGoapGoal>();

    // Use this for initialization
    protected virtual void Start()
    {
        m_AllGoapActions = GetComponents<IGoapAction>();
        m_AllGoapGoals = GetComponents<IGoapGoal>();
        m_Stats = GetComponent<UnitStats>();
        m_BeforeAttackHP = m_Stats.CurrentHealthPoints;
        // We will need to put the actions into dictionary
        foreach (IGoapAction zeAct in m_AllGoapActions)
        {
            m_DictGoapAct.Add(zeAct.m_ActName, zeAct);
        }
        foreach (IGoapGoal zeGoal in m_AllGoapGoals)
        {
            m_DictGoapGoal.Add(zeGoal.m_GoapName, zeGoal);
        }
    }

    public virtual IEnumerator StartPlanning()
    {
        if (m_BeforeAttackHP < m_Stats.CurrentHealthPoints)
        {
            m_UnderAttack = true;
            m_BeforeAttackHP = m_Stats.CurrentHealthPoints;
        }
        // TODO: Probably need coroutine but not now
        GoapNode zeCheapestActNode = null;
        IGoapGoal zeCurrentGoal;
        while (m_Stats.CurrentActionPoints > 0)
        {
            if (zeCheapestActNode == null)
            {
                // We will be following how the Design looks like now
                switch (m_UnderAttack)
                {
                    case true:
                        // Follow this special goal which is to find out whether it is able to defeat the enemy!
                        zeCurrentGoal = m_DictGoapGoal["AttackGoal"];
                        break;
                    default:
                        // We will proceed as normal
                        // this unit did not see any player units, so move tol Before that, check whether is it near the marker
                        if (m_Stats.CurrentTileID.Equals(EnemyUnitManager.Instance.TilePlayerUnits))
                        {
                            EnemyUnitManager.Instance.UpdateMarker();
                        }
                        // Then move towards there!
                        zeCurrentGoal = m_DictGoapGoal["WalkGoal"];
                        // So we will have this list of actions!
                        yield return null;
                        break;
                }
                zeCheapestActNode = GetTheCheapestAction(zeCurrentGoal);
            }
            else
            {
                while (zeCheapestActNode != null)
                {
                    zeCheapestActNode.m_action.DoAction();
                    yield return zeCheapestActNode.m_action.m_UpdateRoutine;
                    zeCheapestActNode = zeCheapestActNode.m_parent;
                }
            }
            yield return null;
        }
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
        foreach (string zeActName in _goal.m_ActNameNeeded)
        {
            IGoapAction zeTempAct;
            if (m_DictGoapAct.TryGetValue(zeActName, out zeTempAct))
            {
                openset.Add(new GoapNode(null, zeTempAct));
            }
        }
        while (openset.Count > 0)
        {
            GoapNode zeNodeToActOn = GetCheapestNode(openset);
            if (zeNodeToActOn.m_action.m_NamePrecActs.Length > 0)
            {
                bool ActionNotPossible = true;
                foreach (string zeActName in zeNodeToActOn.m_action.m_NamePrecActs)
                {
                    IGoapAction zeOtherAct;
                    if (m_DictGoapAct.TryGetValue(zeActName, out zeOtherAct))
                    {
                        zeOtherAct.CheckCurrentState();
                        openset.Add(new GoapNode(zeNodeToActOn, zeOtherAct));
                        ActionNotPossible = false;
                    }
                }
                // This means there are no possible actions for this node to work on!
                if (ActionNotPossible)
                {
                    openset.Remove(zeCheapestActNode);
                    continue;
                }
            }
            else
            {
                // it means this is the final action and check if it is the cheapest to act on!
                if (zeCheapestActNode != null && zeCheapestActNode.m_fCost > zeNodeToActOn.m_fCost)
                {
                    zeCheapestActNode = zeNodeToActOn;
                }
                else
                {
                    // This means we will have to operate on this node even though it is expensive
                    zeCheapestActNode = zeNodeToActOn;
                }
                openset.Remove(zeNodeToActOn);
            }
        }
        return zeCheapestActNode;
    }
}