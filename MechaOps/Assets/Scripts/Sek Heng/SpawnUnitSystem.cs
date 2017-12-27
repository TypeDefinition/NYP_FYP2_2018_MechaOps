using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// The system that handles the spawning of units
/// </summary>
public class SpawnUnitSystem : MonoBehaviour {
    [Header("Variables for SpawnUnitSystem")]
    [SerializeField, Tooltip("GameSystemDirectory")]
    protected GameSystemsDirectory m_GameSystem;
    [SerializeField, Tooltip("Data container for the asset data")]
    protected UnitDataAndCost m_UnitsDataAsset;
    [SerializeField, Tooltip("The UI prefab to be instantiated")]
    protected GameObject m_SelectUnitUIPrefab;
    [SerializeField, Tooltip("Amount of credit for the player to spend")]
    protected int m_PlayerCredits;
    [SerializeField, Tooltip("How many unit capacity it needs")]
    protected int m_CapacityOfUnits = 8;

    [Header("Debugging for SpawnUnitSystem")]
    [SerializeField, Tooltip("Reference to the instantiated prefab")]
    protected GameObject m_InstantiateSelectUnitUI;
    [SerializeField, Tooltip("The array of the of spawn indicators depending on the m_Capacity Of Units")]
    protected UnitSlotUI[] m_ArrayOfUnitSpawn;

    public int PlayerCredits
    {
        set
        {
            m_PlayerCredits = value;
            Assert.IsTrue(m_PlayerCredits >= 0, "Player Credits is less than 0 at SpawnUnitSystem!");
        }
        get
        {
            return m_PlayerCredits;
        }
    }

    // Use this for initialization
    void Start () {
        m_InstantiateSelectUnitUI = Instantiate(m_SelectUnitUIPrefab, m_GameSystem.GetScreenSpaceCanvas().transform);
        // Need to make sure it will always be active
        m_InstantiateSelectUnitUI.SetActive(true);
        SpawnUnitUI_Logic zeSpawnUI_Logic = m_InstantiateSelectUnitUI.GetComponent<SpawnUnitUI_Logic>();
        // making sure the array is the same amount as the capacity
        m_ArrayOfUnitSpawn = new UnitSlotUI[m_CapacityOfUnits];
        #region Spawning of Array of spawned Units
        for (int num = 0; num < m_CapacityOfUnits; ++num)
        {
            GameObject zeInstantiateUnitSlot = Instantiate(zeSpawnUI_Logic.SlotUI.gameObject, zeSpawnUI_Logic.UnitLayoutUI.transform, false);
            UnitSlotUI zeSlotUI = zeInstantiateUnitSlot.GetComponent<UnitSlotUI>();
            m_ArrayOfUnitSpawn[num] = zeSlotUI;
            zeInstantiateUnitSlot.SetActive(true);
        }
        #endregion
        // Make sure the finished button has the startspawning units function
        zeSpawnUI_Logic.ArrayOfSpawnUI = m_ArrayOfUnitSpawn;
        zeSpawnUI_Logic.PlayerCreditText = m_PlayerCredits.ToString();
        zeSpawnUI_Logic.SpawnSystem = this;
    }

    /// <summary>
    /// When the player pressed the finish button
    /// </summary>
    void StartSpawningUnits()
    {
        //foreach (SpawnIndicator_Logic zeSpawnData in m_ListOfSpawnUnits)
        //{
        //    // spawn / instantiate the unit accordingly
        //    GameObject zeUnitGO = Instantiate(m_UnitsDataAsset.GetUnitGO(zeSpawnData.TypeNameTextString));
        //    // then set the unit's stats accordingly
        //    UnitStats zeUnitStat = zeUnitGO.GetComponent<UnitStats>();
        //    zeUnitStat.CurrentTileID = zeSpawnData.TileID;
        //    zeUnitGO.transform.position = new Vector3(zeSpawnData.transform.position.x, zeUnitGO.transform.position.y, zeSpawnData.transform.position.z);
        //    Destroy(zeSpawnData.gameObject);
        //}
        // and then set the gamesystem to be active
        m_GameSystem.gameObject.SetActive(true);
        // destroy the gameobject that this is attached to when it is done
        Destroy(gameObject);
    }

    /// <summary>
    /// When this component is destroyed, destroy the UI too!
    /// </summary>
    private void OnDestroy()
    {
        Destroy(m_InstantiateSelectUnitUI);
    }
}
