using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// To ensure that no one will inheirit from it!
/// </summary>
[ExecuteInEditMode]
public abstract class UnitAction : MonoBehaviour
{
    [Header("[ Values and References for abstract Unit Action ]")]
    [Tooltip("The image UI for unit's action!")]
    public Image actionIconUI;
    [Tooltip("The action cost. For now it will always be 1 but this will be for expandability sake")]
    public int actionCost = 1;
    [Tooltip("The component name for debugging sake")]
    public string unitActionName;

    [Header("[ Debugging purpose sake ]")]
    [SerializeField, Tooltip("The unit stats")]
    protected UnitStatsGameObj unitStatGO;

    /// <summary>
    /// Do note that if the Awake function is written anew at other children, U need to call this function or prepare to face annoying bug.
    /// </summary>
    protected virtual void Awake()
    {
        // If the unit stat is not linked, get the component of it!
        if (!unitStatGO)
            unitStatGO = GetComponent<UnitStatsGameObj>();
    }

    public abstract void UseAction();
}