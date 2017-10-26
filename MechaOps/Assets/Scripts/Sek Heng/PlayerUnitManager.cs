﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnitManager : MonoBehaviour {
    [Header("Linking and variables required")]
    public GetPlayerInputUnit m_PlayerInputOnUnit;
    [Tooltip("The prefab holder for the unit's actions!")]
    public Image m_UnitActionUIIconGO;
    [Tooltip("The parent to contain all of these icons")]
    public GameObject m_HolderOfIcons;
    [Tooltip("The ScrollRect of the unit icons")]
    public GameObject m_ScrollRectUnitIcons;

    [Header("Debugging References")]
    [SerializeField, Tooltip("The array of how many units have yet to make their turn. Meant for debugging purpose")]
    protected List<GameObject> m_UnitsYetToMakeMoves;
    [Tooltip("The number of image icons beneath it")]
    public List<Image> m_AllOfUnitUIIcon;

    /// <summary>
    /// The update of this manager. So that it can be controlled anytime
    /// </summary>
    Coroutine m_UpdateOfManager;

    private void OnEnable()
    {
        ObserverSystemScript.Instance.SubscribeEvent("PlayerAnnihilated", StopUpdate);
        ObserverSystemScript.Instance.SubscribeEvent("EnemyAnnihilated", StopUpdate);
        ObserverSystemScript.Instance.SubscribeEvent("ClickedUnit", PlayerSelectUnit);
        ObserverSystemScript.Instance.SubscribeEvent("ToggleSelectingUnit", SetThePlayerInputFalse);
    }

    private void OnDisable()
    {
        ObserverSystemScript.Instance.UnsubscribeEvent("PlayerAnnihilated", StopUpdate);
        ObserverSystemScript.Instance.UnsubscribeEvent("EnemyAnnihilated", StopUpdate);
        ObserverSystemScript.Instance.UnsubscribeEvent("ClickedUnit", PlayerSelectUnit);
        ObserverSystemScript.Instance.UnsubscribeEvent("ToggleSelectingUnit", SetThePlayerInputFalse);
    }

    public IEnumerator BeginUpdateOfPlayerUnits()
    {
        // Get a shallow copy of the list of all available units!
        m_UnitsYetToMakeMoves = new List<GameObject>(KeepTrackOfUnits.Instance.m_AllPlayerUnitGO);
        ObserverSystemScript.Instance.SubscribeEvent("UnitMakeMove", UnitHasMakeMove);
        WaitForSecondsRealtime zeAmountOfWaitTime = new WaitForSecondsRealtime(0.1f);
        m_PlayerInputOnUnit.enabled = true;
        while (m_UnitsYetToMakeMoves.Count > 0)
        {
            yield return zeAmountOfWaitTime;
        }
        ObserverSystemScript.Instance.UnsubscribeEvent("UnitMakeMove", UnitHasMakeMove);
        m_UpdateOfManager = null;
        ObserverSystemScript.Instance.TriggerEvent("TurnEnded");
        m_PlayerInputOnUnit.enabled = false;
        yield break;
    }
	
    /// <summary>
    /// To stop the coroutine update of this game object
    /// </summary>
    void StopUpdate()
    {
        ObserverSystemScript.Instance.UnsubscribeEvent("UnitMakeMove", UnitHasMakeMove);
        if (m_UpdateOfManager != null)
        {
            StopCoroutine(m_UpdateOfManager);
            m_UpdateOfManager = null;
        }
    }

    /// <summary>
    /// To recognize that the unit has already made a move and remove it from the list!
    /// </summary>
    void UnitHasMakeMove()
    {
        m_UnitsYetToMakeMoves.Remove(ObserverSystemScript.Instance.GetStoredEventVariable<GameObject>("UnitMakeMove"));
        ObserverSystemScript.Instance.RemoveTheEventVariableNextFrame("UnitMakeMove");
    }

    void PlayerSelectUnit()
    {
        GameObject zeClickedGO = ObserverSystemScript.Instance.GetStoredEventVariable<GameObject>("ClickedUnit");
        // If only the clicked unit belongs to the player!
        if (zeClickedGO.tag == "Player")
        {
            m_ScrollRectUnitIcons.SetActive(true);
            UnitAction[] allPossibleUnitActions = zeClickedGO.GetComponentsInChildren<UnitAction>();
            // Instantiate more unit actions if there is not enough actions!
            while (m_AllOfUnitUIIcon.Count < allPossibleUnitActions.Length)
            {
                Image instantiatedUnitIcon = Instantiate(m_UnitActionUIIconGO, m_HolderOfIcons.transform);
                instantiatedUnitIcon.gameObject.SetActive(true);
                m_AllOfUnitUIIcon.Add(instantiatedUnitIcon);
            }
            // Once clicked on the unit, transform the unit's sprite into actual icon!
            for (int num = 0; num < m_AllOfUnitUIIcon.Count; ++num)
            {
                if (num < allPossibleUnitActions.Length)
                {
                    // Then assign the sprite there!
                    m_AllOfUnitUIIcon[num].gameObject.SetActive(true);
                    m_AllOfUnitUIIcon[num].sprite = allPossibleUnitActions[num].m_ActionIconUI;
                    m_AllOfUnitUIIcon[num].GetComponent<UnitActionUILogic>().m_unitActionRef = allPossibleUnitActions[num];
                }
                else
                {
                    m_AllOfUnitUIIcon[num].gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Helps to set the component of player input to be false
    /// </summary>
    void SetThePlayerInputFalse()
    {
        m_PlayerInputOnUnit.enabled = false;
        m_ScrollRectUnitIcons.SetActive(false);
    }
}