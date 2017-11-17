﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The UI regarding attacking which will be depended upon especially if we are going to follow XCOM!
/// </summary>
public class AttackUIGroupLogic : MonoBehaviour {
    [Header("References for linking")]
    [Tooltip("The reference for target UI which the code should already be doing it for you. Will be expanded upon in the future!")]
    public GameObject m_TargetUIref;

    [Header("Debugging")]
    [Tooltip("The unit attack action reference. Player's Unit attack action is to be expected")]
    public UnitAttackAction m_UnitAttackActRef;
    [Tooltip("The unit it is targeting. Usually should be the gameobject with the enemy tag!")]
    public GameObject m_OtherTarget;
    [Tooltip("Index of the target in the array. Usually there should be an array of enemy unit that the unit can see and iterate through that.")]
    public int m_IndexOfTarget;
    [Tooltip("UnitActionScheduler ref")]
    public UnitActionScheduler m_actScheduler;
    [Tooltip("The target UI indicator in the scene as a reference")]
    public GameObject m_TargetGO;
    [Tooltip("The transform reference to world canvas")]
    public Transform m_WorldCanvasTrans;
    [SerializeField, Tooltip("List of enemy units that it can attack based on the attack action range")]
    protected List<GameObject> m_ListOfTargets = new List<GameObject>();
    [SerializeField, Tooltip("All of the attackable tiles within range")]
    protected TileId[] m_AttackableTileRange;
    [SerializeField, Tooltip("The tile system")]
    protected TileSystem m_TileSys;

    private void OnEnable()
    {
        // TODO: Use another system aside from ObserverSystem for better optimization. maybe.
        m_UnitAttackActRef = ObserverSystemScript.Instance.GetStoredEventVariable<UnitAttackAction>("SelectedAction");
        ObserverSystemScript.Instance.RemoveTheEventVariableNextFrame("SelectedAction");
        m_IndexOfTarget = 0;
        m_WorldCanvasTrans = GameObject.FindGameObjectWithTag("WorldCanvas").transform;
        m_TargetGO = Instantiate(m_TargetUIref, m_WorldCanvasTrans);
        m_TargetGO.SetActive(true);
        m_TileSys = FindObjectOfType<TileSystem>();
        //TODO Realised that highlighting of the tile minimum attack range to max attack range is impossible
        m_AttackableTileRange = m_TileSys.GetReachableTiles(m_UnitAttackActRef.MaxAttackRange, m_UnitAttackActRef.m_UnitStats.CurrentTileID, null);
        m_TileSys.SetPathMarkers(m_AttackableTileRange, null);
        // We will iterate through the list of alived units!
        foreach (GameObject zeObj in KeepTrackOfUnits.Instance.m_AllEnemyUnitGO)
        {
            // we get the unit stat and tile distance!
            UnitStats zeObjStat = zeObj.GetComponent<UnitStats>();
            int zeTileDist = TileId.GetDistance(zeObjStat.CurrentTileID, m_UnitAttackActRef.m_UnitStats.CurrentTileID);
            // if within range, then add to the target list!
            if (zeTileDist <= m_UnitAttackActRef.MaxAttackRange && zeTileDist >= m_UnitAttackActRef.MinAttackRange)
            {
                m_ListOfTargets.Add(zeObj);
            }
        }
        if (m_ListOfTargets.Count > 0)
        {
            // For now, it will just pick the 1st enemy in the array
            KeepTrackOfGameObj(m_ListOfTargets[0]);
        }
        ObserverSystemScript.Instance.TriggerEvent("ToggleSelectingUnit");
        // And we will need to link the UnitActionScheduler then we can access the action! we can safely assume there is only 1!
        m_actScheduler = FindObjectOfType<UnitActionScheduler>();
    }

    private void OnDisable()
    {
        m_TileSys.ClearPathMarkers();
        // Set to inactive when the Attack UI is closed
        Destroy(m_TargetGO);
    }

    /// <summary>
    /// Use the base class action logic and allow it to be used by the button in the scene
    /// </summary>
    public void DoTheAttackAction()
    {
        //m_UnitAttackActRef.StartAction(m_OtherTarget);
        //gameObject.SetActive(false);
        // set the target, schedule this action. then destroy this UI gameobject since it is not needed
        m_UnitAttackActRef.SetTarget(m_OtherTarget);
        m_UnitAttackActRef.TurnOn();
        m_actScheduler.ScheduleAction(m_UnitAttackActRef);
        Destroy(gameObject);
    }

    /// <summary>
    /// Cycle to the right in the array
    /// </summary>
    public void GoRightOfTarget()
    {
        m_IndexOfTarget = Mathf.Min(++m_IndexOfTarget, m_ListOfTargets.Count - 1);
        KeepTrackOfGameObj(m_ListOfTargets[m_IndexOfTarget]);
    }

    /// <summary>
    /// Cycle to the left in the array
    /// </summary>
    public void GoLeftOfTarget()
    {
        m_IndexOfTarget = Mathf.Max(--m_IndexOfTarget, 0);
        KeepTrackOfGameObj(m_ListOfTargets[m_IndexOfTarget]);
    }

    /// <summary>
    /// To allow the UI to be there according to where the target needs to be!
    /// </summary>
    /// <param name="trackedTarget">The GameObject that needs to be tracked</param>
    protected void KeepTrackOfGameObj(GameObject trackedTarget)
    {
        m_TargetGO.transform.position = new Vector3(trackedTarget.transform.position.x, trackedTarget.transform.position.y + trackedTarget.transform.localScale.y * 0.5f, trackedTarget.transform.position.z);
        m_OtherTarget = trackedTarget;
    }

    /// <summary>
    /// if the player pressed the back button
    /// </summary>
    public void PressBack()
    {
        // ensure that the player will be able to click on unit again!
        ObserverSystemScript.Instance.TriggerEvent("ToggleSelectingUnit");
        // there is no point in keeping this UI anymore so destroy it!
        Destroy(gameObject);
    }
}
