using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// The container to contain the unit's prefab and the cost to spawn those units!
/// </summary>
[CreateAssetMenu(fileName = "UnitDataAndCost", menuName = "Units/SpawnData", order = 1)]
public class UnitDataAndCost : ScriptableObject {
    /// <summary>
    /// The data container for Units prefab
    /// </summary>
    [System.Serializable]
    public struct UnitsPrefabData
    {
        public UnitStats m_UnitStatsPrefab;
        public int m_Cost;

        public int Cost
        {
            get
            {
                return m_Cost;
            }
        }
        public UnitStats UnitStatsPrefab
        {
            get
            {
                return m_UnitStatsPrefab;
            }
        }
    }

    /// <summary>
    /// For the UI icons
    /// </summary>
    [System.Serializable]
    public struct UnitUI_Data
    {
        public Sprite m_UnitSpriteUI;
        public string m_TypeName;
    }

    [SerializeField, Tooltip("The array of unit data")]
    protected UnitsPrefabData[] m_ArrayOfUnitsData;
    [SerializeField, Tooltip("Array of enemy unit data")]
    protected UnitsPrefabData[] m_ArrayOfEnemyUnits;
    [SerializeField, Tooltip("Array of unit icon UI")]
    protected UnitUI_Data[] m_UnitUIDataArray;

    public UnitUI_Data[] UnitUIDataArray
    {
        get
        {
            return m_UnitUIDataArray;
        }
    }

    public GameObject GetUnitGO(string _name)
    {
        foreach (UnitsPrefabData zeUnitData in m_ArrayOfUnitsData)
        {
            if (zeUnitData.m_UnitStatsPrefab.Name == _name)
            {
                return zeUnitData.UnitStatsPrefab.gameObject;
            }
        }
        return null;
    }

    public GameObject GetEnemyUnitGO(string _name)
    {
        foreach (UnitsPrefabData zeUnitData in m_ArrayOfEnemyUnits)
        {
            if (zeUnitData.m_UnitStatsPrefab.Name == _name)
            {
                return zeUnitData.UnitStatsPrefab.gameObject;
            }
        }
        return null;
    }

    public int GetUnitCost(string _name)
    {
        int zeCost = -1;
        foreach (UnitsPrefabData zeUnitData in m_ArrayOfUnitsData)
        {
            if (zeUnitData.m_UnitStatsPrefab.Name == _name)
            {
                zeCost = zeUnitData.m_Cost;
                break;
            }
        }
        Assert.IsFalse(zeCost < 0, "Something is wrong with GetUnitCost as it shouldnt be less that 0");
        return zeCost;
    }
}
