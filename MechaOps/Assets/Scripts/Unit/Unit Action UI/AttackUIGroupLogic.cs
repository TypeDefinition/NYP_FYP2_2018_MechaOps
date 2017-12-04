using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// The UI regarding attacking which will be depended upon especially if we are going to follow XCOM!
/// </summary>
public class AttackUIGroupLogic : MonoBehaviour {
    [Header("Variables needed")]
    [Tooltip("The reference for target UI which the code should already be doing it for you. Will be expanded upon in the future!")]
    public UnitDisplayUI m_TargetUIref;
    [Tooltip("Text for tracked target")]
    public TextMeshProUGUI m_TargetNameTxt;
    [SerializeField, Tooltip("The Animation time for the attack")]
    protected float m_AnimTime = 0.3f;
    [SerializeField, Tooltip("The distance from the tracked target to camera")]
    protected float m_Dist = 3.0f;

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
    [SerializeField, Tooltip("The HP text")]
    protected TextMeshProUGUI m_hpTextUI;

    private void OnEnable()
    {
        Vector3 zeScale = transform.localScale;
        float zeOriginalScaleX = zeScale.x;
        zeScale.x = 0;
        transform.localScale = zeScale;
        LeanTween.scaleX(gameObject, zeOriginalScaleX, m_AnimTime);
        // And we will need to link the UnitActionScheduler then we can access the action! we can safely assume there is only 1!
        m_IndexOfTarget = 0;
        m_actScheduler = FindObjectOfType<UnitActionScheduler>();
        GameEventSystem.GetInstance().TriggerEvent("ToggleSelectingUnit");
        GameEventSystem.GetInstance().SubscribeToEvent<IUnitAction>("SelectedAction", PressedAction);
    }

    private void OnDisable()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<IUnitAction>("SelectedAction", PressedAction);
        // Set to inactive when the Attack UI is closed
        if (m_TargetGO)
            Destroy(m_TargetGO);
    }

    /// <summary>
    /// Use the base class action logic and allow it to be used by the button in the scene
    /// </summary>
    public void DoTheAttackAction()
    {
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
        UnitStats zeTargetStat = trackedTarget.GetComponent<UnitStats>();
        UnitDisplayUI zeDisplayUI = m_TargetGO.GetComponent<UnitDisplayUI>();
        zeDisplayUI.SetThePosToUnit(trackedTarget.transform);
        zeDisplayUI.HpText = zeTargetStat.CurrentHealthPoints + "/" + zeTargetStat.MaxHealthPoints;
        zeDisplayUI.AnimateUI();
        m_OtherTarget = trackedTarget;
        // set the name and the HP there
        m_TargetNameTxt.text = m_OtherTarget.name;
    }

    /// <summary>
    /// if the player pressed the back button
    /// </summary>
    public void PressBack()
    {
        // ensure that the player will be able to click on unit again!
        GameEventSystem.GetInstance().TriggerEvent("ToggleSelectingUnit");
        // there is no point in keeping this UI anymore so destroy it!
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Called when the unit action is there!
    /// </summary>
    /// <param name="_action"></param>
    protected virtual void PressedAction(IUnitAction _action)
    {
        // it should be the generic attack action
        m_UnitAttackActRef = (UnitAttackAction)_action;

        int layerToCastThrough = 1 << LayerMask.NameToLayer("TileDisplay");
        // We will iterate through the list of units that this unit can see!
        foreach (GameObject zeSeenUnit in m_UnitAttackActRef.m_UnitStats.EnemyInRange)
        {
            // we get the unit stat and tile distance!
            UnitStats zeObjStat = zeSeenUnit.GetComponent<UnitStats>();
            int zeTileDist = TileId.GetDistance(zeObjStat.CurrentTileID, m_UnitAttackActRef.m_UnitStats.CurrentTileID);
            // if within range, then raycast to the target and check whether it works
            if (zeTileDist <= m_UnitAttackActRef.MaxAttackRange && zeTileDist >= m_UnitAttackActRef.MinAttackRange)
            {
                // we need the direction
                Vector3 zeDirection = zeSeenUnit.transform.position - m_UnitAttackActRef.transform.position;
                zeDirection.y = 1;
                // if no obstacle is within the raycast which will be the tileDisplay layer
                if (!Physics.Raycast(m_UnitAttackActRef.transform.position, zeDirection, zeDirection.magnitude, layerToCastThrough))
                    m_ListOfTargets.Add(zeSeenUnit);
            }
        }
        if (m_ListOfTargets.Count > 0)
        {
            m_WorldCanvasTrans = GameObject.FindGameObjectWithTag("WorldCanvas").transform;
            m_TargetGO = Instantiate(m_TargetUIref.gameObject, m_WorldCanvasTrans);
            m_TargetGO.SetActive(true);
            // For now, it will just pick the 1st enemy in the array
            KeepTrackOfGameObj(m_ListOfTargets[0]);
        }
    }
}
