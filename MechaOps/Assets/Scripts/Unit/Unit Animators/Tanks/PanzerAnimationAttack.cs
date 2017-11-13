using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent]
public class PanzerAnimationAttack : MonoBehaviour
{
    [SerializeField] private PanzerAnimator m_Animator = null;
    [SerializeField] private Vector3 m_TargetPosition = new Vector3();
    [SerializeField] private bool m_Hit = true; // Is this attack a hit or miss?
    public bool m_StartAnimation = false;

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
        m_Animator.ShootAtTargetAnimation(m_TargetPosition, m_Hit);
    }

    void PauseAnimation()
    {
    }

    void ResumeAnimation()
    {
    }

    void StopAnimation()
    {
        gameObject.GetInstanceID();
    }

}