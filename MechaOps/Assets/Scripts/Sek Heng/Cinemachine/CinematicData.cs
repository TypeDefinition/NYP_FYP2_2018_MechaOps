using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// Turned on by the CineMachineHandler and turn on the gameobject that it is attached to
/// </summary>
public class CinematicData : MonoBehaviour {
    [Header("Variables for CinematicData")]
    [SerializeField, Tooltip("Name ID to be identified by the CineMachineHandler")]
    protected string m_NameID;
    [SerializeField, Tooltip("Cinemachine virtual camera")]
    protected CinemachineVirtualCameraBase m_CinematicCamera;
    [SerializeField, Tooltip("Cinematic Controls for individual virtual camera")]
    protected CinematicControl[] m_ArrayOfUsableCinematic;

    [Header("Debugging for CinematicData")]
    [SerializeField] protected Transform m_UserFollow;
    [SerializeField] protected Transform m_UserLookAt;
    [SerializeField] protected Transform m_TargetFollow;
    [SerializeField] protected Transform m_TargetLookAt;

    protected Void_Void m_CompleteCinematicCallback;
    public Void_Void CompleteCinematicCallback
    {
        set { m_CompleteCinematicCallback = value; }
        get { return m_CompleteCinematicCallback; }
    }

    public string NameID
    {
        get { return m_NameID; }
    }

    public Transform UserFollow
    {
        get { return m_UserFollow; }
    }

    public Transform UserLookAt
    {
        get { return m_UserLookAt; }
    }

    public Transform TargetFollow
    {
        get { return m_TargetFollow; }
    }

    public Transform TargetLookAt
    {
        get { return m_TargetLookAt; }
    }

    public void SetUserTransform(Transform _userFollow, Transform _userLookAt)
    {
        m_UserFollow = _userFollow;
        m_UserLookAt = _userLookAt;
    }

    public void SetTargetTransform(Transform _targetFollow, Transform _targetLookAt)
    {
        m_TargetFollow = _targetFollow;
        m_TargetLookAt = _targetLookAt;
    }

    public void BeginCinematic(float _time)
    {
        foreach (CinematicControl cineControl in m_ArrayOfUsableCinematic)
        {
            cineControl.SetControl();
        }
        m_CinematicCamera.LookAt = m_UserLookAt;
        m_CinematicCamera.Follow = m_UserFollow;
        StartCoroutine(SetToInactive(_time));
    }

    protected IEnumerator SetToInactive(float _time)
    {
        yield return new WaitForSeconds(_time);
        // set the gameobject it is attached to to be inactive!
        gameObject.SetActive(false);
        if (CompleteCinematicCallback != null)
        {
            CompleteCinematicCallback.Invoke();
        }
        yield break;
    }
}
