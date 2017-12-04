using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [Tooltip("UI Stats display script")]
    public UnitDisplayUI m_UnitDisplayUI;
    [SerializeField, Tooltip("The gameobject to toggle between selecting units")]
    protected GameObject m_SelectBetUnits;
    [SerializeField, Tooltip("The text UI to display the unit's name!")]
    protected TextMeshProUGUI m_UnitNameTxtUI;

    [Header("Debugging References")]
    [SerializeField, Tooltip("The array of how many units have yet to make their turn. Meant for debugging purpose")]
    protected List<GameObject> m_UnitsYetToMakeMoves;
    [Tooltip("The number of image icons beneath it")]
    public List<Button> m_AllOfUnitUIIcon;
    [Tooltip("The current unit action that is clicked upon")]
    public IUnitAction m_CurrentSelectedAct;
    [Tooltip("The player unit that has been clicked upon")]
    public GameObject m_SelectedPlayerUnit;
    [SerializeField, Tooltip("The world canvas transform")]
    protected Transform m_worldCanvasTrans;
    [SerializeField, Tooltip("The instantiated unit display UI")]
    protected UnitDisplayUI m_InstantUnitUI;
    [SerializeField, Tooltip("The index of the current selected unit for select between units")]
    protected int m_IndexOfCurrSelected = 0;

    /// <summary>
    /// The update of this manager. So that it can be controlled anytime
    /// </summary>
    protected Coroutine m_UpdateOfManager;

    private void Start()
    {
        if (!m_worldCanvasTrans)
            m_worldCanvasTrans = GameObject.FindGameObjectWithTag("WorldCanvas").transform;
        if (!m_InstantUnitUI)
        {
            m_InstantUnitUI = Instantiate(m_UnitDisplayUI.gameObject, m_worldCanvasTrans).GetComponent<UnitDisplayUI>();
            m_InstantUnitUI.gameObject.SetActive(false);
        }
    }

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
        m_SelectBetUnits.SetActive(true);
        m_IndexOfCurrSelected = 0;
        // Get a shallow copy of the list of all available units!
        m_UnitsYetToMakeMoves = new List<GameObject>(KeepTrackOfUnits.Instance.m_AllPlayerUnitGO);
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("UnitMakeMove", UnitHasMakeMove);
        WaitForSecondsRealtime zeAmountOfWaitTime = new WaitForSecondsRealtime(0.1f);
        m_PlayerInputOnUnit.enabled = true;
        while (m_UnitsYetToMakeMoves.Count > 0)
        {
            yield return zeAmountOfWaitTime;
        }
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("UnitMakeMove", UnitHasMakeMove);
        m_UpdateOfManager = null;
        GameEventSystem.GetInstance().TriggerEvent("TurnEnded");
        // Player no longer needs to interact with the game so might as well turn off the polling
        m_PlayerInputOnUnit.enabled = false;
        m_SelectBetUnits.SetActive(false);
        yield break;
    }
	
    /// <summary>
    /// To stop the coroutine update of this game object
    /// </summary>
    protected void StopUpdate()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("UnitMakeMove", UnitHasMakeMove);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("ClickedUnit", PlayerSelectUnit);
        GameEventSystem.GetInstance().UnsubscribeFromEvent("UnitFinishAction", PollingForPlayerInput);
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

    protected void PlayerSelectUnit(GameObject _UnitGO)
    {
        // If only the clicked unit belongs to the player and it must be inside the list of player unit that has yet to make a move!
        if (_UnitGO.tag == "Player" && m_UnitsYetToMakeMoves.Contains(_UnitGO))
        {
            SpawnActionUI(_UnitGO);
        }
    }


    /// <summary>
    /// Helps to toggle the observer action subscription and the UI too!
    /// </summary>
    protected void ToggleThePlayerInput()
    {
        m_ScrollRectUnitIcons.SetActive(!m_ScrollRectUnitIcons.activeSelf);
        switch (m_ScrollRectUnitIcons.activeSelf)
        {
            case true:
                m_InstantUnitUI.gameObject.SetActive(true);
                m_SelectBetUnits.SetActive(true);
                GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("ClickedUnit", PlayerSelectUnit);
                break;
            default:
                m_InstantUnitUI.gameObject.SetActive(false);
                GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("ClickedUnit", PlayerSelectUnit);
                break;
        }
    }

    /// <summary>
    /// Starts polling for user input from GetPlayerInput
    /// </summary>
    protected void PollingForPlayerInput()
    {
        m_SelectBetUnits.SetActive(true);
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("ClickedUnit", PlayerSelectUnit);
    }

    /// <summary>
    /// Instantiating the specific action UI and keeping the reference to it!
    /// </summary>
    /// <param name="_SpecificUIGO">The Specific Action UI GameObject</param>
    /// <param name="_SelectedAction">The reference to that UI</param>
    public void ToInstantiateSpecificActionUI(GameObject _SpecificUIGO, IUnitAction _SelectedAction)
    {
        m_InstantUnitUI.gameObject.SetActive(false);
        m_SelectBetUnits.SetActive(false);
        m_CurrentSelectedAct = _SelectedAction;
        Instantiate(_SpecificUIGO, m_UICanvasTransform).SetActive(true);
        GameEventSystem.GetInstance().TriggerEvent<IUnitAction>("SelectedAction", m_CurrentSelectedAct);
    }

    /// <summary>
    /// Going to the next index
    /// </summary>
    public void NextUnit()
    {
        // this will ensure it will not go overboard
        m_IndexOfCurrSelected = Mathf.Min(m_IndexOfCurrSelected + 1, m_UnitsYetToMakeMoves.Count - 1);
        SpawnActionUI(m_UnitsYetToMakeMoves[m_IndexOfCurrSelected]);
    }

    /// <summary>
    /// Going to the previous index
    /// </summary>
    public void PreviousUnit()
    {
        // Need to clamp it!
        m_IndexOfCurrSelected = Mathf.Max(m_IndexOfCurrSelected - 1, 0);
        SpawnActionUI(m_UnitsYetToMakeMoves[m_IndexOfCurrSelected]);
    }

    /// <summary>
    /// It will help to spawn those action UI icons
    /// </summary>
    /// <param name="_go">The unit gameobject which must have the unit stats and unit action for sure!</param>
    protected void SpawnActionUI(GameObject _go)
    {
        m_UnitNameTxtUI.text = _go.name;
        m_ScrollRectUnitIcons.SetActive(true);
        // Need to ensure the selectedPlayerUnit is thr
        m_SelectedPlayerUnit = _go;
        IUnitAction[] allPossibleUnitActions = _go.GetComponentsInChildren<IUnitAction>();
        if (!m_InstantUnitUI.gameObject.activeSelf)
            m_InstantUnitUI.gameObject.SetActive(true);
        else
            m_InstantUnitUI.AnimateUI();
        m_InstantUnitUI.SetThePosToUnit(_go.GetComponent<UnitStats>());
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
