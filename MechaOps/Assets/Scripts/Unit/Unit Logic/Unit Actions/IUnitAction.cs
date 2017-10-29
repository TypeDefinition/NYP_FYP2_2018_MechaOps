using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// To ensure that no one will inherit from it!
/// Provide a base class for 
/// </summary>
[ExecuteInEditMode]
public abstract class UnitAction : MonoBehaviour
{
    [Header("[ Values and References for abstract Unit Action ]")]
    [Tooltip("The sprite UI for unit's action!")]
    public Sprite m_ActionIconUI;
    [Tooltip("The action cost. For now it will always be 1 but this will be for expandability sake")]
    public int m_ActionCost = 1;
    [Tooltip("The component name which will be use to indicate what UI tag to be activated.")]
    public string m_UnitActionName;

    [Header("[ Debugging purpose sake ]")]
    [SerializeField, Tooltip("The unit stats")]
    public UnitStats m_UnitStatGO;
    /// <summary>
    /// Most if not all, unit actions will need animation and some sort of delay
    /// </summary>
    protected Coroutine m_UpdateOfUnitAction;

    /// <summary>
    /// Do note that if the Awake function is written anew at other children, U need to call this function or prepare to face annoying bug.
    /// </summary>
    protected virtual void Awake()
    {
        // If the unit stat is not linked, get the component of it!
        if (!m_UnitStatGO)
            m_UnitStatGO = GetComponent<UnitStats>();
    }

    public virtual bool UseAction()
    {
        return false;
    }
    public virtual bool UseAction(GameObject _other)
    {
        return false;
    }

    /// <summary>
    /// The actual update of the unit action
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator UpdateActionRoutine()
    {
        m_UpdateOfUnitAction = null;
        yield break;
    }

    /// <summary>
    /// The function to start the coroutine of updating.
    /// It is virtual just in case it needs to be overridden.
    /// </summary>
    public virtual void StartUpdating()
    {
        m_UpdateOfUnitAction = StartCoroutine(UpdateActionRoutine());
    }

    /// <summary>
    /// Since not all action needs to be stopped,
    /// this will be a virtual function ready to be inherited
    /// </summary>
    public virtual void StopActionUpdate()
    {

    }
}