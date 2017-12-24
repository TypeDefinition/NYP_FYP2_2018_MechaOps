using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public delegate void Void_UnitStats(UnitStats _unitStat);

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

[DisallowMultipleComponent]
public class UnitStats : MonoBehaviour
{
    // Serialized Variable(s)
    [SerializeField] private GameSystemsDirectory m_GameSystemsDirectory = null;
    [SerializeField] private UnitStatsJSON m_UnitStatsJSON = new UnitStatsJSON();
    [SerializeField] private TileAttributeOverride[] m_TileAttributeOverrides = null;
    [SerializeField] private ViewScript m_ViewTileScript = null;
    [SerializeField] private UnitInfoDisplay m_UnitInfoDisplay_Prefab = null;

    // Why are these all serialized?
    [SerializeField, Tooltip("The current active action so that it can be stopped")]
    private IUnitAction m_CurrentActiveAction;
    [SerializeField, Tooltip("The list of units that are in range")]
    private List<GameObject> m_EnemiesInRange = new List<GameObject>();

    // Non-Serialized Variable(s)
    private TileSystem m_TileSystem = null;
    private UnitsTracker m_UnitsTracker = null;
    private UnitInfoDisplay m_UnitInfoDisplay = null;

    /// <summary>
    /// A callback function will appear when ever the health point decreases
    /// </summary>
    public Void_UnitStats m_HealthDropCallback;

    public GameSystemsDirectory GetGameSystemsDirectory() { return m_GameSystemsDirectory; }

    public void InvokeHealthDropCallback(UnitStats _unitStats)
    {
        if (m_HealthDropCallback != null)
        {
            m_HealthDropCallback(_unitStats);
        }
    }

    public string Name
    {
        get { return m_UnitStatsJSON.m_Name; }
    }

    public int MaxHealthPoints
    {
        get { return m_UnitStatsJSON.m_MaxHealthPoints; }
        set
        {
            m_UnitStatsJSON.m_MaxHealthPoints = Mathf.Max(0, value);
            m_UnitStatsJSON.m_CurrentHealthPoints = Mathf.Min(m_UnitStatsJSON.m_CurrentHealthPoints, m_UnitStatsJSON.m_MaxHealthPoints);
            UpdateUnitInfoDisplay();
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
                string zeEventName = tag + "IsDead";
                //// Trigger an event when the unit died
                GameEventSystem.GetInstance().TriggerEvent<GameObject>(zeEventName, gameObject);
                // If the unit is visible, then start the death cinematic!
                if (m_ViewTileScript.IsVisible())
                {
                    GameEventSystem.GetInstance().TriggerEvent<GameObject>(tag + "VisibleDead", gameObject);
                }
            }

            UpdateUnitInfoDisplay();
        }
    }

    public void ResetHealthPoints()
    {
        CurrentHealthPoints = MaxHealthPoints;
        UpdateUnitInfoDisplay();
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
            UpdateUnitInfoDisplay();
        }
    }

    public int CurrentActionPoints
    {
        get { return m_UnitStatsJSON.m_CurrentActionPoints; }
        set
        {
            m_UnitStatsJSON.m_CurrentActionPoints = Mathf.Clamp(value, 0, m_UnitStatsJSON.m_MaxActionPoints);
            UpdateUnitInfoDisplay();
        }
    }

    public void ResetActionPoints()
    {
        CurrentActionPoints = MaxActionPoints;
        UpdateUnitInfoDisplay();
    }

    public int DeploymentCost
    {
        get { return m_UnitStatsJSON.m_DeploymentCost; }
        set
        {
            m_UnitStatsJSON.m_DeploymentCost = Mathf.Max(0, value);
            UpdateUnitInfoDisplay();
        }
    }

    public int ViewRange
    {
        get { return m_UnitStatsJSON.m_ViewRange; }
        set
        {
            m_UnitStatsJSON.m_ViewRange = Mathf.Max(0, value);
            UpdateUnitInfoDisplay();
        }
    }

    public int ConcealmentPoints
    {
        get { return m_UnitStatsJSON.m_ConcealmentPoints; }
        set
        {
            m_UnitStatsJSON.m_ConcealmentPoints = Mathf.Max(0, value);
            UpdateUnitInfoDisplay();
        }
    }

    public int EvasionPoints
    {
        get { return m_UnitStatsJSON.m_EvasionPoints; }
        set
        {
            m_UnitStatsJSON.m_EvasionPoints = Mathf.Max(0, value);
            UpdateUnitInfoDisplay();
        }
    }

    public TileId CurrentTileID
    {
        get { return m_UnitStatsJSON.m_CurrentTileID; }
        set {
            // we will need to make sure that the value is not the same!
            if (!value.Equals(m_UnitStatsJSON.m_CurrentTileID))
            {
                // and then set the previous tile to have no units since it has moved towards a new tile
                m_TileSystem.GetTile(m_UnitStatsJSON.m_CurrentTileID).Unit = null;
                m_UnitStatsJSON.m_CurrentTileID = value;
                m_TileSystem.GetTile(value).Unit = gameObject;
                UpdateUnitInfoDisplay();
            }
        }
    }

    public IUnitAction CurrentActiveAction
    {
        get { return m_CurrentActiveAction; }
        set
        {
            m_CurrentActiveAction = value;
            UpdateUnitInfoDisplay();
        }
    }

    public TileAttributeOverride[] GetTileAttributeOverrides()
    {
        return m_TileAttributeOverrides;
    }

    public List<GameObject> EnemiesInRange
    {
        get
        {
            return m_EnemiesInRange;
        }
    }

    private void Awake()
    {
        // Ensure that we have the system(s) we require.
        Assert.IsTrue(m_GameSystemsDirectory != null, MethodBase.GetCurrentMethod().Name + " - m_GameSystemsDirectory must not be null!");
        m_TileSystem = m_GameSystemsDirectory.GetTileSystem();
        Assert.IsTrue(m_TileSystem != null, MethodBase.GetCurrentMethod().Name + " - m_TileSystem must not be null!");
        m_UnitsTracker = m_GameSystemsDirectory.GetUnitsTracker();
        Assert.IsTrue(m_UnitsTracker != null, MethodBase.GetCurrentMethod().Name + " - m_UnitsTracker must not be null!");

        Assert.IsTrue(m_ViewTileScript != null);
        Assert.IsTrue(m_UnitStatsJSON.m_Name != null);

        // Ensure that we have the prefab(s) we require.
        Assert.IsTrue(m_UnitInfoDisplay_Prefab != null, MethodBase.GetCurrentMethod().Name + " - m_UnitInfoDisplay_Prefab must not be null!");

        // Initialisation
        m_UnitInfoDisplay = GameObject.Instantiate(m_UnitInfoDisplay_Prefab, m_GameSystemsDirectory.GetScreenSpaceCanvas().transform);
        m_UnitInfoDisplay.SetUnitStats(this);
    }

    private IEnumerator Start()
    {
        // check if there is any enemy in range when the game is starting!
        CheckEnemiesInViewRange();
        m_ViewTileScript.SetVisibleTiles();
        UpdateUnitInfoDisplay();

        m_TileSystem.GetTile(CurrentTileID).Unit = gameObject;

        yield break;
    }

    private void OnEnable()
    {
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("UnitMovedToTile", CheckEnemyInViewRange);
        switch (tag)
        {
            case "Player":
                // so get the list of enemy units
                GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("EnemyUnitIsDead", EnemyInRangeDead);
                break;
            case "EnemyUnit":
                GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("PlayerIsDead", EnemyInRangeDead);
                break;
            default:
                Assert.IsTrue(false, "Make CheckEnemyInRange more robust so that there can be more factions!");
                break;
        }
    }

    private void OnDisable()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("UnitMovedToTile", CheckEnemyInViewRange);
        switch (tag)
        {
            case "Player":
                // so get the list of enemy units
                GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("EnemyUnitIsDead", EnemyInRangeDead);
                break;
            case "EnemyUnit":
                GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("PlayerIsDead", EnemyInRangeDead);
                break;
            default:
                Assert.IsTrue(false, "Make CheckEnemyInRange more robust so that there can be more factions!");
                break;
        }
    }

    /// <summary>
    /// TODO: expand these upon 
    /// </summary>
    public void ResetUnitStats()
    {
        ResetHealthPoints();
        ResetActionPoints();
    }

    /// <summary>
    /// Check whether is it in range.
    /// Should be called every time a unit moves from it's tile!
    /// However, it will check all of the opposition units!
    /// </summary>
    public void CheckEnemiesInViewRange()
    {
        List<GameObject> unitsList = null;
        // TODO: Remove this hardcoding when it is done!
        switch (tag)
        {
            case "Player":
                // so get the list of enemy units
                unitsList = m_UnitsTracker.m_AliveEnemyUnits;
                break;
            case "EnemyUnit":
                unitsList = m_UnitsTracker.m_AlivePlayerUnits;
                break;
            default:
                Assert.IsTrue(false, "Make CheckEnemyInRange more robust so that there can be more factions!");
                break;
        }

        foreach (GameObject unit in unitsList)
        {
            UnitStats unitStats = unit.GetComponent<UnitStats>();
            if (!unitStats.IsAlive()) { continue; }

            int tileDistance = TileId.GetDistance(CurrentTileID, unitStats.CurrentTileID) + unitStats.ConcealmentPoints;
            // if that list does not have the unit in range!
            if (!m_EnemiesInRange.Contains(unit))
            {
                if (tileDistance <= ViewRange && RaycastToOtherPosition(unit.transform))
                {
                    m_EnemiesInRange.Add(unit);
                    unitStats.m_ViewTileScript.IncreaseVisibility();
                }
            }
            else
            {
                if (tileDistance > ViewRange || !RaycastToOtherPosition(unit.transform))
                {
                    // if the opposing unit is in range and 
                    m_EnemiesInRange.Remove(unit);
                    unitStats.m_ViewTileScript.DecreaseVisibility();
                }
            }
        }
    }

    private void UpdateUnitInfoDisplay()
    {
        // Update Health
        m_UnitInfoDisplay.GetHealthBar().MaxHealthPoints = MaxHealthPoints;
        m_UnitInfoDisplay.GetHealthBar().CurrentHealthPoints = CurrentHealthPoints;

        // Update Action Points
        m_UnitInfoDisplay.GetActionPointsCounter().MaxActionPoints = MaxActionPoints;
        m_UnitInfoDisplay.GetActionPointsCounter().CurrentActionPoints = CurrentActionPoints;
    }

    /// <summary>
    /// A function call to be passed to when a unit has moved!
    /// </summary>
    /// <param name="_movedUnit"></param>
    public void CheckEnemyInViewRange(GameObject _movedUnit)
    {
        if (!CompareTag(_movedUnit.tag))
        {
            if (_movedUnit != gameObject)
            {
                UnitStats zeGOState = _movedUnit.GetComponent<UnitStats>();
                int zeTileDist = TileId.GetDistance(CurrentTileID, zeGOState.CurrentTileID) + zeGOState.ConcealmentPoints;
                // Need to make sure that the moveunit is not itself! and the tag is the opposing unit!
                if (!m_EnemiesInRange.Contains(_movedUnit))
                {
                    if (zeTileDist <= ViewRange && RaycastToOtherPosition(_movedUnit.transform))
                    {
                        m_EnemiesInRange.Add(_movedUnit);
                        // so we have to render the enemy unit if it belongs to the enemy
                        zeGOState.m_ViewTileScript.IncreaseVisibility();
                    }
                }
                else
                {
                    if (zeTileDist > ViewRange || !RaycastToOtherPosition(_movedUnit.transform))
                    {
                        // if the opposing unit is in range and 
                        m_EnemiesInRange.Remove(_movedUnit);
                        zeGOState.m_ViewTileScript.DecreaseVisibility();
                    }
                }
            }
        }
        else
        {
            // if u are the 1 moving, check for nearby enemies
            if (gameObject == _movedUnit)
            {
                CheckEnemiesInViewRange();
                m_ViewTileScript.SetVisibleTiles();
            }
        }
    }
    /// <summary>
    /// To be called when the opposing unit died
    /// </summary>
    private void EnemyInRangeDead(GameObject _deadUnit)
    {
        // if the dead unit is itself, iterate through it's list and decrease the visibility of other units!
        if (_deadUnit == gameObject)
        {
            foreach (GameObject zeEnemyGO in m_EnemiesInRange)
            {
                ViewScript zeEnemyView = zeEnemyGO.GetComponent<ViewScript>();
                zeEnemyView.DecreaseVisibility();
            }
            m_EnemiesInRange.Clear();
        }
        else
        {
            m_EnemiesInRange.Remove(_deadUnit);
        }
    }

    public bool RaycastToOtherPosition(Transform _otherTrans)
    {
        // do a raycast between this gamobject and other go
        int layerMask = LayerMask.GetMask("TileDisplay");
        Vector3 rayDirection = _otherTrans.position - transform.position;
        return !Physics.Raycast(transform.position, rayDirection, rayDirection.magnitude, layerMask);
    }

#if UNITY_EDITOR
    private void ValidateTileOverrides()
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

    private void OnValidate()
    {
        ValidateTileOverrides();
    }
#endif // UNITY_EDITOR
}