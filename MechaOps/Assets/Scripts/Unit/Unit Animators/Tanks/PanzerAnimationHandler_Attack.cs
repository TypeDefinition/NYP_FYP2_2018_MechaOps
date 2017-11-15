using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent]
public class PanzerAnimationHandler_Attack : AnimationHandler
{
    private PanzerAnimator m_Animator = null;
    private Vector3 m_TargetPosition;
    private bool m_Hit = true; // Is this attack a hit or miss?
    
    public Vector3 TargetPosition
    {
        get { return m_TargetPosition; }
        set { m_TargetPosition = value; }
    }

    public bool Hit
    {
        get { return m_Hit; }
        set { m_Hit = value; }
    }

	// Use this for initialization
	void Start ()
    {
        m_Animator = gameObject.GetComponent<PanzerAnimator>();
        Assert.IsTrue(m_Animator != null, MethodBase.GetCurrentMethod().Name + " - PanzerAnimator is required for PanzerAnimationHandler to work!");
	}
	
	// Update is called once per frame
	void Update () {}

    public override void StartAnimation()
    {
        m_Animator.SetShootAnimationParameters(m_TargetPosition, m_Hit, m_CompletionCallback);
        m_Animator.StartShootAnimation();
    }

    public override void PauseAnimation()
    {
        m_Animator.StopShootAnimation();
    }

    public override void ResumeAnimation()
    {
        m_Animator.StartShootAnimation();
    }

    public override void StopAnimation()
    {
        m_Animator.StopShootAnimation();
    }

}