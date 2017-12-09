using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public delegate void Void_UnitStat(UnitStats _unitStat);

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
    protected List<GameObject> m_EnemyInRange = new List<GameObject>();
    [SerializeField, Tooltip("The array of tiles override")]
    private TileAttributeOverride[] m_TileAttributeOverrides;
    [SerializeField, Tooltip("The list of animation handler which will be accessed through dictionary. No linking required")]
    protected MOAnimation[] m_AnimHandler;
    [Tooltip("The tile system!")]
    public TileSystem m_TileSys;
    [Tooltip("Death attack animation"), SerializeField]
    protected MOAnimation_UnitDestroy m_DeathAnim;
    [SerializeField, Tooltip("View Render Script")]
    protected ViewTileScript m_ViewTileScript;

    /// <summary>
    /// A callback function will appear when ever the health point decreases
    /// </summary>
    public Void_UnitStat m_HealthDropCallback;

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
                string zeEventName = tag + "IsDead";
                //// Trigger an event when the unit died
                GameEventSystem.GetInstance().TriggerEvent<GameObject>(zeEventName, gameObject);
                m_DeathAnim.StartAnimation();
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
        set {
            // we will need to make sure that the value is not the same!
            if (!value.Equals(m_UnitStatsJSON.m_CurrentTileID))
            {
                // and then set the previous tile to have no units since it has moved towards a new tile
                m_TileSys.GetTile(m_UnitStatsJSON.m_CurrentTileID).Unit = null;
                m_UnitStatsJSON.m_CurrentTileID = value;
                m_TileSys.GetTile(value).Unit = gameObject;
            }
        }
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

    public List<GameObject> EnemyInRange
    {
        get
        {
            return m_EnemyInRange;
        }
    }

    protected IEnumerator Start()
    {
        // Assign the gameobject name to the unit if there is none for the unit stat!
        if (m_UnitStatsJSON.m_Name == null)
        {
            m_UnitStatsJSON.m_Name = name;
        }
        yield return null;
        // check if there is any enemy in range when the game is starting!
        CheckEnemyInRange();
        // And then we will need to register our tile top the tile system so that trespassing will happen!
        if (!m_TileSys)
            m_TileSys = FindObjectOfType<TileSystem>();
        m_TileSys.GetTile(CurrentTileID).Unit = gameObject;
        if (tag == "Player" && !m_ViewTileScript)
            m_ViewTileScript = GetComponent<PlayerViewScript>();
        yield break;
    }

    protected void OnEnable()
    {
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("UnitMoveToTile", CheckEnemyInRange);
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

    protected void OnDisable()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("UnitMoveToTile", CheckEnemyInRange);
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
    public void ResetUnitStat()
    {
        ResetHealthPoints();
        ResetActionPoints();
    }

    /// <summary>
    /// Check whether is it in range.
    /// Should be called every time a unit moves from it's tile!
    /// However, it will check all of the opposition units!
    /// </summary>
    public void CheckEnemyInRange()
    {
        List<GameObject> zeListOfUnits = null;
        // TODO: Remove this hardcoding when it is done!
        switch (tag)
        {
            case "Player":
                // so get the list of enemy units
                m_ViewTileScript.RenderSurroundingTiles();
                zeListOfUnits = KeepTrackOfUnits.Instance.m_AllEnemyUnitGO;
                break;
            case "EnemyUnit":
                zeListOfUnits = KeepTrackOfUnits.Instance.m_AllPlayerUnitGO;
                break;
            default:
                Assert.IsTrue(false, "Make CheckEnemyInRange more robust so that there can be more factions!");
                break;
        }
        foreach (GameObject zeGO in zeListOfUnits)
        {
            UnitStats zeGOState = zeGO.GetComponent<UnitStats>();
            switch (zeGOState.IsAlive())
            {
                case false:
                    continue;
                default:
                    break;
            }
            int zeTileDist = TileId.GetDistance(CurrentTileID, zeGOState.CurrentTileID) + zeGOState.ConcealmentPoints;
            // if that list does not have the unit in range!
            if (!m_EnemyInRange.Contains(zeGO))
            {
                if (zeTileDist <= ViewRange)
                    m_EnemyInRange.Add(zeGO);
            }
            else
            {
                if (zeTileDist > ViewRange)
                {
                    // if the opposing unit is in range and 
                    m_EnemyInRange.Remove(zeGO);
                }
            }
        }
    }

    /// <summary>
    /// A function call to be passed to when a unit has moved!
    /// </summary>
    /// <param name="_movedUnit"></param>
    public void CheckEnemyInRange(GameObject _movedUnit)
    {
        if (!CompareTag(_movedUnit.tag))
        {
            if (_movedUnit != gameObject)
            {
                UnitStats zeGOState = _movedUnit.GetComponent<UnitStats>();
                int zeTileDist = TileId.GetDistance(CurrentTileID, zeGOState.CurrentTileID) + zeGOState.ConcealmentPoints;
                // Need to make sure that the moveunit is not itself! and the tag is the opposing unit!
                if (!m_EnemyInRange.Contains(_movedUnit))
                {
                    if (zeTileDist <= ViewRange)
                    {
                        m_EnemyInRange.Add(_movedUnit);
                        // so we have to render the enemy unit if it belongs to the enemy
                        zeGOState.m_ViewTileScript.IncreVisi();
                    }
                }
                else
                {
                    if (zeTileDist > ViewRange)
                    {
                        // if the opposing unit is in range and 
                        m_EnemyInRange.Remove(_movedUnit);
                        zeGOState.m_ViewTileScript.DecreVisi();
                    }
                }
            }
        }
        else
        {
            // if u are the 1 moving, check for nearby enemies
            if (gameObject == _movedUnit)
            {
                CheckEnemyInRange();
            }
        }
    }
    /// <summary>
    /// To be called when the opposing unit died
    /// </summary>
    protected void EnemyInRangeDead(GameObject _deadUnit)
    {
        m_EnemyInRange.Remove(_deadUnit);
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