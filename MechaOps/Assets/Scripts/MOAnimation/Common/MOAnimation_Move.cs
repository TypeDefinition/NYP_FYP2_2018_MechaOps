using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class MOAnimation_Move : MOAnimation
{
    [SerializeField] MOAnimator m_Animator = null;

    protected TileId[] m_MovementPath = null;
    protected Void_Int m_ReachedTileCallback = null;

    public override MOAnimator GetMOAnimator() { return m_Animator; }

    public TileId[] MovementPath
    {
        get { return m_MovementPath; }
        set { m_MovementPath = value; }
    }

    public Void_Int ReachedTileCallback
    {
        get { return m_ReachedTileCallback; }
        set { m_ReachedTileCallback = value; }
    }

    // Use this for initialization
    void Awake()
    {
        Assert.IsTrue(m_Animator != null, MethodBase.GetCurrentMethod().Name + " - No MOAnimator found!");
    }

    public override void StartAnimation()
    {
        m_Animator.StartMoveAnimation(m_MovementPath, m_ReachedTileCallback, m_CompletionCallback);
    }

    public override void PauseAnimation()
    {
        m_Animator.PauseMoveAnimation();
    }

    public override void ResumeAnimation()
    {
        m_Animator.ResumeMoveAnimation();
    }

    public override void StopAnimation()
    {
        m_Animator.StopMoveAnimation();
    }
}