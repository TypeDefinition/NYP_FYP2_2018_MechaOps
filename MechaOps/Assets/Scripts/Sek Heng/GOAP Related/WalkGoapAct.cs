using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkGoapAct : IGoapAction {
    [Header("References for WalkGoapAct")]
    [SerializeField, Tooltip("The Walk Unit Action that is needed to link")]
    protected UnitWalkAction m_WalkAct;

    [Header("Debugging purpose for WalkGoapAct")]
    [SerializeField, Tooltip("The Tile to move to")]
    protected TileId m_TileDest;

    protected override void Start()
    {
        base.Start();
        if (!m_WalkAct)
            m_WalkAct = GetComponent<UnitWalkAction>();
    }

    public override void DoAction()
    {
        print("Start walking");
        m_UpdateRoutine = StartCoroutine(UpdateActRoutine());
    }

    public override IEnumerator UpdateActRoutine()
    {
        m_Planner.m_Stats.CurrentActionPoints--;
        yield return new WaitForSeconds(1.5f);
        yield break;
    }
}
