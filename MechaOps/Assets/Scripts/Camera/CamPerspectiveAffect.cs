using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Affects other camera perspective when 1 of the camera changes it's perspective!
/// </summary>
public class CamPerspectiveAffect : MonoBehaviour {
    [Header("Debugging Purpose")]
    [SerializeField, Tooltip("Game Camera Movement")]
    protected GameCameraMovement m_GameCamMovement;
    [SerializeField, Tooltip("Array of camera that exists")]
    protected Camera[] m_AllOtherCameras;

    private void Awake()
    {
        if (!m_GameCamMovement)
        {
            m_GameCamMovement = GetComponent<GameCameraMovement>();
        }
        if (m_AllOtherCameras.Length == 0)
        {
            m_AllOtherCameras = GetComponentsInChildren<Camera>();
        }
    }

    private void OnEnable()
    {
        m_GameCamMovement.FieldOfViewChangeCallbacks += OnChangeFieldOfView;
    }

    private void OnDisable()
    {
        m_GameCamMovement.FieldOfViewChangeCallbacks -= OnChangeFieldOfView;
    }

    /// <summary>
    /// It should be a call back to call this function
    /// </summary>
    /// <param name="_changedValue"></param>
    void OnChangeFieldOfView(float _changedValue)
    {
        foreach (Camera cam in m_AllOtherCameras)
        {
            cam.fieldOfView = _changedValue;
        }
    }
}
