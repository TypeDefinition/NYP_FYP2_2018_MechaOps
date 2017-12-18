﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

[RequireComponent(typeof(DetectClickedPlayerUnit)), DisallowMultipleComponent]
public class PlayerUnitsManager : MonoBehaviour
{
    [Header("Linking and variables required")]

    [SerializeField] private GameSystemsDirectory m_GameSystemsDirectory = null;

    // Prefabs used for copy initialisation.
    [SerializeField, Tooltip("The prefab holder for the unit's actions!")]
    private Image m_UnitActionIconImage_Prefab;

    [Header("Variables Shown For Debugging Purposes.")]

    // KeepTrackOfUnits keeps tracks of units that are still alive.
    [SerializeField, Tooltip("Keep Track of Unit Script")]
    private UnitsTracker m_UnitsTracker;

    // World Space Canvas
    // [SerializeField, Tooltip("The world canvas transform.")]
    // private Canvas m_WorldSpaceCanvas;

    // Screen Space Canvas
    [SerializeField, Tooltip("The screen canvas transform.")]
    private ScreenSpaceCanvas m_ScreenSpaceCanvas;
    [SerializeField, Tooltip("The ScrollRect of the unit icons")]
    private ScrollRect m_UnitActionSelectionUIScrollRect;
    [SerializeField, Tooltip("The gameobject to toggle between selecting units")]
    private UnitSelection m_UnitSelection;
    [SerializeField, Tooltip("The text UI to display the unit's name!")]
    private TextMeshProUGUI m_UnitNameText;

    // Game Logic
    [SerializeField, Tooltip("The array of how many units have yet to make their turn. Meant for debugging purpose")]
    private List<GameObject> m_UnitsYetToMakeMoves;
    [SerializeField, Tooltip("The index of the current selected unit for select between units")]
    private int m_SelectedUnitIndex = 0;
    [SerializeField, Tooltip("The player unit that has been clicked upon")]
    private GameObject m_SelectedPlayerUnit;
    [SerializeField, Tooltip("The number of image icons beneath it")]
    private List<Button> m_SelectedUnitActionButtons;
    [SerializeField, Tooltip("The current unit action that is clicked upon")]
    public IUnitAction m_CurrentSelectedAction;

    /// <summary>
    /// The update of this manager. So that it can be controlled anytime.
    /// </summary>
    protected Coroutine m_UpdateOfManager; // Does this even do anything? -Terry

    private void Start()
    {
        // Ensure that m_GameSystemsDirectory is not null.
        Assert.IsFalse(m_GameSystemsDirectory == null);

        // Units Tracker
        m_UnitsTracker = m_GameSystemsDirectory.GetUnitsTracker();
        Assert.IsFalse(m_UnitsTracker == null);

        // World Space Canvas
        // m_WorldSpaceCanvas = m_GameSystemsDirectory.GetWorldSpaceCanvas();
        // Assert.IsFalse(m_WorldSpaceCanvas == null);

        // Screen Space Canvas
        m_ScreenSpaceCanvas = m_GameSystemsDirectory.GetScreenSpaceCanvas().GetComponent<ScreenSpaceCanvas>();
        Assert.IsFalse(m_ScreenSpaceCanvas == null);
        m_UnitActionSelectionUIScrollRect = m_ScreenSpaceCanvas.GetComponent<ScreenSpaceCanvas>().GetUnitActionSelectionScrollRect();
        Assert.IsFalse(m_UnitActionSelectionUIScrollRect == null);
        m_UnitSelection = m_ScreenSpaceCanvas.GetComponent<ScreenSpaceCanvas>().GetUnitSelection();
        Assert.IsFalse(m_UnitSelection == null);
        m_UnitNameText = m_UnitSelection.GetSelectedUnitNameText();
        Assert.IsFalse(m_UnitNameText == null);
    }

    private void InitEvents()
    {
        GameEventSystem.GetInstance().SubscribeToEvent("PlayerAnnihilated", StopUpdate);
        GameEventSystem.GetInstance().SubscribeToEvent("EnemyAnnihilated", StopUpdate);
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("ClickedUnit", PlayerSelectUnit);
        GameEventSystem.GetInstance().SubscribeToEvent("ToggleSelectingUnit", ToggleThePlayerInput);
    }

    private void DeinitEvents()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent("PlayerAnnihilated", StopUpdate);
        GameEventSystem.GetInstance().UnsubscribeFromEvent("EnemyAnnihilated", StopUpdate);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("ClickedUnit", PlayerSelectUnit);
        GameEventSystem.GetInstance().UnsubscribeFromEvent("ToggleSelectingUnit", ToggleThePlayerInput);
        GameEventSystem.GetInstance().UnsubscribeFromEvent("UnitFinishAction", PollingForPlayerInput);
    }

    private void OnEnable()
    {
        InitEvents();
    }

    private void OnDisable()
    {
        DeinitEvents();
    }

    public IEnumerator BeginUpdateOfPlayerUnits()
    {
        // Get a shallow copy of the list of all available units!
        m_UnitsYetToMakeMoves = new List<GameObject>(m_UnitsTracker.m_AlivePlayerUnits);
        m_SelectedUnitIndex = 0;
        m_UnitSelection.gameObject.SetActive(true);

        // Subscribe to Events.
        GameEventSystem.GetInstance().SubscribeToEvent("UnitFinishAction", PollingForPlayerInput);
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("UnitMakeMove", UnitHasMakeMove);

        // Wait for all units to make their move.
        GetComponent<DetectClickedPlayerUnit>().enabled = true;
        WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(0.1f);
        while (m_UnitsYetToMakeMoves.Count > 0)
        {
            yield return waitForSecondsRealtime;
        }

        // Now that all units have completed their moves, unsubscribe from the events.
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("UnitMakeMove", UnitHasMakeMove);
        GameEventSystem.GetInstance().UnsubscribeFromEvent("UnitFinishAction", PollingForPlayerInput);

        m_UpdateOfManager = null; // Is this even used?

        // Player no longer needs to interact with the game so might as well turn off the polling.
        GetComponent<DetectClickedPlayerUnit>().enabled = false;
        m_UnitSelection.gameObject.SetActive(false);

        GameEventSystem.GetInstance().TriggerEvent("TurnEnded");
        yield break;
    }
	
    /// <summary>
    /// To stop the coroutine update of this game object
    /// </summary>
    protected void StopUpdate()
    {
        // Unsubscribe from events.
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("UnitMakeMove", UnitHasMakeMove);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("ClickedUnit", PlayerSelectUnit);
        GameEventSystem.GetInstance().UnsubscribeFromEvent("UnitFinishAction", PollingForPlayerInput);

        // Once again, is this even used?
        if (m_UpdateOfManager != null)
        {
            StopCoroutine(m_UpdateOfManager);
            m_UpdateOfManager = null;
        }
    }

    /// <summary>
    /// To recognize that the unit has already made a move and remove it from the list!
    /// </summary>
    protected void UnitHasMakeMove(GameObject _unitGoFinished)
    {
        m_UnitsYetToMakeMoves.Remove(_unitGoFinished);
    }

    protected void PlayerSelectUnit(GameObject _unit)
    {
        // If only the clicked unit belongs to the player and it must be inside the list of player unit that has yet to make a move!
        if (_unit.tag == "Player" && m_UnitsYetToMakeMoves.Contains(_unit))
        {
            SpawnActionUI(_unit);
        }
    }

    /// <summary>
    /// Helps to toggle the observer action subscription and the UI too!
    /// </summary>
    protected void ToggleThePlayerInput()
    {
        m_UnitActionSelectionUIScrollRect.gameObject.SetActive(!m_UnitActionSelectionUIScrollRect.gameObject.activeSelf);
        if (m_UnitActionSelectionUIScrollRect.gameObject.activeSelf)
        {
            m_UnitSelection.gameObject.SetActive(true);

            GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("ClickedUnit", PlayerSelectUnit);
        }
        else
        {
            GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("ClickedUnit", PlayerSelectUnit);
        }
    }

    /// <summary>
    /// Starts polling for user input from GetPlayerInput
    /// </summary>
    protected void PollingForPlayerInput()
    {
        m_UnitSelection.gameObject.SetActive(true);
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("ClickedUnit", PlayerSelectUnit);
    }

    /// <summary>
    /// Instantiating the specific action UI and keeping the reference to it!
    /// </summary>
    /// <param name="_SpecificUIGO">The Specific Action UI GameObject</param>
    /// <param name="_SelectedAction">The reference to that UI</param>
    public void ToInstantiateSpecificActionUI(GameObject _SpecificUIGO, IUnitAction _SelectedAction)
    {
        m_UnitSelection.gameObject.SetActive(false);
        m_CurrentSelectedAction = _SelectedAction;
        Instantiate(_SpecificUIGO, m_ScreenSpaceCanvas.transform).SetActive(true);
        GameEventSystem.GetInstance().TriggerEvent<IUnitAction>("SelectedAction", m_CurrentSelectedAction);
    }

    /// <summary>
    /// Going to the next index
    /// </summary>
    public void NextUnit()
    {
        if (m_UnitsYetToMakeMoves.Count == 0)
        {
            m_SelectedUnitIndex = 0;
            return;
        }

        m_SelectedUnitIndex = (m_SelectedUnitIndex + 1) % m_UnitsYetToMakeMoves.Count;
        SpawnActionUI(m_UnitsYetToMakeMoves[m_SelectedUnitIndex]);
    }

    /// <summary>
    /// Going to the previous index
    /// </summary>
    public void PreviousUnit()
    {
        if (m_UnitsYetToMakeMoves.Count == 0)
        {
            m_SelectedUnitIndex = 0;
            return;
        }

        if (m_SelectedUnitIndex == 0)
        {
            m_SelectedUnitIndex = m_UnitsYetToMakeMoves.Count - 1;
        }
        else
        {
            --m_SelectedUnitIndex;
        }
        SpawnActionUI(m_UnitsYetToMakeMoves[m_SelectedUnitIndex]);
    }

    /// <summary>
    /// It will help to spawn those action UI icons
    /// </summary>
    /// <param name="_unit">The unit gameobject which must have the unit stats and unit action for sure!</param>
    protected void SpawnActionUI(GameObject _unit)
    {
        m_SelectedPlayerUnit = _unit;
        UnitStats zeStat = _unit.GetComponent<UnitStats>();
        m_UnitNameText.text = zeStat.Name;
        m_UnitActionSelectionUIScrollRect.gameObject.SetActive(true);

        #region Clear Old Buttons
        // Remove all of the previous unit's buttons.
        foreach (Button unitActionButton in m_SelectedUnitActionButtons)
        {
            Destroy(unitActionButton.gameObject);
        }
        m_SelectedUnitActionButtons.Clear();
        #endregion

        // When we will instantiate the button according to the amount of unit actions there are!
        IUnitAction[] unitActions = _unit.GetComponentsInChildren<IUnitAction>();
        foreach (IUnitAction unitAction in unitActions)
        {
            // Create the new icon using m_UnitActionIcon_Prefab.
            Assert.IsFalse(m_UnitActionSelectionUIScrollRect.content == null);
            Image unitActionIconImage = Instantiate(m_UnitActionIconImage_Prefab, m_UnitActionSelectionUIScrollRect.content.transform);
            unitActionIconImage.gameObject.SetActive(true);

            // Replace the icon sprite to be the icon needed for this action.
            unitActionIconImage.sprite = unitAction.ActionIconUI;

            // Add to the onclick event for Unity button!
            Button unitActionButton = unitActionIconImage.GetComponent<Button>();
            unitActionButton.onClick.AddListener(() => ToInstantiateSpecificActionUI(unitAction.UnitActionUI, unitAction));

            // Add this button to m_SelectedUnitActionButtons.
            m_SelectedUnitActionButtons.Add(unitActionButton);
        }
    }
}