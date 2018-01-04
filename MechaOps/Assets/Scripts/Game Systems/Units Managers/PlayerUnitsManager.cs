using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

[RequireComponent(typeof(DetectPlayerClicks)), DisallowMultipleComponent]
public class PlayerUnitsManager : UnitsManager
{
    // Serialised Variable(s)
    [SerializeField, Tooltip("The prefab holder for the unit's actions!")]
    protected Image m_UnitActionButtonPrefab;
    [SerializeField, Tooltip("The prefab for selecting unit indicator")]
    protected SelectedUnitIndicator m_SelectedUnitIndicatorPrefab;

    // Non-Serialised Variable(s)
    protected ScreenSpaceCanvas m_ScreenSpaceCanvas;
    protected ScrollRect m_UnitActionSelectionUIScrollRect;
    protected UnitSelection m_UnitSelection;
    protected TextMeshProUGUI m_UnitNameText;
    protected SelectedUnitIndicator m_SelectedUnitIndicator;
    protected int m_SelectedUnitIndex = 0;
    protected List<Button> m_SelectedUnitActionButtons = new List<Button>();
    IEnumerator m_UpdateCoroutine = null;
    protected bool m_IsPlayerTurn = false;

    protected override void InitEvents()
    {
        base.InitEvents();

        // Game UI
        GameEventSystem.GetInstance().SubscribeToEvent<bool>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.ToggleSelectingUnit), ToggleUnitSelection);
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.ClickedUnit), PlayerSelectedUnit);
    }

    protected override void DeinitEvents()
    {
        base.DeinitEvents();

        // Game UI
        GameEventSystem.GetInstance().UnsubscribeFromEvent<bool>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.ToggleSelectingUnit), ToggleUnitSelection);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.ClickedUnit), PlayerSelectedUnit);
    }

    protected override void Awake()
    {
        base.Awake();

        // Screen Space Canvas
        m_ScreenSpaceCanvas = m_GameSystemsDirectory.GetScreenSpaceCanvas().GetComponent<ScreenSpaceCanvas>();
        Assert.IsNotNull(m_ScreenSpaceCanvas);
        m_UnitActionSelectionUIScrollRect = m_ScreenSpaceCanvas.GetComponent<ScreenSpaceCanvas>().GetUnitActionSelectionScrollRect();
        Assert.IsNotNull(m_UnitActionSelectionUIScrollRect);
        m_UnitSelection = m_ScreenSpaceCanvas.GetComponent<ScreenSpaceCanvas>().GetUnitSelection();
        Assert.IsNotNull(m_UnitSelection);
        m_UnitNameText = m_UnitSelection.GetSelectedUnitNameText();
        Assert.IsNotNull(m_UnitNameText);

        m_SelectedUnitIndicator = Instantiate(m_SelectedUnitIndicatorPrefab.gameObject, m_ScreenSpaceCanvas.transform).GetComponent<SelectedUnitIndicator>();
        m_SelectedUnitIndicator.gameObject.SetActive(false);

        InitEvents();
    }

    protected virtual void OnDestroy()
    {
        DeinitEvents();
    }

    // Gaemplay
    protected override void GameOver(FactionType _winner)
    {
        if (m_UpdateCoroutine != null)
        {
            StopCoroutine(m_UpdateCoroutine);
        }

        ToggleUnitSelection(false);
    }

    protected override void TurnStart(FactionType _factionType)
    {
        if (_factionType != m_ManagedFaction) { return; }
        m_IsPlayerTurn = true;
        m_UpdateCoroutine = UpdateCoroutine();
        StartCoroutine(m_UpdateCoroutine);
    }

    protected virtual IEnumerator UpdateCoroutine()
    {
        // Get all alive player units.
        GetManagedUnitsFromUnitTracker();

        // Set the default selected unit.
        m_SelectedUnitIndex = 0;
        m_UnitSelection.gameObject.SetActive(true);

        // Wait for all units to make their move.
        GetComponent<DetectPlayerClicks>().enabled = true;
        while (m_ManagedUnits.Count > 0)
        {
            yield return null;
        }

        // Player no longer needs to interact with the game so might as well turn off the polling.
        GetComponent<DetectPlayerClicks>().enabled = false;
        m_UnitSelection.gameObject.SetActive(false);

        // and then iterate though units that are still alive and reset their energy points
        GetManagedUnitsFromUnitTracker();
        foreach (UnitStats unit in m_ManagedUnits)
        {
            unit.ResetActionPoints();
        }

        m_UpdateCoroutine = null;
        m_IsPlayerTurn = false;
        GameEventSystem.GetInstance().TriggerEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.TurnEnd), m_ManagedFaction);
        yield break;
    }

    /// <summary>
    /// To recognize that the unit has already made a move and remove it from the list!
    /// </summary>
    protected override void UnitFinishedTurn(UnitStats _unit)
    {
        if (_unit.UnitFaction != m_ManagedFaction) { return; }

        // m_ManagedUnits might not contain the unit as this might be an action that is done during
        // the opponent turn, and m_ManagedUnits might not have been updated if it was the start of
        // the match.
        if (m_ManagedUnits.Contains(_unit))
        {
            m_ManagedUnits.Remove(_unit);
        }
    }

    /// <summary>
    /// Starts polling for user input from GetPlayerInput
    /// </summary>
    protected override void UnitFinishedAction(UnitStats _unit)
    {
        if (_unit.UnitFaction != m_ManagedFaction) { return; }
        // A unit can finish an action when it isn't the player's turn. Such as Dying and Overwatch.
        if (m_IsPlayerTurn)
        {
            m_UnitSelection.gameObject.SetActive(true);
        }
    }

    // Game UI
    protected void PlayerSelectedUnit(GameObject _unit)
    {
        if (!m_UnitActionSelectionUIScrollRect.gameObject.activeSelf) { return; }

        UnitStats unitStats = _unit.GetComponent<UnitStats>();
        Assert.IsNotNull(unitStats);

        // If only the clicked unit belongs to the player and it must be inside the list of player unit that has yet to make a move!
        if (unitStats.UnitFaction == m_ManagedFaction && m_ManagedUnits.Contains(unitStats))
        {
            SpawnActionSelectionUI(unitStats);
        }
    }

    protected void ToggleUnitSelection(bool _on)
    {
        m_SelectedUnitIndicator.gameObject.SetActive(_on);
        m_UnitSelection.gameObject.SetActive(_on);
        m_UnitActionSelectionUIScrollRect.gameObject.SetActive(_on);
    }

    /// <summary>
    /// Going to the next index
    /// </summary>
    public void SelectNextUnit()
    {
        if (m_ManagedUnits.Count == 0)
        {
            m_SelectedUnitIndex = 0;
            return;
        }

        m_SelectedUnitIndex = (m_SelectedUnitIndex + 1) % m_ManagedUnits.Count;
        SpawnActionSelectionUI(m_ManagedUnits[m_SelectedUnitIndex]);
    }

    /// <summary>
    /// Going to the previous index
    /// </summary>
    public void SelectPreviousUnit()
    {
        if (m_ManagedUnits.Count == 0)
        {
            m_SelectedUnitIndex = 0;
            return;
        }

        if (m_SelectedUnitIndex == 0)
        {
            m_SelectedUnitIndex = m_ManagedUnits.Count - 1;
        }
        else
        {
            --m_SelectedUnitIndex;
        }
        SpawnActionSelectionUI(m_ManagedUnits[m_SelectedUnitIndex]);
    }

    public void SpawnSelectedActionUI(IUnitAction _selectedAction)
    {
        m_SelectedUnitIndicator.gameObject.SetActive(false);
        m_UnitSelection.gameObject.SetActive(false);
        Instantiate(_selectedAction.GetUnitActionUI().gameObject, m_ScreenSpaceCanvas.transform).SetActive(true);
        GameEventSystem.GetInstance().TriggerEvent<IUnitAction>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.SelectedAction), _selectedAction);
    }

    /// <summary>
    /// Spawn Action UI Icons to allow player to select the unit action.
    /// </summary>
    /// <param name="_unit">The unit gameobject which must have the unit stats and unit action for sure!</param>
    protected void SpawnActionSelectionUI(UnitStats _unit)
    {
        m_UnitNameText.text = _unit.UnitName;
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
        Assert.IsFalse(m_UnitActionSelectionUIScrollRect.content == null);
        IUnitAction[] availableActions = _unit.GetComponent<UnitActionHandler>().AllActions;
        foreach (IUnitAction unitAction in availableActions)
        {
            // Skip the action if it is not a controllable action.
            if (unitAction.ControllableAction == false) { continue; }

            // if the action cost can still be used the unit, then it will instantiate the action
            if (unitAction.GetUnitStats().CurrentActionPoints >= unitAction.ActionCost)
            {
                // Create the new icon using m_UnitActionIcon_Prefab.
                Image unitActionIconImage = Instantiate(m_UnitActionButtonPrefab, m_UnitActionSelectionUIScrollRect.content.transform);
                unitActionIconImage.gameObject.SetActive(true);

                // Replace the icon sprite to be the icon needed for this action.
                unitActionIconImage.sprite = unitAction.ActionIconUI;

                // Add to the onclick event for Unity button!
                Button unitActionButton = unitActionIconImage.GetComponent<Button>();
                unitActionButton.onClick.AddListener(() => SpawnSelectedActionUI(unitAction));

                // Add this button to m_SelectedUnitActionButtons.
                m_SelectedUnitActionButtons.Add(unitActionButton);
            }
        }

        // Instantiate the selected unit indicator.
        m_SelectedUnitIndicator.gameObject.SetActive(true);
        m_SelectedUnitIndicator.m_Unit = _unit.gameObject;
        m_SelectedUnitIndicator.AnimateUI();
    }
}