using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// The system that handles the spawning of units
/// </summary>
public class UnitsSelectionCanvas : MonoBehaviour
{
    // Serialised Variable(s)
    [SerializeField] MainMenuManager m_MainMenuManager = null;
    [SerializeField, Tooltip("Data container for the asset data.")]
    protected UnitLibrary m_UnitLibrary = null;
    [SerializeField, Tooltip("Amount of credit for the player to spend.")]
    protected int m_PlayerCredits = 20000;
    [SerializeField, Tooltip("The max number of units that the player can control.")]
    protected int m_CapacityOfUnits = 8;

    [SerializeField] protected TextMeshProUGUI m_CreditsLeftText;
    [SerializeField] Color m_SufficientCreditTextColor = Color.green;
    [SerializeField] Color m_InsufficientCreditTextColor = Color.red;

    [SerializeField] protected ScrollRect m_SelectedUnitsScrollView;
    [SerializeField] protected GridLayoutGroup m_SelectedUnitsScrollViewContent;
    [SerializeField] protected ScrollRect m_UnitSelectionScrollView;
    [SerializeField] protected VerticalLayoutGroup m_UnitSelectionScrollViewContent;
    [SerializeField] protected Button m_ConfirmButton;
    [SerializeField] protected UnitSelectionButton m_UnitSelectionButtonPrefab;
    [SerializeField] protected UnitSelectionSlot m_UnitSelectionSlotPrefab;

    [SerializeField] protected PlayableUnits m_PlayableUnits = null;
    [SerializeField] protected UnitsToSpawn m_UnitsToSpawn = null;

    // Non-Serialized Variable(s)
    protected UnitSelectionSlot m_CurrentlySelectedSlot = null;
    protected UnitSelectionSlot[] m_UnitSelectionSlots = null;
    protected UnitSelectionButton[] m_UnitSelectionButtons = null;

    private void UpdateCreditsLeftText()
    {
        m_CreditsLeftText.text = "Credits Left: " + m_PlayerCredits.ToString();
        m_CreditsLeftText.color = (m_PlayerCredits >= 0) ? m_SufficientCreditTextColor : m_InsufficientCreditTextColor;
    }

    public int PlayerCredits
    {
        set
        {
            m_PlayerCredits = value;
            UpdateCreditsLeftText();
        }
        get { return m_PlayerCredits; }
    }

    public int CapacityOfUnits
    {
        get { return m_CapacityOfUnits; }
    }

    private void Awake()
    {
        UpdateCreditsLeftText();

        m_SelectedUnitsScrollView.gameObject.SetActive(true);
        m_UnitSelectionScrollView.gameObject.SetActive(false);
        m_ConfirmButton.gameObject.SetActive(false);

        // Make sure that the array is the same size as m_CapacityOfUnits.
        m_UnitSelectionSlots = new UnitSelectionSlot[m_CapacityOfUnits];
        // Spawn the Selected Unit Slots.
        for (int i = 0; i < m_CapacityOfUnits; ++i)
        {
            UnitSelectionSlot slot = Instantiate(m_UnitSelectionSlotPrefab.gameObject, m_SelectedUnitsScrollViewContent.transform, false).GetComponent<UnitSelectionSlot>();
            slot.SetUnitsSelectionCanvas(this);
            m_UnitSelectionSlots[i] = slot;
        }

        // Spawn the Unit Selection Buttons.
        m_UnitSelectionButtons = new UnitSelectionButton[m_PlayableUnits.GetListSize()];

        for (int i = 0; i < m_UnitSelectionButtons.Length; ++i)
        {
            UnitType unitType = m_PlayableUnits.GetUnitType(i);
            Assert.IsTrue(unitType != UnitType.None && unitType != UnitType.Num_UnitType, MethodBase.GetCurrentMethod().Name + " - What the fuck! m_PlayableUnits has Num_UnitType and None!");

            UnitSelectionButton button = Instantiate(m_UnitSelectionButtonPrefab.gameObject, m_UnitSelectionScrollViewContent.transform, false).GetComponent<UnitSelectionButton>();
            button.SetUnitsSelectionCanvas(this);
            button.SetUnitType(unitType);

            m_UnitSelectionButtons[i] = button;
        }
    }

    /// <summary>
    /// When the player presses the confirm button.
    /// </summary>
    public void OnConfirmButtonClick()
    {
        SetUnitsToSpawn();
        m_MainMenuManager.SetMenuState(MainMenuManager.MenuState.StartGame);
    }

    /// <summary>
    /// Sets the units to be spawned.
    /// </summary>
    void SetUnitsToSpawn()
    {
        List<UnitType> selectedUnits = new List<UnitType>();
        for (int i = 0; i < m_UnitSelectionSlots.Length; ++i)
        {
            if (m_UnitSelectionSlots[i].GetUnitType() != UnitType.None)
            {
                selectedUnits.Add(m_UnitSelectionSlots[i].GetUnitType());
            }
        }

        m_UnitsToSpawn.SetUnitList(selectedUnits.ToArray());
    }

    bool HasSelectedUnit()
    {
        for (int i = 0; i < m_UnitSelectionSlots.Length; ++i)
        {
            if (m_UnitSelectionSlots[i].GetUnitType() != UnitType.None)
            {
                return true;
            }
        }

        return false;
    }

    public void SelectUnit(UnitType _unitType)
    {
        UnitLibrary.UnitLibraryData unitLibraryData = m_UnitLibrary.GetUnitLibraryData(_unitType);
        PlayerCredits -= unitLibraryData.GetUnitPrefab().DeploymentCost;
        m_CurrentlySelectedSlot.SetUnitType(_unitType);

        m_ConfirmButton.gameObject.SetActive(PlayerCredits >= 0 && HasSelectedUnit());
    }

    public void RemoveSelectedUnit(UnitType _unitType)
    {
        UnitLibrary.UnitLibraryData unitLibraryData = m_UnitLibrary.GetUnitLibraryData(_unitType);
        PlayerCredits += unitLibraryData.GetUnitPrefab().DeploymentCost;

        m_ConfirmButton.gameObject.SetActive(PlayerCredits >= 0 && HasSelectedUnit());
    }

    public void OpenUnitSelectionMenu(UnitSelectionSlot _currentlySelectedSlot)
    {
        m_CurrentlySelectedSlot = _currentlySelectedSlot;
        m_SelectedUnitsScrollView.gameObject.SetActive(false);
        m_UnitSelectionScrollView.gameObject.SetActive(true);
    }

    public void CloseUnitSelectionMenu()
    {
        m_SelectedUnitsScrollView.gameObject.SetActive(true);
        m_UnitSelectionScrollView.gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        m_CapacityOfUnits = Mathf.Max(1, m_CapacityOfUnits);
    }
#endif
}