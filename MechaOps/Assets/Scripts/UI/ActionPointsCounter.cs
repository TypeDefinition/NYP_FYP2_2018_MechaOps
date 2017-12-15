using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionPointsCounter : MonoBehaviour
{
    [SerializeField] private int m_MaxActionPoints = 5;
    [SerializeField] private int m_CurrentActionPoints = 5;
    [SerializeField] private TextMeshProUGUI m_ActionPointsText;

    private bool m_ValuesChanged = false; // Dirty Flag

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
}