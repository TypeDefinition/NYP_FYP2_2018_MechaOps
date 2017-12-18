using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MOAnimation_Move : MOAnimation
{
    protected Vector3 m_Destination;

    public Vector3 Destination
    {
        get { return m_Destination; }
        set { m_Destination = value; }
    }

    public override MOAnimator GetMOAnimator()
    {
        throw new System.NotImplementedException();
    }

    public override void StartAnimation() {}

    public override void PauseAnimation() {}

    public override void ResumeAnimation() {}

    public override void StopAnimation() {}
}