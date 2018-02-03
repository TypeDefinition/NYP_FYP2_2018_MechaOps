using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOAnimation_DamageIndicator : MOAnimation
{
    [SerializeField] protected MOAnimator m_Animator = null;

    private GameObject m_Target = null;
    private bool m_Hit = false; // Is this attack a hit or miss?
    private int m_DamageValue = 0;

    public GameObject Target
    {
        get { return m_Target; }
        set { m_Target = value; }
    }

    public bool Hit
    {
        get { return m_Hit; }
        set { m_Hit = value; }
    }

    public int DamageValue
    {
        get { return m_DamageValue; }
        set { m_DamageValue = value; }
    }

    public override MOAnimator GetMOAnimator()
    {
        return m_Animator;
    }

    public override void StartAnimation()
    {
        m_Animator.CreateDamageIndicator(m_Hit, m_DamageValue, m_Target, m_CompletionCallback);
    }

    public override void PauseAnimation() {}

    public override void ResumeAnimation() {}

    public override void StopAnimation() {}
}