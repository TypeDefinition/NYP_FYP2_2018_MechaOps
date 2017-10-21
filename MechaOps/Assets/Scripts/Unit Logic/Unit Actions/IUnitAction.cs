using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// To ensure that no one will inheirit from it!
/// Provide a base class for 
/// </summary>
[ExecuteInEditMode]
public abstract class UnitAction : MonoBehaviour
{
    [Header("[ Values and References for abstract Unit Action ]")]
    [Tooltip("The sprite UI for unit's action!")]
    public Sprite actionIconUI;
    [Tooltip("The action cost. For now it will always be 1 but this will be for expandability sake")]
    public int actionCost = 1;
    [Tooltip("The component name which will be use to indicate what UI tag to be activated.")]
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

    public virtual bool UseAction()
    {
        return false;
    }
    public virtual bool UseAction(GameObject other)
    {
        return false;
    }
}