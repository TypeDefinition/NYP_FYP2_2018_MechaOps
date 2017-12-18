using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOAnimator : MonoBehaviour
{
    [SerializeField] protected CineMachineHandler m_CineMachineHandler = null;

    public CineMachineHandler GetCineMachineHandler() { return m_CineMachineHandler; }

    /// <summary>
    /// Stopped the cinematic camera if there is any!
    /// </summary>
    public void StopCinematicCamera()
    {
        if (m_CineMachineHandler)
        {
            m_CineMachineHandler.SetCineBrain(false);
        }
    }
}