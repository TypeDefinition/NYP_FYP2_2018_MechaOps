using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent]
public class PanzerAnimationHandler_Attack : AnimationHandler
{
    private PanzerAnimator m_Animator = null;
    private GameObject m_Target = null;
    private bool m_Hit = true; // Is this attack a hit or miss?
    
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
        Assert.IsFalse(m_Target == null, MethodBase.GetCurrentMethod().Name + " - Cannot start animation without a target!");

        m_Animator.SetShootAnimationParameters(m_Target.transform.position, m_Hit, m_CompletionCallback);
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