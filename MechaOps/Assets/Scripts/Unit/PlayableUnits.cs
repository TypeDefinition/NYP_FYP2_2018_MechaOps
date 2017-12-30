using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "PlayableUnits", menuName = "Units/Playable Units")]
public class PlayableUnits : ScriptableObject
{
    [SerializeField] private UnitType[] m_List = null;

    public int GetListSize()
    {
        if (m_List == null) { return 0; }
        return m_List.Length;
    }

    public UnitType[] GetList() { return m_List; }

    public UnitType GetUnitType(int _index) { return m_List[_index]; }
}