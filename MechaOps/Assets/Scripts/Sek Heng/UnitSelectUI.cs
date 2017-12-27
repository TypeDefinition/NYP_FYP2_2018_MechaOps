using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// The UI icon that allows player to select it's unit
/// </summary>
public class UnitSelectUI : MonoBehaviour {
    [Header("Variables for UnitSelectUI")]
    [SerializeField, Tooltip("Text UI for unit name")]
    protected TextMeshProUGUI m_UnitNameUI;
    [SerializeField, Tooltip("Text for unit description")]
    protected TextMeshProUGUI m_UnitDescriptionUI;
    [SerializeField, Tooltip("Text for unit stat")]
    protected TextMeshProUGUI m_UnitStatUI;
    [SerializeField, Tooltip("Text for unit cost")]
    protected TextMeshProUGUI m_UnitCostUI;
    [SerializeField, Tooltip("Image for unit icon")]
    protected Image m_UnitIconUI;

    [Header("Debugging for UnitSelectUI")]
    [SerializeField, Tooltip("Unit stat in this UI")]
    UnitStats m_UnitStat;

    /// <summary>
    /// When pass in the unit stat, it will also change the text UI for it
    /// </summary>
    /// <param name="_unitStat">Stats of the unit</param>
    /// <param name="_UnitIconSprite">Unit icon for references</param>
    /// <param name="_CostOfUnit">Unit cost</param>
    public void PassInUnitStat(UnitStats _unitStat, Sprite _UnitIconSprite, int _CostOfUnit)
    {
        m_UnitIconUI.sprite = _UnitIconSprite;
        m_UnitNameUI.SetText(_unitStat.Name);
        m_UnitDescriptionUI.SetText(_unitStat.Description);
        m_UnitStatUI.text = string.Format("hp: {0}\nAttack: {1}\nView Range: {2}\nEvasion: {3}\nConcealment: {4}", _unitStat.MaxHealthPoints, _unitStat.GetComponent<UnitAttackAction>().DamagePoints, _unitStat.ViewRange, _unitStat.EvasionPoints, _unitStat.ConcealmentPoints);
        m_UnitCostUI.SetText("Cost: {0}", _CostOfUnit);
        m_UnitStat = _unitStat;
    }
}
