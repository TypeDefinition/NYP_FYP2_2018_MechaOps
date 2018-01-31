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
    public string m_UnitName;

    [SerializeField, Tooltip("Description of the unit")]
    public string m_UnitDescription;

    [SerializeField, Tooltip("The faction that this unit belongs to.")]
    public FactionType m_FactionType = FactionType.None;

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
    public TileId m_CurrentTileID = new TileId(0, 0);
}

[DisallowMultipleComponent]
public class UnitStats : MonoBehaviour
{
    // Serialized Variable(s)
    [SerializeField] private UnitStatsJSON m_UnitStatsJSON = new UnitStatsJSON();
    [SerializeField] private TileAttributeOverride[] m_TileAttributeOverrides = null;
    [SerializeField] private ViewScript m_ViewTileScript = null;
    [SerializeField] private UnitInfoDisplay m_UnitInfoDisplay_Prefab = null;

    // Non-Serialized Variable(s)
    private GameEventNames m_GameEventNames = null;
    private TileSystem m_TileSystem = null;
    private UnitsTracker m_UnitsTracker = null;
    private UnitInfoDisplay m_UnitInfoDisplay = null;
    private IUnitAction m_CurrentActiveAction;
    private List<UnitStats> m_EnemiesInViewRange = new List<UnitStats>();

    public GameSystemsDirectory GetGameSystemsDirectory() { return GameSystemsDirectory.GetSceneInstance(); }

    public string UnitName
    {
        get { return m_UnitStatsJSON.m_UnitName; }
    }

    public string UnitDescription
    {
        get { return m_UnitStatsJSON.m_UnitDescription; }
    }

    public FactionType UnitFaction
    {
        get { return m_UnitStatsJSON.m_FactionType; }
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
        get { return m_UnitStatsJSON.m_CurrentHealthPoints; }
        set
        {
            if (m_UnitStatsJSON.m_CurrentHealthPoints == value) { return; }

            m_UnitStatsJSON.m_CurrentHealthPoints = Mathf.Clamp(value, 0, m_UnitStatsJSON.m_MaxHealthPoints);

            // Send the event that this unit is dead!
            if (m_UnitStatsJSON.m_CurrentHealthPoints <= 0)
            {
                // Trigger an event when the unit died
                // If the unit is visible, then start the death cinematic!
                GameEventSystem.GetInstance().TriggerEvent<UnitStats, bool>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitDead), this, m_ViewTileScript.IsVisible());
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
        get
        {
            return m_UnitStatsJSON.m_ConcealmentPoints;
        }
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
                Tile currentTile = m_TileSystem.GetTile(m_UnitStatsJSON.m_CurrentTileID);
                Assert.IsNotNull(currentTile);
                currentTile.Unit = null;

                // m_TileSystem.GetTile(m_UnitStatsJSON.m_CurrentTileID).Unit = null;
                m_UnitStatsJSON.m_CurrentTileID = value;
                Tile newTile = m_TileSystem.GetTile(m_UnitStatsJSON.m_CurrentTileID);
                Assert.IsNotNull(newTile);
                newTile.Unit = gameObject;
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

    public TileAttributeOverride[] GetTileAttributeOverrides() { return m_TileAttributeOverrides; }

    public List<UnitStats> GetEnemiesInViewRange() { return m_EnemiesInViewRange; }

    public ViewScript GetViewScript() { return m_ViewTileScript; }

    public UnitInfoDisplay GetUnitInfoDisplay() { return m_UnitInfoDisplay; }

    /// <summary>
    /// TODO: expand these upon
    /// (Terry: Naisu Engalishu, very the descriptive, 最佳 commment, Sek Heng.)
    /// </summary>
    public void ResetUnitStats()
    {
        ResetHealthPoints();
        ResetActionPoints();
    }

    private void InitEvents()
    {
        GameEventSystem.GetInstance().SubscribeToEvent(m_GameEventNames.GetEventName(GameEventNames.SpawnSystemNames.UnitsSpawned), OnUnitsSpawned);
        GameEventSystem.GetInstance().SubscribeToEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitMovedToTile), OnUnitMovedToTile);
        GameEventSystem.GetInstance().SubscribeToEvent<UnitStats, bool>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitDead), OnUnitDead);
    }

    private void DeinitEvents()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent(m_GameEventNames.GetEventName(GameEventNames.SpawnSystemNames.UnitsSpawned), OnUnitsSpawned);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitMovedToTile), OnUnitMovedToTile);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<UnitStats, bool>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitDead), OnUnitDead);
    }

    private void Awake()
    {
        // Ensure that we have the system(s) we require.
        m_GameEventNames = GameSystemsDirectory.GetSceneInstance().GetGameEventNames();
        Assert.IsTrue(m_GameEventNames != null, MethodBase.GetCurrentMethod().Name + " - m_GameEventNames must not be null!");
        m_TileSystem = GameSystemsDirectory.GetSceneInstance().GetTileSystem();
        Assert.IsTrue(m_TileSystem != null, MethodBase.GetCurrentMethod().Name + " - m_TileSystem must not be null!");
        m_UnitsTracker = GameSystemsDirectory.GetSceneInstance().GetUnitsTracker();
        Assert.IsTrue(m_UnitsTracker != null, MethodBase.GetCurrentMethod().Name + " - m_UnitsTracker must not be null!");

        // Ensure that we have the variable(s) we require.
        Assert.IsTrue(m_ViewTileScript != null);
        Assert.IsTrue(m_UnitStatsJSON.m_UnitName != null);

        // Ensure that we have the prefab(s) we require.
        Assert.IsTrue(m_UnitInfoDisplay_Prefab != null, MethodBase.GetCurrentMethod().Name + " - m_UnitInfoDisplay_Prefab must not be null!");

        // Initialisation of Unit Info Display
        m_UnitInfoDisplay = Instantiate(m_UnitInfoDisplay_Prefab.gameObject, GameSystemsDirectory.GetSceneInstance().GetUnclickableScreenSpaceCanvas().transform).GetComponent<UnitInfoDisplay>();
        m_UnitInfoDisplay.SetUnitStats(this);
        m_UnitInfoDisplay.transform.position = transform.position;

        // Subscribe to events.
        InitEvents();
    }

    private void OnDestroy()
    {
        // Unsubscribe From events.
        DeinitEvents();
    }

    // Event Callbacks
    private void OnUnitsSpawned()
    {
        CheckEnemiesInViewRange();
        UpdateUnitInfoDisplay();

        m_TileSystem.GetTile(CurrentTileID).Unit = gameObject;
    }

    private void OnUnitMovedToTile(UnitStats _movedUnit)
    {
        if (!IsAlive()) { return; }
        CheckEnemyInViewRange(_movedUnit);
    }

    private void OnUnitDead(UnitStats _deadUnit, bool _isUnitVisible)
    {
        EnemyInRangeDead(_deadUnit, _isUnitVisible);

        if (_deadUnit == this)
        {
            Destroy(m_UnitInfoDisplay.gameObject);
        }
    }

    /// <summary>
    /// Check whether is it in range.
    /// Should be called every time a unit moves from it's tile!
    /// However, it will check all of the opposition units!
    /// </summary>
    public void CheckEnemiesInViewRange()
    {
        List<UnitStats> aliveEnemies = new List<UnitStats>();
        for (int i = 0; i < (int)FactionType.Num_FactionType; ++i)
        {
            if ((FactionType)i == UnitFaction) { continue; }

            UnitStats[] aliveUnits = m_UnitsTracker.GetAliveUnits((FactionType)i);
            for (int j = 0; j < aliveUnits.Length; ++j)
            {
                aliveEnemies.Add(aliveUnits[j]);
            }
        }

        foreach (UnitStats unitStats in aliveEnemies)
        {
            Assert.IsTrue(unitStats.IsAlive(), MethodBase.GetCurrentMethod().Name + " - There should not be dead units!");

            Tile enemyTile = m_TileSystem.GetTile(unitStats.CurrentTileID);
            int tileDistance = TileId.GetDistance(CurrentTileID, unitStats.CurrentTileID) + unitStats.ConcealmentPoints + enemyTile.GetTotalConcealmentPoints();
            // if that list does not have the unit in range!
            if (!m_EnemiesInViewRange.Contains(unitStats))
            {
                if (tileDistance <= ViewRange && m_ViewTileScript.RaycastToTile(m_TileSystem.GetTile(unitStats.CurrentTileID)))
                {
                    m_EnemiesInViewRange.Add(unitStats);
                    unitStats.m_ViewTileScript.IncreaseVisibility();
                }
            }
            else
            {
                if (tileDistance > ViewRange || !m_ViewTileScript.RaycastToTile(m_TileSystem.GetTile(unitStats.CurrentTileID)))
                {
                    // if the opposing unit is in range and 
                    m_EnemiesInViewRange.Remove(unitStats);
                    unitStats.m_ViewTileScript.DecreaseVisibility();
                }
            }
        }
    }

    private void UpdateUnitInfoDisplay()
    {
        if (m_UnitInfoDisplay)
        {
            // Update Health
            m_UnitInfoDisplay.GetHealthBar().MaxHealthPoints = MaxHealthPoints;
            m_UnitInfoDisplay.GetHealthBar().CurrentHealthPoints = CurrentHealthPoints;

            // Update Action Points
            m_UnitInfoDisplay.GetActionPointsCounter().MaxActionPoints = MaxActionPoints;
            m_UnitInfoDisplay.GetActionPointsCounter().CurrentActionPoints = CurrentActionPoints;
        }
    }

    /// <summary>
    /// A function call to be passed to when a unit has moved!
    /// </summary>
    /// <param name="_movedUnit"></param>
    private void CheckEnemyInViewRange(UnitStats _movedUnit)
    {
        if (_movedUnit == this)
        {
            CheckEnemiesInViewRange();
            return;
        }

        if (_movedUnit.UnitFaction == UnitFaction) { return; }

        int tileDistance = TileId.GetDistance(CurrentTileID, _movedUnit.CurrentTileID) + _movedUnit.ConcealmentPoints;
        if (m_EnemiesInViewRange.Contains(_movedUnit))
        {
            if (tileDistance > ViewRange || !m_ViewTileScript.RaycastToTile(m_TileSystem.GetTile(_movedUnit.CurrentTileID)))
            {
                m_EnemiesInViewRange.Remove(_movedUnit);
                _movedUnit.m_ViewTileScript.DecreaseVisibility();
            }
        }
        else
        {
            if (tileDistance <= ViewRange && m_ViewTileScript.RaycastToTile(m_TileSystem.GetTile(_movedUnit.CurrentTileID)))
            {
                m_EnemiesInViewRange.Add(_movedUnit);
                _movedUnit.m_ViewTileScript.IncreaseVisibility();
            }
        }
    }
    /// <summary>
    /// To be called when the opposing unit died
    /// </summary>
    private void EnemyInRangeDead(UnitStats _deadUnit, bool _destroyedUnitVisible)
    {
        // if the dead unit is itself, iterate through it's list and decrease the visibility of other units!
        if (_deadUnit == this)
        {
            foreach (UnitStats enemy in m_EnemiesInViewRange)
            {
                ViewScript enemyViewScript = enemy.gameObject.GetComponent<ViewScript>();
                enemyViewScript.DecreaseVisibility();
            }
            m_EnemiesInViewRange.Clear();
        }
        else
        {
            if (m_EnemiesInViewRange.Contains(_deadUnit))
            {
                m_EnemiesInViewRange.Remove(_deadUnit);
                _deadUnit.GetComponent<ViewScript>().DecreaseVisibility();
            }
        }
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
            if (overrideTypeValidator.Contains(m_TileAttributeOverrides[i].GetTileType()))
            {
                string message = "More than 1 TileAttributeOverride has the same tile type!";
                EditorUtility.DisplayDialog("Duplicate TileType!", message, "OK");
                Debug.Log(message);
            }
            else
            {
                overrideTypeValidator.Add(m_TileAttributeOverrides[i].GetTileType());
            }
        }
    }

    private void OnValidate()
    {
        ValidateTileOverrides();
    }
#endif // UNITY_EDITOR
}