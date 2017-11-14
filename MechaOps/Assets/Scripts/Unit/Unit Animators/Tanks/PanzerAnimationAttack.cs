using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent]
public class PanzerAnimationAttack : MonoBehaviour
{
    private PanzerAnimator m_Animator = null;
    [SerializeField] private GameObject m_Target = null;
    [SerializeField] private bool m_Hit = true; // Is this attack a hit or miss?
    
    // For Debugging
    public bool m_StartAnimation = false;

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
	void Update ()
    {
        if (m_StartAnimation)
        {
            StartAnimation();
            m_StartAnimation = false;
        }
	}

    void StartAnimation()
    {
        //m_Animator.SetShootAnimationParameters(m_Target.transform.position, m_Hit);
        m_Animator.StartShootAnimation();
    }

    void PauseAnimation()
    {
        m_Animator.StopShootAnimation();
    }

    void ResumeAnimation()
    {
        m_Animator.StartShootAnimation();
    }

    void StopAnimation()
    {
        m_Animator.StopShootAnimation();
    }

}