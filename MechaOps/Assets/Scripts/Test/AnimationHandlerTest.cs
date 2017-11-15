using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandlerTest : MonoBehaviour {

    public bool m_StartAnimation = false;
    public GameObject m_Target = null;    
    public bool m_Hit = false;
    private PanzerAnimationHandler_Attack m_AnimationHandlerAttack;
    private PanzerAnimationHandler_Move m_AnimationHandlerMove;

    // Use this for initialization
    void Start () {
        m_AnimationHandlerAttack = gameObject.GetComponent<PanzerAnimationHandler_Attack>();
        m_AnimationHandlerAttack.CompletionCallback = this.AttackCompletionCallback;

        m_AnimationHandlerMove = gameObject.GetComponent<PanzerAnimationHandler_Move>();
        m_AnimationHandlerMove.CompletionCallback = this.MoveCompletionCallback;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (m_StartAnimation)
        {
            m_StartAnimation = false;

            m_AnimationHandlerAttack.TargetPosition = m_Target.transform.position;
            m_AnimationHandlerAttack.Hit = m_Hit;
            m_AnimationHandlerAttack.StartAnimation();
            
            m_AnimationHandlerMove.Destination = m_Target.transform.position;
            m_AnimationHandlerMove.StartAnimation();
        }
	}

    void AttackCompletionCallback()
    {
        Debug.Log("Attack Animation Completed!");
    }

    void MoveCompletionCallback()
    {
        Debug.Log("Move Animation Completed!");
    }
}
