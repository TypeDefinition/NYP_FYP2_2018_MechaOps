using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UnitType
{
    Player_Panzer = 0,
    Player_Wasp,
    Player_SHSM,

    Num_UnitType,
    None = -1
}

[System.Serializable, CreateAssetMenu(fileName = "UnitLibrary", menuName = "Units/Unit Library")]
public class UnitLibrary : ScriptableObject
{
    [System.Serializable]
    public class UnitLibraryData
    {
        [SerializeField] private Sprite m_UnitIconSprite = null;
        [SerializeField] private UnitStats m_UnitStats = null;

        public Sprite GetUnitIconSprite() { return m_UnitIconSprite; }
        public UnitStats GetUnitStats() { return m_UnitStats; }
    }

    [SerializeField] private UnitLibraryData[] m_Library = new UnitLibraryData[(uint)UnitType.Num_UnitType];

    public int GetLibrarySize() { return m_Library.Length; }

    public UnitLibraryData[] GetAllUnitLibraryData() { return m_Library; }

    public UnitLibraryData GetUnitLibraryData(UnitType _unitType) { return m_Library[(int)_unitType]; }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (m_Library == null)
        {
            m_Library = new UnitLibraryData[(uint)UnitType.Num_UnitType];
        }

        if (m_Library.Length != (uint)UnitType.Num_UnitType)
        {
            UnitLibraryData[] currentLibrary = m_Library;
            m_Library = new UnitLibraryData[(uint)UnitType.Num_UnitType];
            for (int i = 0; i < currentLibrary.Length && i < m_Library.Length; ++i)
            {
                m_Library[i] = currentLibrary[i];
            }
        }
    }
#endif // UNITY_EDITOR
}