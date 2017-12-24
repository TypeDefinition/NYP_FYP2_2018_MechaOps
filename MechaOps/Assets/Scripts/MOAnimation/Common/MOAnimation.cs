using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Animations are the ones that bridge the Animator and whatever is using the Animation.
// Sometimes an animation might not even require a Animator.
// Sometimes an animation might require additional parameters.

/*
 * NOTE ABOUT COMPLETION CALLBACK!
 * 
 * An MOAnimation's CompletionCallback is to signal that the ANIMATION has finished.
 * Therefore we pass MOAnimation's CompletionCallback into the Animator.
 * This is because an MOAnimation is pretty much tied to a MOAnimator.
 * 
 * An IUnitAction's CompletionCallback is to signal that the ACTION has finished.
 * Therefore we DO NOT pass IUnitAction's CompletionCallback to the MOAnimation!
 * Just because an ANIMATION has finished, it does not mean that the IUnitAction has finished!
 * What IUnitAction passes to MOAnimation is its OnAnimationCompleted() function!
*/
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