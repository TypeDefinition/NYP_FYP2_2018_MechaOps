using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class MOAnimation_SHSMMove : MOAnimation_Move
{
    [SerializeField] private SHSMAnimator m_Animator = null;

    public override MOAnimator GetMOAnimator() { return m_Animator; }

    // Use this for initialization
    protected virtual void Awake ()
    {
        Assert.IsNotNull(m_Animator, MethodBase.GetCurrentMethod().Name + " - SHSMAnimator is required for MOAnimation_SHSMMove to work!");
    }

    public override void StartAnimation()
    {
        base.StartAnimation();
        m_Animator.SetMoveAnimationParameters(m_Destination, m_CompletionCallback);
        m_Animator.StartMoveAnimation();
    }

    public override void PauseAnimation()
    {
        base.PauseAnimation();
        m_Animator.StopMoveAnimation();
    }

    public override void ResumeAnimation()
    {
        base.ResumeAnimation();
        m_Animator.StartMoveAnimation();
    }

    public override void StopAnimation()
    {
        base.StopAnimation();
        m_Animator.StopMoveAnimation();
    }

}