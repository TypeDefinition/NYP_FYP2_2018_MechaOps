#define GOAP_AI
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Mainly to iterate and update all of the enemy units 1 by 1
/// </summary>
public class EnemyUnitsManager : MonoBehaviour
{
    [SerializeField] private GameSystemsDirectory m_GameSystemsDirectory = null;
    private TileSystem m_TileSystem = null;
    private UnitsTracker m_UnitsTracker = null;

    [Header("Shown in the Inspector for debugging purposes.")]

    [SerializeField, Tooltip("The list of available units when it begins")]
    private List<GameObject> m_EnemyList;
    [SerializeField, Tooltip("Tile marker where all of the player units last gathered")]
    private TileId m_PlayerUnitsLocations; // This isn't an array?

    /// <summary>
    /// The Update of this manager
    /// </summary>
    Coroutine m_UpdateOfManager;
    /// <summary>
    /// The individual update of the unit
    /// </summary>
    Coroutine m_UpdateOfEnemy;

    public TileId PlayerUnitLocations
    {
        get { return m_PlayerUnitsLocations; }
    }

    private void Awake()
    {
        Assert.IsFalse(m_GameSystemsDirectory == null, MethodBase.GetCurrentMethod().Name + " - m_GameSystemsDirectory must not be null!");

        m_TileSystem = m_GameSystemsDirectory.GetTileSystem();
        Assert.IsFalse(m_TileSystem == null, MethodBase.GetCurrentMethod().Name + " - TileSystem not found in m_GameSystemsDirectory.");
        m_UnitsTracker = m_GameSystemsDirectory.GetUnitsTracker();
        Assert.IsFalse(m_UnitsTracker == null, MethodBase.GetCurrentMethod().Name + " - UnitsTracker not found in m_GameSystemsDirectory.");
    }

    private void InitEvents()
    {
        GameEventSystem.GetInstance().SubscribeToEvent("PlayerAnnihilated", StopUpdate);
        GameEventSystem.GetInstance().SubscribeToEvent("EnemyAnnihilated", StopUpdate);
    }

    private void DeinitEvents()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent("PlayerAnnihilated", StopUpdate);
        GameEventSystem.GetInstance().UnsubscribeFromEvent("EnemyAnnihilated", StopUpdate);
    }

    private void OnEnable()
    {
        InitEvents();
    }

    private void OnDisable()
    {
        DeinitEvents();
    }

    /// <summary>
    /// The coroutine to update the manager!
    /// </summary>
    /// <returns></returns>
    public IEnumerator IterateThroughEnemyUpdate()
    {
       UpdateMarker();
        m_EnemyList = new List<GameObject>(m_UnitsTracker.m_AliveEnemyUnits);
#if GOAP_AI
        foreach (GameObject enemy in m_EnemyList)
        {
            GoapPlanner zePlanner = enemy.GetComponent<GoapPlanner>();
            m_UpdateOfEnemy = StartCoroutine(zePlanner.StartPlanning());
            // wait till the update is finish then proceed to the next unit
            yield return m_UpdateOfEnemy;
        }
#else
#endif
        m_UpdateOfManager = null;
        // reset all of alive enemy unit's action points!
        foreach (GameObject zeEnemyGO in m_UnitsTracker.m_AliveEnemyUnits)
        {
            zeEnemyGO.GetComponent<UnitStats>().ResetActionPoints();
        }

        GameEventSystem.GetInstance().TriggerEvent("TurnEnded");
        print("Finish Enemy Manager turn");
        yield break;
    }

    protected void StopUpdate()
    {
        if (m_UpdateOfManager != null)
        {
            StopCoroutine(m_UpdateOfManager);
            m_UpdateOfManager = null;
        }
        if (m_UpdateOfEnemy != null)
        {
            StopCoroutine(m_UpdateOfEnemy);
            m_UpdateOfEnemy = null;
        }
    }

    /// <summary>
    /// Meant to re-update the position of the player units gathered if the enemy unit has reached the current marker
    /// </summary>
    public void UpdateMarker()
    {
        // TODO: improve this function!
        m_PlayerUnitsLocations = m_UnitsTracker.m_AlivePlayerUnits[0].GetComponent<UnitStats>().CurrentTileID;
        Tile zeTile = m_TileSystem.GetTile(m_PlayerUnitsLocations);
        if (!zeTile.GetIsWalkable() || zeTile.HasUnit())
        {
            // we only need the 1 radius tile!
            TileId []zeTiles = m_TileSystem.GetSurroundingTiles(m_PlayerUnitsLocations, 1);
            while (!zeTile.GetIsWalkable() || zeTile.HasUnit())
            {
                foreach (TileId zeTileID in zeTiles)
                {
                    m_PlayerUnitsLocations = zeTileID;
                    zeTile = m_TileSystem.GetTile(m_PlayerUnitsLocations);
                    if (zeTile.GetIsWalkable() && !zeTile.HasUnit())
                    {
                        break;
                    }
                }
            }
        }
    }
}