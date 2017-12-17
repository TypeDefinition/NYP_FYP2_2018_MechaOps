using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent]
public class MOAnimation_PanzerMove : MOAnimation
{
    [SerializeField] private PanzerAnimator m_Animator = null;
    private Vector3 m_Destination;
    
    public PanzerAnimator Animator
    {
        get
        {
            return m_Animator;
        }
    }

    public Vector3 Destination
    {
        get { return m_Destination; }
        set { m_Destination = value; }
    }

	// Use this for initialization
	void Start ()
    {
        Assert.IsTrue(m_Animator != null, MethodBase.GetCurrentMethod().Name + " - PanzerAnimator is required for PanzerAnimationHandler to work!");
	}
	
	// Update is called once per frame
	void Update () {}

    public override void StartAnimation()
    {
        m_Animator.SetMoveAnimationParameters(m_Destination, m_CompletionCallback);
        m_Animator.StartMoveAnimation();
    }

    public override void PauseAnimation()
    {
        m_Animator.StopMoveAnimation();
    }

    public override void ResumeAnimation()
    {
        m_Animator.StartMoveAnimation();
    }

    public override void StopAnimation()
    {
        m_Animator.StopMoveAnimation();
    }
}