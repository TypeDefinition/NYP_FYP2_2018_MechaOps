using System.Collections;
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
    [Tooltip("The canvas transform that holds all of the action stuff")]
    public Transform m_UICanvasTransform;

    [Header("Debugging References")]
    [SerializeField, Tooltip("The array of how many units have yet to make their turn. Meant for debugging purpose")]
    protected List<GameObject> m_UnitsYetToMakeMoves;
    [Tooltip("The number of image icons beneath it")]
    public List<Button> m_AllOfUnitUIIcon;
    [Tooltip("The current unit action that is clicked upon")]
    public IUnitAction m_CurrentSelectedAct;
    [Tooltip("The player unit that has been clicked upon")]
    public GameObject m_SelectedPlayerUnit;

    /// <summary>
    /// The update of this manager. So that it can be controlled anytime
    /// </summary>
    protected Coroutine m_UpdateOfManager;

    private void OnEnable()
    {
        GameEventSystem.GetInstance().SubscribeToEvent("PlayerAnnihilated", StopUpdate);
        GameEventSystem.GetInstance().SubscribeToEvent("EnemyAnnihilated", StopUpdate);
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("ClickedUnit", PlayerSelectUnit);
        GameEventSystem.GetInstance().SubscribeToEvent("ToggleSelectingUnit", ToggleThePlayerInput);
        GameEventSystem.GetInstance().SubscribeToEvent("UnitFinishAction", PollingForPlayerInput);
    }

    private void OnDisable()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent("PlayerAnnihilated", StopUpdate);
        GameEventSystem.GetInstance().UnsubscribeFromEvent("EnemyAnnihilated", StopUpdate);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("ClickedUnit", PlayerSelectUnit);
        GameEventSystem.GetInstance().UnsubscribeFromEvent("ToggleSelectingUnit", ToggleThePlayerInput);
        GameEventSystem.GetInstance().UnsubscribeFromEvent("UnitFinishAction", PollingForPlayerInput);
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
        GameEventSystem.GetInstance().TriggerEvent("TurnEnded");
        // Player no longer needs to interact with the game so might as well turn off the polling
        m_PlayerInputOnUnit.enabled = false;
        yield break;
    }
	
    /// <summary>
    /// To stop the coroutine update of this game object
    /// </summary>
    protected void StopUpdate()
    {
        ObserverSystemScript.Instance.UnsubscribeEvent("UnitMakeMove", UnitHasMakeMove);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("ClickedUnit", PlayerSelectUnit);
        ObserverSystemScript.Instance.UnsubscribeEvent("UnitFinishAction", PollingForPlayerInput);
        if (m_UpdateOfManager != null)
        {
            StopCoroutine(m_UpdateOfManager);
            m_UpdateOfManager = null;
        }
    }

    /// <summary>
    /// To recognize that the unit has already made a move and remove it from the list!
    /// </summary>
    protected void UnitHasMakeMove()
    {
        m_UnitsYetToMakeMoves.Remove(ObserverSystemScript.Instance.GetStoredEventVariable<GameObject>("UnitMakeMove"));
        ObserverSystemScript.Instance.RemoveTheEventVariableNextFrame("UnitMakeMove");
    }

    protected void UnitHasMakeMove(GameObject _unitGoFinished)
    {
        m_UnitsYetToMakeMoves.Remove(_unitGoFinished);
    }

    protected void PlayerSelectUnit(GameObject _UnitGO)
    {
        // If only the clicked unit belongs to the player and it must be inside the list of player unit that has yet to make a move!
        if (_UnitGO.tag == "Player" && m_UnitsYetToMakeMoves.Contains(_UnitGO))
        {
            m_ScrollRectUnitIcons.SetActive(true);
            // Need to ensure the selectedPlayerUnit is thr
            m_SelectedPlayerUnit = _UnitGO;
            IUnitAction[] allPossibleUnitActions = _UnitGO.GetComponentsInChildren<IUnitAction>();
            #region Clear The old buttons
            foreach (Button zeUnitButton in m_AllOfUnitUIIcon)
            {
                Destroy(zeUnitButton.gameObject);
            }
            m_AllOfUnitUIIcon.Clear();
            #endregion
            // When we will instantiate the button according to the amount of unit actions there are!
            foreach (IUnitAction zeUnitAction in allPossibleUnitActions)
            {
                Image zeUnitIconUI = Instantiate(m_UnitActionUIIconGO, m_HolderOfIcons.transform);
                // Replace the sprite
                zeUnitIconUI.gameObject.SetActive(true);
                zeUnitIconUI.sprite = zeUnitAction.ActionIconUI;
                // Add to the onclick event for Unity button!
                Button zeUnitIconButton = zeUnitIconUI.GetComponent<Button>();
                zeUnitIconButton.onClick.AddListener(() => ToInstantiateSpecificActionUI(zeUnitAction.UnitActionUI, zeUnitAction));
                m_AllOfUnitUIIcon.Add(zeUnitIconButton);
            }
        }
    }


    /// <summary>
    /// Helps to toggle the observer action subscription and the UI too!
    /// </summary>
    protected void ToggleThePlayerInput()
    {
        //m_PlayerInputOnUnit.enabled = !m_PlayerInputOnUnit.enabled;
        m_ScrollRectUnitIcons.SetActive(!m_ScrollRectUnitIcons.activeSelf);
        switch (m_ScrollRectUnitIcons.activeSelf)
        {
            case true:
                GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("ClickedUnit", PlayerSelectUnit);
                break;
            default:
                GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("ClickedUnit", PlayerSelectUnit);
                break;
        }
    }

    /// <summary>
    /// Starts polling for user input from GetPlayerInput
    /// </summary>
    protected void PollingForPlayerInput()
    {
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("ClickedUnit", PlayerSelectUnit);
    }

    /// <summary>
    /// Instantiating the specific action UI and keeping the reference to it!
    /// </summary>
    /// <param name="_SpecificUIGO">The Specific Action UI GameObject</param>
    /// <param name="_SelectedAction">The reference to that UI</param>
    public void ToInstantiateSpecificActionUI(GameObject _SpecificUIGO, IUnitAction _SelectedAction)
    {
        m_CurrentSelectedAct = _SelectedAction;
        Instantiate(_SpecificUIGO, m_UICanvasTransform).SetActive(true);
        GameEventSystem.GetInstance().TriggerEvent<IUnitAction>("SelectedAction", m_CurrentSelectedAct);
    }
}
