using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

/// <summary>
/// Created with JSON formatter in mind!
/// </summary>
[System.Serializable]
public class UnitStatsJSON
{
    [SerializeField, Tooltip("The name of the unit")]
    public string m_Name;

    [SerializeField, Tooltip("The max healthpoint of the unit")]
    private int m_MaxHealthPoints;
    [SerializeField, Tooltip("The health points of the unit.")]
    private int m_CurrentHealthPoints;

    [SerializeField, Tooltip("Max action points of the unit")]
    private int m_MaxActionPoints;
    [SerializeField, Tooltip("The action points left for the unit.")]
    private int m_CurrentActionPoints;

    [SerializeField, Tooltip("The deployment cost of the unit")]
    private int m_DeploymentCost;

    [SerializeField, Tooltip("The view range of the unit")]
    private int m_ViewRange;

    [SerializeField, Tooltip("The concealment points of the unit")]
    private int m_ConcealmentPoints;
    [SerializeField, Tooltip("The evasion points of the unit")]
    private int m_EvasionPoints;
    
    public int MaxHealthPoints
    {
        get
        {
            return m_MaxHealthPoints;
        }
        set
        {
            m_MaxHealthPoints = Mathf.Max(0, value);
            m_CurrentHealthPoints = Mathf.Min(m_CurrentHealthPoints, m_MaxHealthPoints);
        }
    }

    public int CurrentHealthPoints
    {
        get
        {
            return m_CurrentHealthPoints;
        }
        set
        {
            m_CurrentHealthPoints = Mathf.Clamp(value, 0, m_MaxHealthPoints);
        }
    }

    public bool IsAlive()
    {
        return m_CurrentHealthPoints > 0;
    }

    public int MaxActionPoints
    {
        get
        {
            return m_MaxActionPoints;
        }
        set
        {
            m_MaxActionPoints = Mathf.Max(0, value);
            m_CurrentActionPoints = Mathf.Min(m_CurrentActionPoints, m_MaxHealthPoints);
        }
    }

    public int CurrentActionPoints
    {
        get
        {
            return m_CurrentActionPoints;
        }
        set
        {
            m_CurrentActionPoints = Mathf.Clamp(value, 0, m_MaxActionPoints);
        }
    }

    public int DeploymentCost
    {
        get { return m_DeploymentCost; }
        set { m_DeploymentCost = Mathf.Max(0, value); }
    }
    
    public int ViewRange
    {
        get { return m_ViewRange; }
        set { m_ViewRange = Mathf.Max(0, value); }
    }
    
    public int ConcealmentPoints
    {
        get { return m_ConcealmentPoints; }
        set { m_ConcealmentPoints = Mathf.Max(0, value); }
    }

    public int EvasionPoints
    {
        get { return m_EvasionPoints; }
        set { m_EvasionPoints = Mathf.Max(0, value); }
    }
}

public class UnitStats : MonoBehaviour {

    [Header("The references of the ")]
    [Tooltip("The unit stats information")]
    public UnitStatsJSON m_UnitStatsJSON = new UnitStatsJSON();
    public TileAttributeOverride[] m_TileAttributeOverrides;

    private void Start()
    {
        // Assign the gameobject name to the unit if there is none for the unit stat!
        if (m_UnitStatsJSON.m_Name == null)
        {
            m_UnitStatsJSON.m_Name = name;
        }
    }

    private void OnDestroy()
    {
        ObserverSystemScript.Instance.StoreVariableInEvent(tag + "IsDead", gameObject);
        // Trigger an event when the unit died
        ObserverSystemScript.Instance.TriggerEvent(tag + "IsDead");
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < m_TileAttributeOverrides.Length; ++i)
        {
            m_TileAttributeOverrides[i].EditorValidate();
        }

        HashSet<TileType> overrideTypeValidator = new HashSet<TileType>();
        for (int i = 0; i < m_TileAttributeOverrides.Length; ++i)
        {
            if (overrideTypeValidator.Contains(m_TileAttributeOverrides[i].Type))
            {
                string message = "More than 1 TileAttributeOverride has the same tile type!";
                EditorUtility.DisplayDialog("Duplicate TileType!", message, "OK");
                Debug.Log(message);
            }
            else
            {
                overrideTypeValidator.Add(m_TileAttributeOverrides[i].Type);
            }
        }
    }
#endif // UNITY_EDITOR

}