using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class TileDisplay_Shield : TileDisplay
{
    [SerializeField] ShieldAnimator m_ShieldAnimator = null;
    private bool m_HasUnit = false;

    public ShieldAnimator GetShieldAnimator()
    {
        return m_ShieldAnimator;
    }

    private void Start()
    {
        Assert.IsTrue(m_ShieldAnimator != null, MethodBase.GetCurrentMethod().Name + " - ShieldAnimator required for this to work!");
    }

    private void Update()
    {
        if (GetOwner().HasUnit() && !m_HasUnit)
        {
            m_ShieldAnimator.StartTurnOnAnimation();
            m_HasUnit = true;
        }

        if (!GetOwner().HasUnit() && m_HasUnit)
        {
            m_ShieldAnimator.StartTurnOffAnimation();
            m_HasUnit = false;
        }
    }

}