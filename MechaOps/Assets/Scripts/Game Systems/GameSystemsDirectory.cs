using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameSystemsDirectory : MonoBehaviour
{
    [SerializeField] private TileSystem m_TileSystem = null;
    [SerializeField] private Canvas m_ScreenSpaceCanvas = null;
    [SerializeField] private Canvas m_WorldSpaceCanvas = null;
    [SerializeField] private UnitsTracker m_UnitsTracker = null;
    [SerializeField] private EnemyUnitsManager m_EnemyUnitsManager = null;
    [SerializeField] private PlayerUnitsManager m_PlayerUnitsManager = null;
    [SerializeField] private UnitActionScheduler m_UnitActionScheduler = null;
    [SerializeField] private CineMachineHandler m_CineMachineHandler = null;

    public TileSystem GetTileSystem() { return m_TileSystem; }
    public Canvas GetScreenSpaceCanvas() { return m_ScreenSpaceCanvas; }
    public Canvas GetWorldSpaceCanvas() { return m_WorldSpaceCanvas; }
    public UnitsTracker GetUnitsTracker() { return m_UnitsTracker; }
    public EnemyUnitsManager GetEnemyUnitsManager() { return m_EnemyUnitsManager; }
    public PlayerUnitsManager GetPlayerUnitsManager() { return m_PlayerUnitsManager; }
    public UnitActionScheduler GetUnitActionScheduler() { return m_UnitActionScheduler; }
    public CineMachineHandler GetCineMachineHandler() { return m_CineMachineHandler; }
}