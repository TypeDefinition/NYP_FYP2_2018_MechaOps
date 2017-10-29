using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public enum HazardType
{
    None = -1,

    Fire = 0,

    Num_HazardType
}

public class HazardLibrary : MonoBehaviour {

    [SerializeField] private Hazard[] m_Library = new Hazard[(uint)HazardType.Num_HazardType];
    
    public Hazard GetHazard(HazardType _type)
    {
        return m_Library[(uint)_type];
    }

}
