using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Set the current selected image and name to the player selected unit!
/// </summary>
public class SpawnUnitUI_Logic : MonoBehaviour {
    [Header("Variable for SpawnUnitUI_Logic")]
    [SerializeField, Tooltip("Text for the player credits")]
    protected TextMeshProUGUI m_TMProCreditText;
    [SerializeField, Tooltip("Layout group of the unit's UI")]
    protected GridLayoutGroup m_UnitLayoutUI;
    [SerializeField, Tooltip("Vertical Layout groups of the unit selection button")]
    protected VerticalLayoutGroup m_UnitSelectionLayoutUI;
    [SerializeField, Tooltip("Finished button")]
    protected Button m_FinishedButton;
    [SerializeField, Tooltip("To signal insufficient credit UI")]
    protected TweenDisableScript m_InsufficientCreditUI;
    [SerializeField, Tooltip("Time taken to disable Insufficient credit UI")]
    protected float m_TimeToDisableInsufficientUI = 2.0f;
    [SerializeField, Tooltip("Units data and asset")]
    protected UnitDataAndCost m_UnitScriptObject;
    [SerializeField, Tooltip("UnitSelectUI in this UI")]
    protected UnitSelectUI m_UnitSelectUI;
    [SerializeField, Tooltip("Unit slot UI in this UI")]
    protected UnitSlotUI m_SlotUI;

    [Header("Debugging for SpawnUnitUI_Logic")]
    [SerializeField, Tooltip("Spawn System")]
    protected SpawnUnitSystem m_SpawnSystem;
    [SerializeField, Tooltip("Array referenced from SpawnUnitSystem")]
    protected UnitSlotUI[] m_ArrayOfSpawnUI;
    [SerializeField, Tooltip("The currently select SpawnUI")]
    protected UnitSlotUI m_CurrentSelectedSlot;
    [SerializeField, Tooltip("Array of units that can be selected")]
    protected UnitSelectUI[] m_ArrayOfUnitSelectUI;

    Coroutine m_InsufficientUICoroutine;

    public string PlayerCreditText
    {
        set
        {
            m_TMProCreditText.text = value;
        }
        get
        {
            return m_TMProCreditText.text;
        }
    }

    public GridLayoutGroup UnitLayoutUI
    {
        get
        {
            return m_UnitLayoutUI;
        }
    }

    public Button FinishedButton
    {
        get
        {
            return m_FinishedButton;
        }
    }

    public VerticalLayoutGroup UnitSelectionLayoutUI
    {
        get
        {
            return m_UnitSelectionLayoutUI;
        }
    }

    public UnitSlotUI[] ArrayOfSpawnUI
    {
        set
        {
            m_ArrayOfSpawnUI = value;
            // Make sure each of the button will work
            foreach (UnitSlotUI zeSlot in m_ArrayOfSpawnUI)
            {
                zeSlot.ClearButton.onClick.AddListener(() => ClearUnit(zeSlot));
            }
        }
        get
        {
            return m_ArrayOfSpawnUI;
        }
    }
    
    public UnitSlotUI SlotUI
    {
        get
        {
            return m_SlotUI;
        }
    }

    public SpawnUnitSystem SpawnSystem
    {
        set
        {
            m_SpawnSystem = value;
        }
        get
        {
            return m_SpawnSystem;
        }
    }

    /// <summary>
    /// It will only
    /// </summary>
    public void SetInsufficientUI_Active()
    {
        if (m_InsufficientUICoroutine == null)
        {
            m_InsufficientCreditUI.gameObject.SetActive(true);
            m_InsufficientUICoroutine = StartCoroutine(SetInsufficientUICoroutine());
        }
    }

    IEnumerator SetInsufficientUICoroutine()
    {
        yield return new WaitForSeconds(m_TimeToDisableInsufficientUI);
        m_InsufficientCreditUI.AnimateUI();
        m_InsufficientUICoroutine = null;
        yield break;
    }

    private void Start()
    {
        m_ArrayOfUnitSelectUI = new UnitSelectUI[m_UnitScriptObject.UnitUIDataArray.Length];
        for (int num = 0; num < m_UnitScriptObject.UnitUIDataArray.Length; ++num)
        {
            UnitDataAndCost.UnitUI_Data zeUnitUIData = m_UnitScriptObject.UnitUIDataArray[num];
            UnitSelectUI zeSelectUI = Instantiate(m_UnitSelectUI.gameObject, m_UnitSelectionLayoutUI.transform, false).GetComponent<UnitSelectUI>();
            zeSelectUI.PassInUnitStat(zeUnitUIData.m_UnitPrefabDataReference.m_UnitStatsPrefab, zeUnitUIData.m_UnitSpriteUI, zeUnitUIData.m_UnitPrefabDataReference.Cost);
            m_ArrayOfUnitSelectUI[num] = zeSelectUI;
            Button zeSelectUIButton = zeSelectUI.GetComponent<Button>();
            zeSelectUIButton.onClick.AddListener(() => PressedUnitSelectUI(zeUnitUIData));
            zeSelectUIButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// When pressed the Unit Select UI, button pressed will come to here!
    /// </summary>
    /// <param name="_UnitData"></param>
    private void PressedUnitSelectUI(UnitDataAndCost.UnitUI_Data _UnitData)
    {
        if (m_SpawnSystem.PlayerCredits + m_CurrentSelectedSlot.UnitData.m_UnitPrefabDataReference.Cost < _UnitData.m_UnitPrefabDataReference.Cost)
        {
            SetInsufficientUI_Active();
        }
        else
        {
            m_SpawnSystem.PlayerCredits += m_CurrentSelectedSlot.UnitData.m_UnitPrefabDataReference.Cost;
            // we need to take account of the previous unit data!
            m_CurrentSelectedSlot.UnitData = _UnitData;
            m_SpawnSystem.PlayerCredits -= _UnitData.m_UnitPrefabDataReference.Cost;
            m_TMProCreditText.text = m_SpawnSystem.PlayerCredits.ToString();
        }
    }

    /// <summary>
    /// When player press clear
    /// </summary>
    public void ClearUnit(UnitSlotUI _slotUI)
    {
        m_SpawnSystem.PlayerCredits += _slotUI.UnitData.m_UnitPrefabDataReference.Cost;
        m_TMProCreditText.text = m_SpawnSystem.PlayerCredits.ToString();
        _slotUI.UnitData = new UnitDataAndCost.UnitUI_Data();
    }

    public void EditCurrentSlot(UnitSlotUI _UnitSlotUI)
    {
        m_CurrentSelectedSlot = _UnitSlotUI;
    }
}
