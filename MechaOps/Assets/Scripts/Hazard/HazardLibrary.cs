using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public enum HazardType
{
    Fire,

    Num_HazardType,
    None = -1,
}

public class HazardLibrary : MonoBehaviour
{
    [SerializeField] private Hazard[] m_Library = new Hazard[(int)HazardType.Num_HazardType];

    public Hazard GetHazard(HazardType _type) { return m_Library[(int)_type]; }
}