using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Cinemachine;

/// <summary>
/// Help to decides whether the virtual camera should look at / follow the target 
/// </summary>
public class CinematicControl : MonoBehaviour {
    public enum ControlOfCineCamera
    {
        TrackUser,
        TrackTarget,
        TOTAL_CONTROLS,
    }

    [SerializeField, Tooltip("Cinematic camera that is being attached to")]
    protected CinemachineVirtualCameraBase m_CinematicCam;
    [SerializeField, Tooltip("What it should follow")]
    protected ControlOfCineCamera m_FollowTransformControl;
    [SerializeField, Tooltip("What it should look at")]
    protected ControlOfCineCamera m_LookAtTransformControl;
    [SerializeField, Tooltip("Cinematic data which should be attached to the parent")]
    protected CinematicData m_CinematicData;

	// Use this for initialization
	public void SetControl() {
		if (!m_CinematicData)
        {
            m_CinematicData = transform.parent.GetComponent<CinematicData>();
        }
        switch (m_FollowTransformControl)
        {
            case ControlOfCineCamera.TrackTarget:
                m_CinematicCam.Follow = m_CinematicData.TargetFollow;
                break;
            case ControlOfCineCamera.TrackUser:
                m_CinematicCam.Follow = m_CinematicData.UserFollow;
                break;
            default:
                Assert.IsTrue(true == false, "Something is wrong at cinematic control setcontrol");
                break;
        }
        switch (m_LookAtTransformControl)
        {
            case ControlOfCineCamera.TrackTarget:
                m_CinematicCam.LookAt = m_CinematicData.TargetLookAt;
                break;
            case ControlOfCineCamera.TrackUser:
                m_CinematicCam.LookAt = m_CinematicData.UserLookAt;
                break;
            default:
                Assert.IsTrue(true == false, "Something is wrong at cinematic control set control");
                break;
        }
    }
}
