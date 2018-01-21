using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

/// <summary>
/// The UI regarding attacking which will be depended upon especially if we are going to follow XCOM!
/// </summary>
public class UnitActionUI_Attack : UnitActionUI
{
    [Tooltip("Text for tracked target")]
    [SerializeField] protected TextMeshProUGUI m_TargetNameText;
    [SerializeField] Image m_CrosshairPrefab = null;

    protected Image m_Crosshair = null;
    protected UnitAttackAction m_UnitAttackAction = null;
    protected UnitStats m_OtherTarget = null;
    protected int m_IndexOfTarget = 0;
    protected List<UnitStats> m_ListOfTargets = new List<UnitStats>();

    protected override void Awake()
    {
        base.Awake();
        m_IndexOfTarget = 0;

        Canvas screenSpaceCanvas = GameSystemsDirectory.GetSceneInstance().GetClickableScreenSpaceCanvas();
        m_Crosshair = Instantiate(m_CrosshairPrefab.gameObject, screenSpaceCanvas.gameObject.transform).GetComponent<Image>();
        m_Crosshair.gameObject.SetActive(false);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Destroy(m_Crosshair);
    }

    /// <summary>
    /// Use the base class action logic and allow it to be used by the button in the scene
    /// </summary>
    public override void PressedConfirm()
    {
        // only press the button if the target is not null!
        if (m_OtherTarget && m_UnitAttackAction)
        {
            // set the target, schedule this action. then destroy this UI gameobject since it is not needed
            m_UnitAttackAction.SetTarget(m_OtherTarget.gameObject);
            m_UnitAttackAction.TurnOn();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Cycle to the right in the array
    /// </summary>
    public void SelectNextTarget()
    {
        if (m_ListOfTargets.Count > 0)
        {
            m_IndexOfTarget = (m_IndexOfTarget + 1) % m_ListOfTargets.Count;
            SetTarget(m_ListOfTargets[m_IndexOfTarget]);
        }
        else
        {
            m_IndexOfTarget = 0;
        }
    }

    /// <summary>
    /// Cycle to the left in the array
    /// </summary>
    public void SelectPreviousTarget()
    {
        if (m_ListOfTargets.Count > 0)
        {
            m_IndexOfTarget = (m_IndexOfTarget > 0) ? (m_IndexOfTarget - 1) : (m_ListOfTargets.Count - 1);
            SetTarget(m_ListOfTargets[m_IndexOfTarget]);
        }
        else
        {
            m_IndexOfTarget = 0;
        }
    }

    /// <summary>
    /// To allow the UI to be there according to where the target needs to be!
    /// </summary>
    /// <param name="_target">The GameObject that needs to be tracked</param>
    protected void SetTarget(UnitStats _target)
    {
        m_OtherTarget = _target;
        // Update the target name.
        m_TargetNameText.text = m_OtherTarget.UnitName;
        GameEventSystem.GetInstance().TriggerEvent<GameObject>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.FocusOnTarget), m_OtherTarget.gameObject);
    }

    protected virtual void Update()
    {
        if (m_OtherTarget)
        {
            Canvas screenSpaceCanvas = GameSystemsDirectory.GetSceneInstance().GetClickableScreenSpaceCanvas();
            Camera gameCamera = GameSystemsDirectory.GetSceneInstance().GetGameCamera();
            Vector3 screenPoint = gameCamera.WorldToScreenPoint(m_OtherTarget.gameObject.transform.position);
            screenPoint.z = 0.0f;
            m_Crosshair.gameObject.transform.position = screenPoint;
            m_Crosshair.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Called when the unit action is there!
    /// </summary>
    /// <param name="_action"></param>
    protected override void SetUnitAction(IUnitAction _action)
    {
        // it should be the generic attack action
        m_UnitAttackAction = (UnitAttackAction)_action;

        int layerToCastThrough = LayerMask.GetMask("TileDisplay");
        // We will iterate through the global list of visible enemies
        PlayerUnitsManager playerUnitsManager = GameSystemsDirectory.GetSceneInstance().GetPlayerUnitsManager();
        List<UnitStats> seenEnemies = playerUnitsManager.GetSeenEnemies();
        foreach (UnitStats enemy in seenEnemies)
        {
            int tileDistance = TileId.GetDistance(enemy.CurrentTileID, m_UnitAttackAction.GetUnitStats().CurrentTileID);
            // if within range, then raycast to the target and check whether it works
            if (tileDistance <= m_UnitAttackAction.MaxAttackRange && tileDistance >= m_UnitAttackAction.MinAttackRange)
            {
                // we need the direction
                Vector3 direction = enemy.transform.position - m_UnitAttackAction.transform.position;
                direction.y = 1.0f;
                // if no obstacle is within the raycast which will be the tileDisplay layer
                if (!Physics.Raycast(m_UnitAttackAction.transform.position, direction, direction.magnitude, layerToCastThrough))
                {
                    m_ListOfTargets.Add(enemy);
                }
            }
        }

        m_IndexOfTarget = 0;
        if (m_ListOfTargets.Count > 0)
        {
            SetTarget(m_ListOfTargets[m_IndexOfTarget]);
        }
        else
        {
            m_ConfirmButton.gameObject.SetActive(false);
            m_TargetNameText.text = "No Targets In Range";
        }

        UpdateActionInfo(_action);
    }

    protected override void UpdateActionInfo(IUnitAction _action)
    {
        m_ActionNameText.text = _action.UnitActionName;
        string actionCostText = string.Format("Action Cost: {0}", _action.ActionCost);
        string endsTurnText = _action.EndsTurn ? "Ends Turn: Yes" : "Ends Turn: No";

        string hitChanceText;
        // If not target, then make it not applicable
        if (!m_OtherTarget)
        {
            hitChanceText = "Hit Chance: NA";
        }
        else
        {
            // have to set target otherwise calculate hit chance will crash
            m_UnitAttackAction.SetTarget(m_OtherTarget.gameObject);
            hitChanceText = string.Format("Hit Chance: {0}%", m_UnitAttackAction.CalculateHitChance());
        }

        m_ActionDescriptionText.text = actionCostText + " " + endsTurnText + "\n" + hitChanceText + "\n" + _action.UnitActionDescription;
    }
}