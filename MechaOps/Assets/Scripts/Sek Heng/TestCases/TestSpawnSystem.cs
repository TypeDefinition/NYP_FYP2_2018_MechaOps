using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawnSystem : SpawnSystem {

	// Use this for initialization
	void Start () {
        // need to get all of the player and enemies stuff
        GameObject[] ArrayOfPlayerUnits = GameObject.FindGameObjectsWithTag("Player");
        List<UnitStats> ListOfPlayerUnitStats = new List<UnitStats>();
        foreach (GameObject playerUnitGO in ArrayOfPlayerUnits)
        {
            UnitStats playerUnitStat = playerUnitGO.GetComponent<UnitStats>();
            ListOfPlayerUnitStats.Add(playerUnitStat);
        }
        m_UnitsTracker.SetUnits(ListOfPlayerUnitStats[0].UnitFaction, ListOfPlayerUnitStats.ToArray());

        GameObject[] ArrayOfEnemyUnits = GameObject.FindGameObjectsWithTag("EnemyUnit");
        List<UnitStats> ListOfEnemyUnitStats = new List<UnitStats>();
        foreach (GameObject enemyUnitGO in ArrayOfEnemyUnits)
        {
            ListOfEnemyUnitStats.Add(enemyUnitGO.GetComponent<UnitStats>());
        }
        m_UnitsTracker.SetUnits(ListOfEnemyUnitStats[0].UnitFaction, ListOfEnemyUnitStats.ToArray());
        GameEventSystem.GetInstance().TriggerEvent(m_GameEventNames.GetEventName(GameEventNames.SpawnSystemNames.UnitsSpawned));
    }
}
