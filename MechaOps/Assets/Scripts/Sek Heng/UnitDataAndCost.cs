using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        public GameObject m_UnitStatsPrefab;
        public string m_TypeName;
        public int m_Cost;

        public int Cost
        {
            get
            {
                return m_Cost;
            }
        }
        public string TypeName
        {
            get
            {
                return m_TypeName;
            }
        }
        public GameObject UnitStatsPrefab
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
            if (zeUnitData.TypeName == _name)
            {
                return zeUnitData.UnitStatsPrefab;
            }
        }
        return null;
    }

    public GameObject GetEnemyUnitGO(string _name)
    {
        foreach (UnitsPrefabData zeUnitData in m_ArrayOfEnemyUnits)
        {
            if (zeUnitData.TypeName == _name)
            {
                return zeUnitData.UnitStatsPrefab;
            }
        }
        return null;
    }
}
