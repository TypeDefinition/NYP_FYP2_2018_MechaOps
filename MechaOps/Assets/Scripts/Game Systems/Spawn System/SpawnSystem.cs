using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class SpawnSystem : MonoBehaviour
{
    // Serialised Variable(s)
    [SerializeField] private UnitLibrary m_UnitLibrary = null;
    [SerializeField] private UnitsToSpawn[] m_UnitsToSpawn = null;

    // Non-Serialised Variable(s)
    private TileSystem m_TileSystem = null;
    private UnitsTracker m_UnitsTracker = null;
    private GameEventNames m_GameEventNames = null;

    private void Awake()
    {
        m_TileSystem = GameSystemsDirectory.GetSceneInstance().GetTileSystem();
        m_UnitsTracker = GameSystemsDirectory.GetSceneInstance().GetUnitsTracker();
        m_GameEventNames = GameSystemsDirectory.GetSceneInstance().GetGameEventNames();
    }

    private void Start()
    {
        SpawnUnits();
    }

    private void SpawnUnits()
    {
        SpawnTiles spawnTiles = m_TileSystem.GetSpawnTiles();

        for (int i = 0; i < m_UnitsToSpawn.Length; ++i)
        {
            FactionType factionType = m_UnitsToSpawn[i].GetFactionType();
            UnitType[] unitList = m_UnitsToSpawn[i].GetUnitList();

            // Skip this iteration if there no units from this faction to spawn.
            if (unitList.Length == 0) { continue; }

            TileId[] tileList = spawnTiles.GetFactionTileList(factionType).GetList();
            Assert.IsTrue(tileList.Length >= unitList.Length, MethodBase.GetCurrentMethod().Name + " - There are more units to spawn then there are spawn tiles!");

            List<UnitStats> spawnedUnits = new List<UnitStats>();
            for (int j = 0; j < unitList.Length; ++j)
            {
                // Spawn the unit.
                UnitLibrary.UnitLibraryData libraryData = m_UnitLibrary.GetUnitLibraryData(unitList[j]);
                UnitStats unitPrefab = libraryData.GetUnitPrefab();
                UnitStats spawnedUnit = Instantiate(unitPrefab);

                Assert.IsTrue(spawnedUnit.UnitFaction == m_UnitsToSpawn[i].GetFactionType(), MethodBase.GetCurrentMethod().Name + " - A Unit is spawned in the wrong faction!");

                // Set the spawned unit's TileId.
                Tile spawnTile = m_TileSystem.GetTile(tileList[j]);
                Assert.IsNotNull(spawnTile, MethodBase.GetCurrentMethod().Name + " - Spawn Tile not found!");
                // Set the spawned unit's position to the spawn tile's position.
                spawnedUnit.gameObject.transform.position = spawnTile.transform.position;
                spawnedUnit.CurrentTileID = tileList[j];

                // Add the spawned units to spawnedUnits.
                spawnedUnits.Add(spawnedUnit);
            }

            m_UnitsTracker.SetUnits(factionType, spawnedUnits.ToArray());
        }

        GameEventSystem.GetInstance().TriggerEvent(m_GameEventNames.GetEventName(GameEventNames.SpawnSystemNames.UnitsSpawned));
    }
}