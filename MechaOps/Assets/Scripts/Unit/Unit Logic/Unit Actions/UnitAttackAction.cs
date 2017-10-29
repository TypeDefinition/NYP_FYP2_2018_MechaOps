using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackAction : UnitAction
{
    [Header("The References and variables needed for Unit Attack")]
    [SerializeField, Tooltip("Minimum attack range of the unit")]
    private int m_MinAttackRange;
    [SerializeField, Tooltip("Maximum attack range of the unit")]
    private int m_MaxAttackRange;
    [SerializeField, Tooltip("The accuracy points of the unit")]
    private int m_AccuracyPoints;
    [Tooltip("The damage point it dealt")]
    public int m_DamagePts;

    [Header("Debugging purpose for Unit Attack Action")]
    [Tooltip("The unit stat of the target")]
    public UnitStats m_OtherTargetStat;

    /// <summary>
    /// Optimization will have to come later as this will need to be expanded upon!
    /// </summary>
    /// <param name="_other">The opposing target</param>
    /// <returns>Dont know yet!</returns>
    public override bool UseAction(GameObject _other)
    {
        m_OtherTargetStat = _other.GetComponent<UnitStats>();
        if (m_OtherTargetStat)
        {
            m_UpdateOfUnitAction = StartCoroutine(UpdateActionRoutine());
            return true;
        }
        return false;
    }

    /// <summary>
    /// To do raycasting and calculation. Along with the animation required.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator UpdateActionRoutine()
    {
        // TODO: Do some complex calculation and animation for this
        m_OtherTargetStat.m_UnitStatsJSON.CurrentHealthPoints -= m_DamagePts;
        // Thinking of a way to implement it
        m_UpdateOfUnitAction = null;
        --m_UnitStatGO.m_UnitStatsJSON.CurrentActionPoints;
        switch (m_UnitStatGO.m_UnitStatsJSON.CurrentActionPoints)
        {
            case 0:
                m_UnitStatGO.ResetUnitStat();
                ObserverSystemScript.Instance.StoreVariableInEvent("UnitMakeMove", gameObject);
                ObserverSystemScript.Instance.TriggerEvent("UnitMakeMove");
                break;
            default:
                break;
        }
        ObserverSystemScript.Instance.TriggerEvent("UnitFinishAction");
        yield break;
    }

    // Use this for initialization
    void Start () {
        if (m_UnitActionName == null)
        {
            m_UnitActionName = "Attack";
        }
        else
        {
            // Just in case the action name is not included!
            switch (m_UnitActionName.Length)
            {
                case 0:
                    m_UnitActionName = "Attack";
                    break;
                default:
                    break;
            }
        }
    }
}