using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent]
public class GameSystemsDirectory : MonoBehaviour
{
    // Ensure that there is only ever 1 GameSystemsDirectory in a scene.
    static private GameSystemsDirectory m_SceneInstance = null;
    private bool m_Destroyed = false;

    // This only returns the scene instance. If it does not already exist, it will not be created.
    static public GameSystemsDirectory GetSceneInstance()
    {
        // If GetSceneInstance() is called during Awake(), m_SceneInstance might not have been set yet.
        // In that case, find the GameSystemsDirectory in the scene.
        if (m_SceneInstance == null)
        {
            GameSystemsDirectory[] gameSystemDirectories = FindObjectsOfType<GameSystemsDirectory>();
            if (gameSystemDirectories == null || gameSystemDirectories.Length == 0) { return null; }

            // Ensure that there is not more than 1 GameSystemsDirectory that is not destroyed.
            for (int i = 0; i < gameSystemDirectories.Length; ++i)
            {
                if (gameSystemDirectories[i].m_Destroyed) { continue; }
                if (m_SceneInstance == null)
                {
                    m_SceneInstance = gameSystemDirectories[i];
                }
                else
                {
                    Assert.IsTrue(false, MethodBase.GetCurrentMethod().Name + " - There can be no more than 1 GameSystemDirectory per scene!");
                }
            }
        }

        return m_SceneInstance;
    }

    // Destroy this if it another instance already exists in the scene.
    private void Awake()
    {
        if (null == m_SceneInstance)
        {
            m_SceneInstance = this;
        }
        else if (this != m_SceneInstance)
        {
            Assert.IsTrue(false, MethodBase.GetCurrentMethod().Name + " - There can only be 1 GameSystemsDirectory per scene!");
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        m_Destroyed = true;
        if (this == m_SceneInstance) { m_SceneInstance = null; }
    }

    [Header("Systems that are NOT a child of this GameObject.")]
    [SerializeField] private TileSystem m_TileSystem = null;
    [SerializeField] private Camera m_GameCamera = null;

    [Header("Systems that are a child of this GameObject.")]
    [SerializeField] private UnitsTracker m_UnitsTracker = null;
    [SerializeField] private AIUnitsManager[] m_AIUnitManagers = null;
    [SerializeField] private PlayerUnitsManager m_PlayerUnitsManager = null;
    [SerializeField] private UnitActionScheduler m_UnitActionScheduler = null;
    [SerializeField] private CineMachineHandler m_CineMachineHandler = null;
    [SerializeField] private SpawnSystem m_SpawnSystem = null;
    [SerializeField] private Canvas m_ScreenSpaceCanvas = null;

    [Header("Scriptable Objects.")]
    [SerializeField] private GameAudioSettings m_GameAudioSettings = null;
    [SerializeField] private GameEventNames m_GameEventNames = null;
    [SerializeField] private FactionNames m_FactionNames = null;

    public TileSystem GetTileSystem() { return m_TileSystem; }
    public Canvas GetScreenSpaceCanvas() { return m_ScreenSpaceCanvas; }
    public Camera GetGameCamera() { return m_GameCamera; }

    public UnitsTracker GetUnitsTracker() { return m_UnitsTracker; }
    public AIUnitsManager[] GetAIUnitsManager() { return m_AIUnitManagers; }
    public PlayerUnitsManager GetPlayerUnitsManager() { return m_PlayerUnitsManager; }
    public UnitActionScheduler GetUnitActionScheduler() { return m_UnitActionScheduler; }
    public CineMachineHandler GetCineMachineHandler() { return m_CineMachineHandler; }
    public SpawnSystem GetSpawnSystem() { return m_SpawnSystem; }

    public GameAudioSettings GetGameAudioSettings() { return m_GameAudioSettings; }
    public GameEventNames GetGameEventNames() { return m_GameEventNames; }
    public FactionNames GetFactionNames() { return m_FactionNames; }
}