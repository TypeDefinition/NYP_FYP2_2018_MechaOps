using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoapNearTarget : IGoapAction
{
    [Header("References required for GoapNearTarget")]
    [SerializeField, Tooltip("Unit Attack Action reference")]
    protected UnitAttackAction m_AttackAct;

    protected override void Start()
    {
        base.Start();
        if (!m_AttackAct)
            m_AttackAct = GetComponent<UnitAttackAction>();
    }

    public override void DoAction()
    {
        m_UpdateRoutine = StartCoroutine(UpdateActRoutine());
    }

    public override IEnumerator UpdateActRoutine()
    {
        print("Tracked the target successfully");
        yield return new WaitForSeconds(1.5f);
        yield break;
    }
}
