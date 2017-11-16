using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimationHandler : MonoBehaviour
{
    [Tooltip("The Handler name")]
    public string m_HandleName;

    protected Void_Void m_CompletionCallback = null;

    public Void_Void CompletionCallback
    {
        get { return m_CompletionCallback; }
        set { m_CompletionCallback = value; }
    }

    public abstract void StartAnimation();
    public abstract void PauseAnimation();
    public abstract void ResumeAnimation();
    public abstract void StopAnimation();

}