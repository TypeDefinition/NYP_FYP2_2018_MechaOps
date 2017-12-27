using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitSlotUI : MonoBehaviour {
    [Header("Variables for SpawnUI")]
    [SerializeField, Tooltip("Unit data that the player has selected")]
    protected UnitDataAndCost.UnitUI_Data m_UnitData;
    [SerializeField, Tooltip("Unit holder UI gameobject")]
    protected GameObject m_UnitHolderUI;
    [SerializeField, Tooltip("No Unit display UI")]
    protected GameObject m_NoUnitDisplayUI;
    [SerializeField, Tooltip("Button for the clear")]
    protected Button m_ClearButton;
    [SerializeField, Tooltip("Edit unit button")]
    protected Button m_EditButton;
    [SerializeField, Tooltip("Add unit button")]
    protected Button m_AddUnitButton;
    [SerializeField, Tooltip("Image of the UnitSlot for Unit icon UI")]
    protected Image m_UnitIconUI;
    [SerializeField, Tooltip("Text of the unit cost UI")]
    protected TextMeshProUGUI m_UnitCostTextUI;

    public UnitDataAndCost.UnitUI_Data UnitData
    {
        set
        {
            m_UnitData = value;
            if (value.m_UnitPrefabDataReference.Cost > 0)
            {
                m_UnitHolderUI.SetActive(true);
                m_NoUnitDisplayUI.SetActive(false);
                m_UnitIconUI.sprite = value.m_UnitSpriteUI;
                m_UnitCostTextUI.text = "Cost: " + value.m_UnitPrefabDataReference.Cost;
            }
            else
            {
                m_UnitHolderUI.SetActive(false);
                m_NoUnitDisplayUI.SetActive(true);
            }
        }
        get
        {
            return m_UnitData;
        }
    }

    public Button ClearButton
    {
        get
        {
            return m_ClearButton;
        }
    }

    public Button EditButton
    {
        get
        {
            return m_EditButton;
        }
    }

    public Button AddUnitButton
    {
        get
        {
            return m_AddUnitButton;
        }
    }

    public GameObject UnitHolderUI
    {
        get
        {
            return m_UnitHolderUI;
        }
    }

    public GameObject NoUnitDisplayUI
    {
        get
        {
            return m_NoUnitDisplayUI;
        }
    }
}
