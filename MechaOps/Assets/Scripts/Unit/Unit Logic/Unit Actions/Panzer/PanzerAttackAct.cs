using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Is to use the special attack helper in it's attack calculation
/// </summary>
public class PanzerAttackAct : UnitAttackAction {
    protected override bool CheckIfHit()
    {
        int zeHitChance = UnitGameHelper.CalculateAttackHitChance_Tank(TileId.GetDistance(m_UnitStats.CurrentTileID, m_TargetUnitStats.CurrentTileID), MinAttackRange, MaxAttackRange, m_TargetUnitStats.EvasionPoints);
        //if (AccuracyPoints > )
        // not sure about Accuracy points
        if (zeHitChance > 50)
            return true;
        return false;
    }
}
