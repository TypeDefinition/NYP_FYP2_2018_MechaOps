using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class ActionPointsCounter : MonoBehaviour
{
    [SerializeField] private int m_MaxActionPoints = 5;
    [SerializeField] private int m_CurrentActionPoints = 5;
    [SerializeField] private TextMeshProUGUI m_ActionPointsText;
    [SerializeField] private Color m_FullActionPointsColor = Color.cyan;
    [SerializeField] private Color m_NotFullActionPointsColor = Color.yellow;
    [SerializeField] private Color m_NoActionPointsColor = Color.red;

    private bool m_ValuesChanged = true; // Dirty Flag

    public int MaxActionPoints
    {
        get { return m_MaxActionPoints; }
        set
        {
            m_MaxActionPoints = Mathf.Max(0, value);
            m_CurrentActionPoints = Mathf.Min(m_CurrentActionPoints, m_MaxActionPoints);
            m_ValuesChanged = true;
        }
    }

    public int CurrentActionPoints
    {
        get { return m_CurrentActionPoints; }
        set { m_CurrentActionPoints = Mathf.Clamp(value, 0, m_MaxActionPoints); m_ValuesChanged = true; }
    }

    private void SetActionPointsText()
    {
        Assert.IsTrue(m_ActionPointsText != null, MethodBase.GetCurrentMethod().Name + " - m_ActionPointsText cannot be null!");
        m_ActionPointsText.text = m_CurrentActionPoints.ToString() + "/" + m_MaxActionPoints.ToString();
        if (m_CurrentActionPoints >= MaxActionPoints)
        {
            m_ActionPointsText.color = m_FullActionPointsColor;
        }
        else if (m_CurrentActionPoints <= 0)
        {
            m_ActionPointsText.color = m_NoActionPointsColor;
        }
        else
        {
            m_ActionPointsText.color = m_NotFullActionPointsColor;
        }
    }

    private void Awake()
    {
        Assert.IsTrue(m_ActionPointsText != null, MethodBase.GetCurrentMethod().Name + " - m_ActionPointsText cannot be null!");
    }

    private void Start()
    {
        SetActionPointsText();
    }

    private void Update()
    {
        if (m_ValuesChanged)
        {
            SetActionPointsText();
            m_ValuesChanged = false;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        MaxActionPoints = m_MaxActionPoints;
        CurrentActionPoints = m_CurrentActionPoints;
    }
#endif // UNITY_EDITOR

}