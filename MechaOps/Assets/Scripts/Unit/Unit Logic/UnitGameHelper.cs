using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGameHelper
{
    static int CalculateAttackHitChance_Tank(int _distanceToTarget, int _minAttackRange, int _maxAttackRange, int _targetEvasion)
    {
        int optimalDistance = (_maxAttackRange - _minAttackRange) >> 1;
        float hitChance = (float)Mathf.Abs(optimalDistance - _distanceToTarget) / (float)optimalDistance;
        hitChance *= 100.0f;
        hitChance -= (float)_targetEvasion;

        return Mathf.Clamp((int)hitChance, 1, 100);
    }

}