using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

/// <summary>
/// The UI regarding attacking which will be depended upon especially if we are going to follow XCOM!
/// </summary>
public class AttackUI_Logic : TweenUI_Scale
{
    [Header("Variables needed")]
    [Tooltip("The prefab for target UI which the code should already be doing it for you. Will be expanded upon in the future!")]
    [SerializeField] protected UnitInfoDisplay m_UnitInfoDisplay_Prefab;
    [Tooltip("Text for tracked target")]
    [SerializeField] protected TextMeshProUGUI m_TargetNameText;

    // Action Name & Description
    [SerializeField] protected TextMeshProUGUI m_ActionNameText;
    [SerializeField] protected TextMeshProUGUI m_ActionDescriptionText;

    [Header("Shown in Inspector for debugging purposes.")]
    [Tooltip("The unit attack action reference. Player's Unit attack action is to be expected")]
    public UnitAttackAction m_UnitAttackAction;
    [Tooltip("The unit it is targeting. Usually should be the gameobject with the enemy tag!")]
    public GameObject m_OtherTarget;
    [Tooltip("Index of the target in the array. Usually there should be an array of enemy unit that the unit can see and iterate through that.")]
    public int m_IndexOfTarget;
    [Tooltip("The target UI indicator in the scene as a reference")]
    public UnitInfoDisplay m_UnitInfoDisplay;
    [SerializeField, Tooltip("List of enemy units that it can attack based on the attack action range")]
    protected List<GameObject> m_ListOfTargets = new List<GameObject>();

    private void OnEnable()
    {
        AnimateUI();
        // And we will need to link the UnitActionScheduler then we can access the action! we can safely assume there is only 1!
        m_IndexOfTarget = 0;
        GameEventSystem.GetInstance().TriggerEvent("ToggleSelectingUnit");
        GameEventSystem.GetInstance().SubscribeToEvent<IUnitAction>("SelectedAction", SetUnitAction);
    }

    private void OnDisable()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<IUnitAction>("SelectedAction", SetUnitAction);
        // Set to inactive when the Attack UI is closed
        if (m_UnitInfoDisplay)
        {
            Destroy(m_UnitInfoDisplay);
        }
    }

    /// <summary>
    /// Use the base class action logic and allow it to be used by the button in the scene
    /// </summary>
    public void PressedConfirm()
    {
        // only press the button if the target is not null!
        if (m_OtherTarget)
        {
            // set the target, schedule this action. then destroy this UI gameobject since it is not needed
            m_UnitAttackAction.SetTarget(m_OtherTarget);
            m_UnitAttackAction.TurnOn();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Cycle to the right in the array
    /// </summary>
    public void SelectPreviousTarget()
    {
        if (m_ListOfTargets.Count > 0)
        {
            m_IndexOfTarget = Mathf.Min(++m_IndexOfTarget, m_ListOfTargets.Count - 1);
            KeepTrackOfGameObject(m_ListOfTargets[m_IndexOfTarget]);
        }
    }

    /// <summary>
    /// Cycle to the left in the array
    /// </summary>
    public void SelectNextTarget()
    {
        if (m_ListOfTargets.Count > 0)
        {
            m_IndexOfTarget = Mathf.Max(--m_IndexOfTarget, 0);
            KeepTrackOfGameObject(m_ListOfTargets[m_IndexOfTarget]);
        }
    }

    /// <summary>
    /// To allow the UI to be there according to where the target needs to be!
    /// </summary>
    /// <param name="_trackedTarget">The GameObject that needs to be tracked</param>
    protected void KeepTrackOfGameObject(GameObject _trackedTarget)
    {
        m_OtherTarget = _trackedTarget;
        // set the name and the HP there
        m_TargetNameText.text = m_OtherTarget.name;
    }

    /// <summary>
    /// if the player pressed the cancel button
    /// </summary>
    public void PressedCancel()
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
    protected virtual void SetUnitAction(IUnitAction _action)
    {
        // it should be the generic attack action
        m_UnitAttackAction = (UnitAttackAction)_action;

        // Set the name and description.
        m_ActionNameText.text = _action.UnitActionName;
        m_ActionDescriptionText.text = _action.UnitActionDescription;

        int layerToCastThrough = 1 << LayerMask.NameToLayer("TileDisplay");
        // We will iterate through the global list of visible enemies
        PlayerUnitsManager playerUnitsManager = GameSystemsDirectory.GetSceneInstance().GetPlayerUnitsManager();
        foreach (GameObject seenUnit in playerUnitsManager.GlobalViewedEnemyInRange)
        {
            // we get the unit stat and tile distance!
            UnitStats unitStats = seenUnit.GetComponent<UnitStats>();
            int tileDistance = TileId.GetDistance(unitStats.CurrentTileID, m_UnitAttackAction.GetUnitStats().CurrentTileID);
            // if within range, then raycast to the target and check whether it works
            if (tileDistance <= m_UnitAttackAction.MaxAttackRange && tileDistance >= m_UnitAttackAction.MinAttackRange)
            {
                // we need the direction
                Vector3 direction = seenUnit.transform.position - m_UnitAttackAction.transform.position;
                direction.y = 1.0f;
                // if no obstacle is within the raycast which will be the tileDisplay layer
                if (!Physics.Raycast(m_UnitAttackAction.transform.position, direction, direction.magnitude, layerToCastThrough))
                {
                    m_ListOfTargets.Add(seenUnit);
                }
            }
        }
        if (m_ListOfTargets.Count > 0)
        {
            KeepTrackOfGameObject(m_ListOfTargets[0]);
        }
    }
}
