using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandlerTest : MonoBehaviour {

    public bool m_StartAnimation = false;
    public GameObject m_Target = null;    
    public bool m_Hit = false;
    private PanzerAnimationHandler_Attack m_AnimationHandler;

    // Use this for initialization
    void Start () {
        m_AnimationHandler = gameObject.GetComponent<PanzerAnimationHandler_Attack>();
        m_AnimationHandler.CompletionCallback = this.CompletionCallback;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (m_StartAnimation)
        {
            m_AnimationHandler.Target = m_Target;
            m_AnimationHandler.Hit = m_Hit;
            m_AnimationHandler.StartAnimation();
            m_StartAnimation = false;
        }
	}

    void CompletionCallback()
    {
        Debug.Log("Animation Completed!");
    }

}
