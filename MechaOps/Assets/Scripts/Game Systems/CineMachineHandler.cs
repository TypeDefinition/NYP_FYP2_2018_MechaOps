using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Cinemachine;

/// <summary>
/// It will handle the Cinemachine for this gameplay
/// </summary>
public class CineMachineHandler : MonoBehaviour {
    [System.Serializable]
    public class CinemachineDataHolder
    {
        public string m_HolderName;
        public CinemachineVirtualCameraBase m_CinematicCam;
    }

    [Header("Variables for CineMachineHandler")]
    [SerializeField, Tooltip("CineMachine for attacking cinematics")]
    protected CinemachineStateDrivenCamera m_CineStateCam;
    [SerializeField, Tooltip("To turn on the the death cinematic camera")]
    protected CinemachineVirtualCameraBase m_DeathCinemachine;

    [SerializeField, Tooltip("Array of cinematic camera for panzer")]
    protected CinemachineDataHolder[] m_ArrayOfCineCamPanzer;

    [Header("Debugging for CineMachineHandler")]
    [SerializeField, Tooltip("CineMachine Brain Script. Thr should only be 1!")]
    protected CinemachineBrain m_CineBrain;
    [SerializeField, Tooltip("Original position of the camera")]
    protected Vector3 m_OriginalCamPos;
    [SerializeField, Tooltip("Original rotation of the camera")]
    protected Quaternion m_OriginalCamRotation;
    [SerializeField, Tooltip("Current active Cinematic camera")]
    protected CinemachineVirtualCameraBase m_ActiveCamBase;
    [SerializeField, Tooltip("Current string of the active cam holder")]
    protected string m_CamHolderString;

    public CinemachineStateDrivenCamera CineStateCam
    {
        get
        {
            return m_CineStateCam;
        }
    }

    public CinemachineVirtualCameraBase ActiveCamBase
    {
        get
        {
            return m_ActiveCamBase;
        }
    }

    /// <summary>
    /// The coroutine to be controlled to turn off cinemachine
    /// </summary>
    protected Coroutine m_CineshutdownCoroutine;

    /// <summary>
    /// Try to get those missing variable when Awake()
    /// </summary>
    private void Awake()
    {
        if (!m_CineBrain)
            m_CineBrain = FindObjectOfType<CinemachineBrain>();
    }

    /// <summary>
    /// Set the cinebrain to active or inactive.
    /// </summary>
    /// <param name="_toggleFlag">true will set the CineBrain active. false will set the CineBrain inactive</param>
    public void SetCineBrain(bool _toggleFlag)
    {
        // and ensure that the coroutine is not null
        if (m_CineBrain.enabled != _toggleFlag && m_CineshutdownCoroutine == null)
        {
            m_CineBrain.enabled = _toggleFlag;
            switch (_toggleFlag)
            {
                case true:
                    // if set active, then need to contain the original position and rotation of the camera
                    m_OriginalCamPos = Camera.main.transform.position;
                    m_OriginalCamRotation = Camera.main.transform.localRotation;
                    break;
                default:
                    // set the camera back to normal
                    Camera.main.transform.position = m_OriginalCamPos;
                    Camera.main.transform.localRotation = m_OriginalCamRotation;
                    if (m_ActiveCamBase != null)
                    {
                        m_ActiveCamBase.gameObject.SetActive(false);
                        m_ActiveCamBase = null;
                        m_CamHolderString = "";
                    }
                    break;
            }
        }
    }

    public void SetPanzerCinematicCamActive(string _CamName)
    {
        SetCineBrain(true);
        if (_CamName != m_CamHolderString)
        {
            if (m_ActiveCamBase != null)
            {
                m_ActiveCamBase.gameObject.SetActive(false);
                m_ActiveCamBase = null;
            }
            foreach (CinemachineDataHolder zeCamHolder in m_ArrayOfCineCamPanzer)
            {
                if (zeCamHolder.m_HolderName == _CamName)
                {
                    m_ActiveCamBase = zeCamHolder.m_CinematicCam;
                    m_CamHolderString = zeCamHolder.m_HolderName;
                }
            }
            if (m_ActiveCamBase != null)
            {
                m_ActiveCamBase.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// The function to start the coroutine in order to delay setting the cinemachine active/inactive
    /// </summary>
    /// <param name="_toggleFlag">flag to set the activeness of the cinemachine</param>
    /// <param name="_delayTime">time to be delayed</param>
    public void DelayCinemachineSetActive(bool _toggleFlag, float _delayTime)
    {
        if (m_CineshutdownCoroutine == null)
        {
            m_CineshutdownCoroutine = StartCoroutine(DelayCinemachineUpdate(_toggleFlag, _delayTime));
        }
    }

    /// <summary>
    /// The coroutine to delay the update of setting the cinebrain active/inactive
    /// </summary>
    /// <param name="_toggleFlag">What kind of flag to set</param>
    /// <param name="_delayTime">Time to be delay</param>
    /// <returns></returns>
    protected IEnumerator DelayCinemachineUpdate(bool _toggleFlag, float _delayTime)
    {
        yield return new WaitForSeconds(_delayTime);
        m_CineshutdownCoroutine = null;
        SetCineBrain(_toggleFlag);
        yield break;
    }
}
