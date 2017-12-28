using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// The system that handles the spawning of units
/// </summary>
public class SpawnUnitSystem : MonoBehaviour {
    [Header("Variables for SpawnUnitSystem")]
    [SerializeField, Tooltip("Data container for the asset data")]
    protected UnitDataAndCost m_UnitsDataAsset;
    [SerializeField, Tooltip("Amount of credit for the player to spend")]
    protected int m_PlayerCredits;
    [SerializeField, Tooltip("How many unit capacity it needs")]
    protected int m_CapacityOfUnits = 8;
    [SerializeField, Tooltip("Select Unit UI GameObject")]
    protected GameObject m_SelectUnitUIGO;

    [Header("Debugging for SpawnUnitSystem")]
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

    private void Start()
    {
        StartSpawnLogic();
    }

    // Use this for initialization
    public void StartSpawnLogic() {
        // Need to make sure that the array in UnitAsset is the same
        if (m_UnitsDataAsset.SpawnDataList.Length != m_CapacityOfUnits)
        {
            m_UnitsDataAsset.SpawnDataList = new UnitDataAndCost.UnitsPrefabData[m_CapacityOfUnits];
        }
        // Need to make sure it will always be active
        m_SelectUnitUIGO.SetActive(true);
        SpawnUnitUI_Logic zeSpawnUI_Logic = m_SelectUnitUIGO.GetComponent<SpawnUnitUI_Logic>();
        // making sure the array is the same amount as the capacity
        m_ArrayOfUnitSpawn = new UnitSlotUI[m_CapacityOfUnits];
        #region Spawning of Array of spawned Units
        for (int num = 0; num < m_CapacityOfUnits; ++num)
        {
            GameObject zeInstantiateUnitSlot = Instantiate(zeSpawnUI_Logic.SlotUI.gameObject, zeSpawnUI_Logic.UnitLayoutUI.transform, false);
            UnitSlotUI zeSlotUI = zeInstantiateUnitSlot.GetComponent<UnitSlotUI>();
            m_ArrayOfUnitSpawn[num] = zeSlotUI;
            zeInstantiateUnitSlot.SetActive(true);
            // and we will need to see if the unit stat inside the UnitDataAndCost also contains the same thing!
            if (m_UnitsDataAsset.SpawnDataList[num].m_UnitStatsPrefab)
            {
                zeSlotUI.UnitData = m_UnitsDataAsset.GetUnitIconSprite(m_UnitsDataAsset.SpawnDataList[num].m_UnitStatsPrefab.Name);
                // deduct the current credit too!
                PlayerCredits -= zeSlotUI.UnitData.m_UnitPrefabDataReference.Cost;
            }
        }
        #endregion
        // Make sure the finished button has the start spawning units function
        zeSpawnUI_Logic.ArrayOfSpawnUI = m_ArrayOfUnitSpawn;
        zeSpawnUI_Logic.PlayerCreditText = m_PlayerCredits.ToString();
        zeSpawnUI_Logic.SpawnSystem = this;
        zeSpawnUI_Logic.FinishedButton.onClick.AddListener(StartSpawningUnits);
    }

    /// <summary>
    /// When the player pressed the finish button
    /// </summary>
    void StartSpawningUnits()
    {
        for (int num = 0; num < m_ArrayOfUnitSpawn.Length; ++num)
        {
            m_UnitsDataAsset.SpawnDataList[num] = m_ArrayOfUnitSpawn[num].UnitData.m_UnitPrefabDataReference;
        }
    }
}
