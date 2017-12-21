using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Animations are the ones that bridge the Animator and whatever is using the Animation.
// Sometimes an animation might not even require a Animator.
// Sometimes an animation might require additional parameters.
public abstract class MOAnimation : MonoBehaviour
{
    protected Void_Void m_CompletionCallback = null;

    public Void_Void CompletionCallback
    {
        get { return m_CompletionCallback; }
        set { m_CompletionCallback = value; }
    }

    public abstract MOAnimator GetMOAnimator();
    public abstract void StartAnimation();
    public abstract void PauseAnimation();
    public abstract void ResumeAnimation();
    public abstract void StopAnimation();
}