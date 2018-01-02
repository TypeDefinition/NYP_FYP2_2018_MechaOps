using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// The UI icon that allows player to select it's unit
/// </summary>
[RequireComponent(typeof(Button))]
public class UnitSelectionButton : MonoBehaviour
{
    // Serialised Variable(s)
    [SerializeField, Tooltip("Text UI for unit name.")]
    protected TextMeshProUGUI m_UnitNameText;
    [SerializeField, Tooltip("Text for unit description.")]
    protected TextMeshProUGUI m_UnitDescriptionText;
    [SerializeField, Tooltip("Text for unit stats.")]
    protected TextMeshProUGUI m_UnitStatsText;
    [SerializeField, Tooltip("Text for unit costs.")]
    protected TextMeshProUGUI m_UnitCostText;
    [SerializeField, Tooltip("Image for the unit icon.")]
    protected Image m_UnitIcon;
    [SerializeField]
    UnitLibrary m_UnitLibrary = null;

    // Non-Serialised Variable(s)
    UnitType m_UnitType = UnitType.None;
    UnitsSelectionCanvas m_UnitsSelectionCanvas = null;

    /// <summary>
    /// When pass in the unit stat, it will also change the text UI for it.
    /// </summary>
    /// <param name="_unitType">Unit Type</param>
    public void SetUnitType(UnitType _unitType)
    {
        m_UnitType = _unitType;
        UnitLibrary.UnitLibraryData unitLibraryData = m_UnitLibrary.GetUnitLibraryData(m_UnitType);

        m_UnitIcon.sprite = unitLibraryData.GetUnitIconSprite();
        m_UnitNameText.SetText(unitLibraryData.GetUnitPrefab().UnitName);
        m_UnitDescriptionText.SetText(unitLibraryData.GetUnitPrefab().UnitDescription);
        //m_UnitStatsText.text = string.Format("HP: {0}\nAttack: {1}\nView Range: {2}\nEvasion: {3}\nConcealment: {4}", m_UnitLibraryData.MaxHealthPoints, m_UnitLibraryData.GetUnitStats().ViewRange, m_UnitLibraryData.GetUnitStats().EvasionPoints, m_UnitLibraryData.GetUnitStats().ConcealmentPoints);
        m_UnitCostText.SetText("Deployment Cost: {0}", unitLibraryData.GetUnitPrefab().DeploymentCost);
    }

    public UnitType GetUnitType() { return m_UnitType; }

    public void SetUnitsSelectionCanvas(UnitsSelectionCanvas _unitsSelectionCanvas)
    {
        m_UnitsSelectionCanvas = _unitsSelectionCanvas;
    }

    public void OnClick()
    {
        m_UnitsSelectionCanvas.SelectUnit(m_UnitType);
        m_UnitsSelectionCanvas.CloseUnitSelectionMenu();
    }
}