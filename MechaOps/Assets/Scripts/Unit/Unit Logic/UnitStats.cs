using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
    public int m_MaxHealthPoints;
    [SerializeField, Tooltip("The health points of the unit.")]
    public int m_CurrentHealthPoints;

    [SerializeField, Tooltip("Max action points of the unit")]
    public int m_MaxActionPoints;
    [SerializeField, Tooltip("The action points left for the unit.")]
    public int m_CurrentActionPoints;

    [SerializeField, Tooltip("The deployment cost of the unit")]
    public int m_DeploymentCost;

    [SerializeField, Tooltip("The view range of the unit")]
    public int m_ViewRange;

    [SerializeField, Tooltip("The concealment points of the unit")]
    public int m_ConcealmentPoints;
    [SerializeField, Tooltip("The evasion points of the unit")]
    public int m_EvasionPoints;

    [Tooltip("The current tile that the unit is at. Currently needs to be manually linked as the TileSystem cant identify current tile is at world space")]
    public TileId m_CurrentTileID;
}

public class UnitStats : MonoBehaviour
{
    [SerializeField]
    private UnitStatsJSON m_UnitStatsJSON = new UnitStatsJSON();
    [SerializeField, Tooltip("The current active action so that it can be stopped")]
    private IUnitAction m_CurrentActiveAction;
    [SerializeField, Tooltip("The list of units that are in range")]
    public UnitStats[] m_EnemyInRange;
    [SerializeField, Tooltip("The array of tiles override")]
    private TileAttributeOverride[] m_TileAttributeOverrides;

    public string Name
    {
        get { return m_UnitStatsJSON.m_Name; }
    }

    public int MaxHealthPoints
    {
        get
        {
            return m_UnitStatsJSON.m_MaxHealthPoints;
        }
        set
        {
            m_UnitStatsJSON.m_MaxHealthPoints = Mathf.Max(0, value);
            m_UnitStatsJSON.m_CurrentHealthPoints = Mathf.Min(m_UnitStatsJSON.m_CurrentHealthPoints, m_UnitStatsJSON.m_MaxHealthPoints);
        }
    }

    public int CurrentHealthPoints
    {
        get
        {
            return m_UnitStatsJSON.m_CurrentHealthPoints;
        }
        set
        {
            m_UnitStatsJSON.m_CurrentHealthPoints = Mathf.Clamp(value, 0, m_UnitStatsJSON.m_MaxHealthPoints);
            // Send the event that this unit is dead!
            if (m_UnitStatsJSON.m_CurrentHealthPoints <= 0)
            {
                ObserverSystemScript.Instance.StoreVariableInEvent(tag + "IsDead", gameObject);
                // Trigger an event when the unit died
                ObserverSystemScript.Instance.TriggerEvent(tag + "IsDead");
            }
        }
    }

    public void ResetHealthPoints()
    {
        CurrentHealthPoints = MaxHealthPoints;
    }

    public bool IsAlive()
    {
        return m_UnitStatsJSON.m_CurrentHealthPoints > 0;
    }

    public int MaxActionPoints
    {
        get
        {
            return m_UnitStatsJSON.m_MaxActionPoints;
        }
        set
        {
            m_UnitStatsJSON.m_MaxActionPoints = Mathf.Max(0, value);
            m_UnitStatsJSON.m_CurrentActionPoints = Mathf.Min(m_UnitStatsJSON.m_CurrentActionPoints, m_UnitStatsJSON.m_MaxHealthPoints);
        }
    }

    public int CurrentActionPoints
    {
        get { return m_UnitStatsJSON.m_CurrentActionPoints; }
        set { m_UnitStatsJSON.m_CurrentActionPoints = Mathf.Clamp(value, 0, m_UnitStatsJSON.m_MaxActionPoints); }
    }

    public void ResetActionPoints()
    {
        CurrentActionPoints = MaxActionPoints;
    }

    public int DeploymentCost
    {
        get { return m_UnitStatsJSON.m_DeploymentCost; }
        set { m_UnitStatsJSON.m_DeploymentCost = Mathf.Max(0, value); }
    }

    public int ViewRange
    {
        get { return m_UnitStatsJSON.m_ViewRange; }
        set { m_UnitStatsJSON.m_ViewRange = Mathf.Max(0, value); }
    }

    public int ConcealmentPoints
    {
        get { return m_UnitStatsJSON.m_ConcealmentPoints; }
        set { m_UnitStatsJSON.m_ConcealmentPoints = Mathf.Max(0, value); }
    }

    public int EvasionPoints
    {
        get { return m_UnitStatsJSON.m_EvasionPoints; }
        set { m_UnitStatsJSON.m_EvasionPoints = Mathf.Max(0, value); }
    }

    public TileId CurrentTileID
    {
        get { return m_UnitStatsJSON.m_CurrentTileID; }
        set { m_UnitStatsJSON.m_CurrentTileID = value; }
    }

    public IUnitAction CurrentActiveAction
    {
        get { return m_CurrentActiveAction; }
        set { m_CurrentActiveAction = value; }
    }

    public TileAttributeOverride[] GetTileAttributeOverrides()
    {
        return m_TileAttributeOverrides;
    }

    private void Start()
    {
        // Assign the gameobject name to the unit if there is none for the unit stat!
        if (m_UnitStatsJSON.m_Name == null)
        {
            m_UnitStatsJSON.m_Name = name;
        }
    }

    /// <summary>
    /// TODO: expand these upon 
    /// </summary>
    public void ResetUnitStat()
    {
        ResetHealthPoints();
        ResetActionPoints();
    }

    /// <summary>
    /// Check whether is it in range.
    /// Should be called every time a unit ends it's turn.
    /// </summary>
    public void CheckEnemyInRange()
    {
        GameObject[] zeListOfUnits = null;
        // TODO: Remove this hardcoding when it is done!
        switch (tag)
        {
            case "Player":
                // so get the list of enemy units
                zeListOfUnits = KeepTrackOfUnits.Instance.m_AllEnemyUnitGO.ToArray();
                break;
            case "EnemyUnit":
                zeListOfUnits = KeepTrackOfUnits.Instance.m_AllPlayerUnitGO.ToArray();
                break;
            default:
                Assert.IsTrue(false, "Make CheckEnemyInRange more robust so that there can be more factions!");
                break;
        }
        foreach (GameObject zeGO in zeListOfUnits)
        {

        }
    }

    //private void OnDestroy()
    //{
    //    ObserverSystemScript.Instance.StoreVariableInEvent(tag + "IsDead", gameObject);
    //    // Trigger an event when the unit died
    //    ObserverSystemScript.Instance.TriggerEvent(tag + "IsDead");
    //}

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