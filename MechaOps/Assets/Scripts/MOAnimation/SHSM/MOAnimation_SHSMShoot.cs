using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent]
public class MOAnimation_SHSMShoot : MOAnimation
{
    [SerializeField] private SHSMAnimator m_Animator = null;
    private Tile m_TargetTile;

    public Tile TargetTile
    {
        get { return m_TargetTile; }
        set { m_TargetTile = value; }
    }

    public override MOAnimator GetMOAnimator()
    {
        return m_Animator;
    }

    void Awake()
    {
        Assert.IsTrue(m_Animator != null, MethodBase.GetCurrentMethod().Name + " - SHSMAnimator is required for MOAnimation_SHSMShoot to work!");
    }

    public override void StartAnimation()
    {
        m_Animator.SetShootAnimationParameters(m_TargetTile, m_CompletionCallback);
        m_Animator.StartShootAnimation();
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