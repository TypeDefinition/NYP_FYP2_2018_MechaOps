using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FactionType
{
    Player,
    Enemy,
    Dummy,

    Num_FactionType,
    None = -1
}

[System.Serializable, CreateAssetMenu(fileName = "FactionNames", menuName = "Factions/Faction Names")]
public class FactionNames : ScriptableObject
{
    [SerializeField] private string[] m_FactionNames = new string[(int)FactionType.Num_FactionType];

    public string GetFactionName(FactionType _factionType)
    {
        if (_factionType == FactionType.None) { return "None";  }
        if (_factionType == FactionType.Num_FactionType) { return "Num_FactionType"; }

        return m_FactionNames[(int)_factionType];
    }
    public string[] GetAllFactionNames() { return m_FactionNames; }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (m_FactionNames == null)
        {
            m_FactionNames = new string[(int)FactionType.Num_FactionType];
        }

        if (m_FactionNames.Length != (int)FactionType.Num_FactionType)
        {
            string[] currentNames = m_FactionNames;
            m_FactionNames = new string[(int)FactionType.Num_FactionType];
            for (int i = 0; i < currentNames.Length && i < m_FactionNames.Length; ++i)
            {
                m_FactionNames[i] = currentNames[i];
            }
        }
    }
#endif // UNITY_EDITOR
}