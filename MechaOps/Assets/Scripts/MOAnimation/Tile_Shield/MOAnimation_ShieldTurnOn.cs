using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent]
public class MOAnimation_ShieldTurnOn : MOAnimation
{
    [SerializeField] private MOAnimator_Shield m_Animator = null;

    private void Start()
    {
        Assert.IsTrue(m_Animator != null, MethodBase.GetCurrentMethod().Name + " - An Animator is required for this to work!");
    }

    public override MOAnimator GetMOAnimator()
    {
        return m_Animator;
    }

    public override void StartAnimation()
    {
        m_Animator.StartTurnOnAnimation();
    }

    public override void PauseAnimation()
    {
        m_Animator.StopAllAnimations();
    }

    public override void ResumeAnimation()
    {
        m_Animator.StartTurnOnAnimation();
    }

    public override void StopAnimation()
    {
        m_Animator.StopAllAnimations();
    }

}