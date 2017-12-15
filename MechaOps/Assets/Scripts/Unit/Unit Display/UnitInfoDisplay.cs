using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

/// <summary>
/// meant to just display the stats UI only!
/// </summary>
public class UnitInfoDisplay : TweenUI_Scale
{
    [Header("Variables needed")]
    [SerializeField, Tooltip("The HP text of the unit!")]
    protected TextMeshProUGUI m_HealthPointsText;
    [SerializeField, Tooltip("The action point text of the unit")]
    protected TextMeshProUGUI m_ActionPointsText;
    [SerializeField, Tooltip("The gameobj holds the text of HP")]
    protected Image m_HealthPointsImage;
    [SerializeField, Tooltip("The gameobj holds the text of action pt")]
    protected Image m_ActionPointsImage;
    [Tooltip("The distance to from the target to the camera")]
    protected float m_CameraToTargetDistance = 3.0f;

    public float CameraToTargetDistance
    {
        get { return m_CameraToTargetDistance; }
        set { m_CameraToTargetDistance = Mathf.Max(0.0f, value); }
    }

    private void OnEnable()
    {
        AnimateUI();
    }

    /// <summary>
    /// Meant to just set the value of the text ui
    /// </summary>
    public string HealthPointsText
    {
        set
        {
            m_HealthPointsImage.gameObject.SetActive(true);
            m_HealthPointsText.text = value;
        }
        get
        {
            return m_HealthPointsText.text;
        }
    }
    /// <summary>
    /// meant to set the action point text
    /// </summary>
    public string ActPtText
    {
        set
        {
            m_ActionPointsImage.gameObject.SetActive(true);
            m_ActionPointsText.text = value;
        }
        get
        {
            return m_ActionPointsText.text;
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
        Vector3 zeTrackedUIPos = (zeDirFromTargetToCam * m_CameraToTargetDistance) + _unitGo.position;
        transform.position = zeTrackedUIPos;
    }

    /// <summary>
    /// Set the position of the UI to camera and set the value automatically
    /// </summary>
    /// <param name="_unitStat">The Unit stats</param>
    public void SetThePosToUnit(UnitStats _unitStat)
    {
        HealthPointsText = _unitStat.CurrentHealthPoints + "/" + _unitStat.MaxHealthPoints;
        ActPtText = _unitStat.CurrentActionPoints.ToString();
        SetThePosToUnit(_unitStat.transform);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        CameraToTargetDistance = m_CameraToTargetDistance;
    }
#endif // UNITY_EDITOR

}
