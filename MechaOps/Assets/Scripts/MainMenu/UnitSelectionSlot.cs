using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitSelectionSlot : MonoBehaviour
{
    // Serialised Fields
    [SerializeField] protected GameObject m_SelectedUnitDisplay;
    [SerializeField] protected Button m_AddUnitButton;
    [SerializeField] protected Button m_ClearButton;
    [SerializeField] protected Button m_EditButton;
    [SerializeField] protected Image m_UnitIcon;
    [SerializeField] protected TextMeshProUGUI m_UnitCostText;
    [SerializeField] protected UnitLibrary m_UnitLibrary = null;

    // Non-Serialised Fields
    protected UnitsSelectionCanvas m_UnitsSelectionCanvas = null;
    protected UnitType m_UnitType = UnitType.None;

    public UnitType GetUnitType() { return m_UnitType; }

    public void SetUnitType(UnitType _unitType)
    {
        m_UnitType = _unitType;
        if (m_UnitType == UnitType.None)
        {
            m_SelectedUnitDisplay.SetActive(false);
            m_AddUnitButton.gameObject.SetActive(true);
        }
        else
        {
            UnitLibrary.UnitLibraryData unitLibraryData = m_UnitLibrary.GetUnitLibraryData(m_UnitType);
            m_SelectedUnitDisplay.SetActive(true);
            m_AddUnitButton.gameObject.SetActive(false);
            m_UnitIcon.sprite = unitLibraryData.GetUnitIconSprite();
            m_UnitCostText.text = "Cost: " + unitLibraryData.GetUnitStats().DeploymentCost.ToString();
        }
    }

    public void SetUnitsSelectionCanvas(UnitsSelectionCanvas _unitsSelectionCanvas)
    {
        m_UnitsSelectionCanvas = _unitsSelectionCanvas;
    }

    public UnitsSelectionCanvas GetUnitsSelectionCanvas() { return m_UnitsSelectionCanvas; }

    public void OnEditButtonClick()
    {
        m_UnitsSelectionCanvas.OpenUnitSelectionMenu(this);
    }

    public void OnClearButtonClick()
    {
        m_AddUnitButton.gameObject.SetActive(true);
        m_SelectedUnitDisplay.SetActive(false);

        m_UnitsSelectionCanvas.RemoveSelectedUnit(m_UnitType);
        SetUnitType(UnitType.None);
    }

    private void Awake()
    {
        SetUnitType(UnitType.None);
    }
}