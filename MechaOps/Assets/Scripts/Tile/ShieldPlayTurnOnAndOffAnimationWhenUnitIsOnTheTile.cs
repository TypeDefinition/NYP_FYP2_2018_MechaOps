using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

// Okay, the name is fucking horrible. Doesn't even correctly describes what this does.
// But I really cannot think of a better name.
// Feel free to rename this hedious shit name if you have a better one.
public class ShieldPlayTurnOnAndOffAnimationWhenUnitIsOnTheTile : MonoBehaviour
{
    [SerializeField] TileDisplay m_TileDisplay = null;
    [SerializeField] MOAnimation_ShieldTurnOn m_ShieldAnimationOn = null;
    [SerializeField] MOAnimation_ShieldTurnOff m_ShieldAnimationOff = null;

    private bool m_HasUnit = false;

    private void Start()
    {
        Assert.IsTrue(m_TileDisplay != null);
        Assert.IsTrue(m_ShieldAnimationOn != null);
        Assert.IsTrue(m_ShieldAnimationOff != null);
    }

    private void Update()
    {
        if (m_TileDisplay.GetOwner().HasUnit() && !m_HasUnit)
        {
            m_ShieldAnimationOn.StartAnimation();
            m_HasUnit = true;
        }

        if (!m_TileDisplay.GetOwner().HasUnit() && m_HasUnit)
        {
            m_ShieldAnimationOff.StartAnimation();
            m_HasUnit = false;
        }
    }

}