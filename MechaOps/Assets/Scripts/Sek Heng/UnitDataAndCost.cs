using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The container to contain the unit's prefab and the cost to spawn those units!
/// </summary>
[CreateAssetMenu(fileName = "UnitDataAndCost", menuName = "Units/SpawnData", order = 1)]
public class UnitDataAndCost : ScriptableObject {
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

    [SerializeField, Tooltip("The array of unit data")]
    protected UnitsPrefabData[] m_UnitsData;

    public GameObject GetUnitGO(string _name)
    {
        return null;
    }
}
