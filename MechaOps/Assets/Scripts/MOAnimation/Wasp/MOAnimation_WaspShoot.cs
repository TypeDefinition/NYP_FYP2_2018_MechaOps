using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class MOAnimation_WaspShoot : MOAnimation
{
    [SerializeField] private MOAnimator_Wasp m_Animator = null;

    private GameObject m_Target = null;

    public override MOAnimator GetMOAnimator() { return m_Animator; }

    public GameObject Target
    {
        get { return m_Target; }
        set { m_Target = value; }
    }

    // Use this for initialization
    protected virtual void Awake ()
    {
        Assert.IsTrue(m_Animator != null, MethodBase.GetCurrentMethod().Name + " - No MOAnimator found!");
    }

    public override void StartAnimation()
    {
        m_Animator.StartShootAnimation(m_Target, m_CompletionCallback);
    }

    public override void PauseAnimation()
    {
        m_Animator.PauseShootAnimation();
    }

    public override void ResumeAnimation()
    {
        m_Animator.ResumeShootAnimation();
    }

    public override void StopAnimation()
    {
        m_Animator.StopShootAnimation();
    }
}