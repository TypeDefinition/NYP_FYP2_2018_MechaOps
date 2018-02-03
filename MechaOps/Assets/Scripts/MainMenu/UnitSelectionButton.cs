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
    [SerializeField, Tooltip("Text for unit actions.")]
    protected TextMeshProUGUI m_UnitActionsText;
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
        UnitStats unitPrefab = unitLibraryData.GetUnitPrefab();

        m_UnitIcon.sprite = unitLibraryData.GetUnitIconSprite();
        m_UnitNameText.SetText(unitLibraryData.GetUnitPrefab().UnitName);
        m_UnitDescriptionText.SetText("Info:\n" + unitLibraryData.GetUnitPrefab().UnitDescription);
        m_UnitCostText.SetText("Cost: {0}", unitLibraryData.GetUnitPrefab().DeploymentCost);
        m_UnitStatsText.text = string.Format("HP: {0}\nView Range: {1}\nEvasion: {2}\nConcealment: {3}", unitPrefab.MaxHealthPoints, unitPrefab.ViewRange, unitPrefab.EvasionPoints, unitPrefab.ConcealmentPoints);

        m_UnitActionsText.text = "Actions:\n";
        IUnitAction[] availableActions = unitPrefab.gameObject.GetComponentsInChildren<IUnitAction>();
        for (int i = 0; i < availableActions.Length; ++i)
        {
            if (availableActions[i].ControllableAction)
            {
                m_UnitActionsText.text += (availableActions[i].UnitActionName + "\n");
            }
        }
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