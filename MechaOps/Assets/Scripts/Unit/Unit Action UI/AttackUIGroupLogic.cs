using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The UI regarding attacking which will be depended upon especially if we are going to follow XCOM!
/// </summary>
public class AttackUIGroupLogic : MonoBehaviour {
    [Header("References needed for linking")]
    [Tooltip("The reference for target UI which the code should already be doing it for you. Will be expanded upon in the future!")]
    public GameObject m_TargetUIref;

    [Header("The references for debugging")]
    [Tooltip("The unit attack action reference. Player's Unit attack action is to be expected")]
    public UnitAction m_UnitAttackActRef;
    [Tooltip("The unit it is targetting. Usually should be the gameobject with the enemy tag!")]
    public GameObject m_OtherTarget;
    [Tooltip("Index of the target in the array. Usually there should be an array of enemy unit that the unit can see and iterate through that.")]
    public int m_IndexOfTarget;

    private void OnEnable()
    {
        // TODO: Use another system aside from ObserverSystem for better optimization. maybe.
        m_UnitAttackActRef = ObserverSystemScript.Instance.GetStoredEventVariable<UnitAction>(tag);
        ObserverSystemScript.Instance.RemoveTheEventVariableNextFrame(tag);
        m_IndexOfTarget = 0;
        // Need to set the references to be active
        m_TargetUIref.SetActive(true);
        // For now, it will just pick the 1st enemy in the array
        KeepTrackOfGameObj(KeepTrackOfUnits.Instance.m_AllEnemyUnitGO[0]);
        ObserverSystemScript.Instance.TriggerEvent("ToggleSelectingUnit");
    }

    private void OnDisable()
    {
        // Making sure the PlayerInput will be able to select unit again!
        //ObserverSystemScript.Instance.TriggerEvent("ToggleSelectingUnit");
        // Set to inactive when the Attack UI is closed
        m_TargetUIref.SetActive(false);
    }

    /// <summary>
    /// Use the base class action logic and allow it to be used by the button in the scene
    /// </summary>
    public void DoTheAttackAction()
    {
        m_UnitAttackActRef.UseAction(m_OtherTarget);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Cycle to the right in the array
    /// </summary>
    public void GoRightOfTarget()
    {
        m_IndexOfTarget = Mathf.Min(++m_IndexOfTarget, KeepTrackOfUnits.Instance.m_AllEnemyUnitGO.Count - 1);
        KeepTrackOfGameObj(KeepTrackOfUnits.Instance.m_AllEnemyUnitGO[m_IndexOfTarget]);
    }

    /// <summary>
    /// Cycle to the left in the array
    /// </summary>
    public void GoLeftOfTarget()
    {
        m_IndexOfTarget = Mathf.Max(--m_IndexOfTarget, 0);
        KeepTrackOfGameObj(KeepTrackOfUnits.Instance.m_AllEnemyUnitGO[m_IndexOfTarget]);
    }

    /// <summary>
    /// To allow the UI to be there according to where the target needs to be!
    /// </summary>
    /// <param name="trackedTarget">The GameObject that needs to be tracked</param>
    protected void KeepTrackOfGameObj(GameObject trackedTarget)
    {
        m_TargetUIref.transform.position = new Vector3(trackedTarget.transform.position.x, trackedTarget.transform.position.y + trackedTarget.transform.localScale.y * 0.5f, trackedTarget.transform.position.z);
        m_OtherTarget = trackedTarget;
    }
}
