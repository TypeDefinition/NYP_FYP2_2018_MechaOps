using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// meant to just display the stats UI only!
/// </summary>
public class UnitInfoDisplay : TweenUI_Scale
{
    [Header("Variables needed")]
    [SerializeField, Tooltip("The HP text of the unit!")]
    protected TextMeshProUGUI m_HpTextUI;
    [SerializeField, Tooltip("The action point text of the unit")]
    protected TextMeshProUGUI m_ActionPtTextUI;
    [SerializeField, Tooltip("The gameobj holds the text of HP")]
    protected GameObject m_goHP;
    [SerializeField, Tooltip("The gameobj holds the text of action pt")]
    protected GameObject m_goActPt;
    [Tooltip("The distance to from the target to the camera")]
    public float m_Dist = 3.0f;

    private void OnEnable()
    {
        AnimateUI();
    }

    /// <summary>
    /// Meant to just set the value of the text ui
    /// </summary>
    public string HpText
    {
        set
        {
            m_goHP.SetActive(true);
            m_HpTextUI.text = value;
        }
        get
        {
            return m_HpTextUI.text;
        }
    }
    /// <summary>
    /// meant to set the action point text
    /// </summary>
    public string ActPtText
    {
        set
        {
            m_goActPt.SetActive(true);
            m_ActionPtTextUI.text = value;
        }
        get
        {
            return m_ActionPtTextUI.text;
        }
    }

    /// <summary>
    /// set the position of the UI to the camera
    /// </summary>
    /// <param name="_unitGo">Transform of the gameobject</param>
    public void SetThePosToUnit(Transform _unitGo)
    {
        // we will use the distance from the camera to the object and determine the position there. so we will need the direction vector and from there scale the distance of the point
        Vector3 zeDirFromTargetToCam = Camera.main.transform.position - _unitGo.position;
        zeDirFromTargetToCam.Normalize();
        Vector3 zeTrackedUIPos = (zeDirFromTargetToCam * m_Dist) + _unitGo.position;
        transform.position = zeTrackedUIPos;
    }

    /// <summary>
    /// Set the position of the UI to camera and set the value automatically
    /// </summary>
    /// <param name="_unitStat">The Unit stats</param>
    public void SetThePosToUnit(UnitStats _unitStat)
    {
        HpText = _unitStat.CurrentHealthPoints + "/" + _unitStat.MaxHealthPoints;
        ActPtText = _unitStat.CurrentActionPoints.ToString();
        SetThePosToUnit(_unitStat.transform);
    }
}
