using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Set the current selected image and name to the player selected unit!
/// </summary>
public class SpawnUnitUI_Logic : MonoBehaviour
{
    /*
    Coroutine m_InsufficientNotificationCoroutine;

    public UnitSelectionSlot[] UnitSelectionSlots
    {
        set
        {
            m_UnitSelectionSlots = value;
            // Make sure each of the button will work
            foreach (UnitSelectionSlot slot in m_UnitSelectionSlots)
            {
                slot.ClearButton.onClick.AddListener(() => ClearUnit(slot));
            }
        }
        get { return m_UnitSelectionSlots; }
    }

    /// <summary>
    /// When pressed the Unit Select UI, button pressed will come to here!
    /// </summary>
    /// <param name="_UnitData"></param>
    private void PressedUnitSelectUI(int _deploymentCost)
    {
        if (m_SpawnUnitSystem.PlayerCredits + m_CurrentlySelectedSlot.GetUnitLibraryData().GetUnitStats().DeploymentCost < _UnitData.m_UnitPrefabDataReference.Cost)
        {
            SetInsufficientUI_Active();
        }
        else
        {
            m_SpawnUnitSystem.PlayerCredits += m_CurrentlySelectedSlot.GetUnitLibraryData().GetUnitStats().DeploymentCost;
            // we need to take account of the previous unit data!
            //m_CurrentSelectedSlot.UnitData = _UnitData;
            m_SpawnUnitSystem.PlayerCredits -= _UnitData.m_UnitPrefabDataReference.Cost;
            m_CreditsLeftText.text = "Credits Left: " + m_SpawnUnitSystem.PlayerCredits.ToString();
        }
    }

    /// <summary>
    /// When player press clear.
    /// </summary>
    public void ClearUnit(UnitSelectionSlot _slotUI)
    {
        //m_SpawnSystem.PlayerCredits += _slotUI.UnitData.m_UnitPrefabDataReference.Cost;
        //m_TMProCreditText.text = m_SpawnSystem.PlayerCredits.ToString();
        //_slotUI.UnitData = new UnitDataAndCost.UnitUI_Data();
    }

    public void EditCurrentSlot(UnitSelectionSlot _unitSlotUI)
    {
        m_CurrentlySelectedSlot = _unitSlotUI;
    }
    */
}