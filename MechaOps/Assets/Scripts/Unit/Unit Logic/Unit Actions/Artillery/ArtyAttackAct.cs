using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For the artillery unit attack logic
/// </summary>
public class ArtyAttackAct : UnitAttackAction {
    [Header("Variables for ArtyAttackAct")]
    [SerializeField, Tooltip("The Exploding radius of attack")]
    protected int m_ExplodeRadius = 1;
}
