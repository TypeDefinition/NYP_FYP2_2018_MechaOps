using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The system that handles the spawning of units
/// </summary>
public class SpawnUnitSystem : MonoBehaviour {
    /// <summary>
    /// To record down where will the units be spawning
    /// </summary>
    [System.Serializable]
    public struct UnitSpawnData
    {
        public TileId m_UnitSpawnArea;
        public string m_TypeName;
    }
    [Header("Variables for SpawnUnitSystem")]
    [SerializeField, Tooltip("GameSystemDirectory")]
    protected GameSystemsDirectory m_GameSystem;
    [SerializeField, Tooltip("Data container for the asset data")]
    protected UnitDataAndCost m_UnitsDataAsset;
    [SerializeField, Tooltip("The UI prefab to be instantiated")]
    protected GameObject m_SelectUnitUIPrefab;
    [SerializeField, Tooltip("Button prefab for selecting the unit")]
    protected Button m_SelectUnitButtonPrefab;
    [SerializeField, Tooltip("Amount of credit for the player to spend")]
    protected int m_PlayerCredits;

    [Header("Debugging for SpawnUnitSystem")]
    [SerializeField, Tooltip("Name of the selected unit from the UI")]
    protected string m_CurrentSelectedUnitName;
    [SerializeField, Tooltip("Reference to the instantiated prefab")]
    protected GameObject m_InstantiateSelectUnitUI;
    [SerializeField, Tooltip("List of player units that will be spawn later")]
    protected List<UnitSpawnData> m_ListOfSpawnUnits = new List<UnitSpawnData>();

	// Use this for initialization
	void Start () {
		
	}

    void SetUnitPosition(GameObject _go)
    {

    }

    void SetUnitToSpawn(string _UnitTypeName)
    {

    }

    /// <summary>
    /// When the player pressed the finish button
    /// </summary>
    void StartSpawningUnits()
    {
        // destroy the gameobject that this is attached to when it is done
        Destroy(gameObject);
    }
}
