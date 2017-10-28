using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitOverwatchAction : UnitAction {
    [Header("Linking reference and variables required")]
    [Tooltip("The attack action so as to reuse the attack logic. If there is no linking, then there will be Getting of the component at Start")]
    public UnitAttackAction m_AttackAction;

    [Header("Debug Reference for OverWatch Action")]
    [SerializeField, Tooltip("The flag to allow OverWatch!")]
    protected bool m_OverwatchFlag = false;

	// Use this for initialization
	void Start () {
        if (!m_AttackAction)
            m_AttackAction = GetComponent<UnitAttackAction>();
    }

    public override bool UseAction()
    {
        m_OverwatchFlag = true;
        return true;
    }

    /// <summary>
    /// To wait for the enemy to enter it's range!
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        switch (m_OverwatchFlag)
        {
            case true:
                // TODO: need to do something special here
                m_OverwatchFlag = false;
                break;
            default:
                break;
        }
    }
}
