using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Cinemachine;

/// <summary>
/// It will handle the Cinemachine for this gameplay
/// </summary>
public class CineMachineHandler : MonoBehaviour {
    [Header("Variables for CineMachineHandler")]
    [SerializeField, Tooltip("Animator for controlling cinemachine state machine")]
    protected Animator m_CineAnimator;
    [SerializeField, Tooltip("CineMachine for attacking cinematics")]
    protected CinemachineStateDrivenCamera m_AttckStateCineCam;

    [Header("Debugging for CineMachineHandler")]
    [SerializeField, Tooltip("CineMachine Brain Script. Thr should only be 1!")]
    protected CinemachineBrain m_CineBrain;
    [SerializeField, Tooltip("Original position of the camera")]
    protected Vector3 m_OriginalCamPos;
    [SerializeField, Tooltip("Original rotation of the camera")]
    protected Quaternion m_OriginalCamRotation;

    /// <summary>
    /// To map the parameter name to hash so that the animator variable will be accessed faster!
    /// </summary>
    protected Dictionary<string, int> m_ParamNameKeyDict = new Dictionary<string, int>();

    /// <summary>
    /// Getter for the CineAnimator
    /// </summary>
    public Animator CineAnimator
    {
        get
        {
            return m_CineAnimator;
        }
    }

    public CinemachineStateDrivenCamera AttckStateCineCam
    {
        get
        {
            return m_AttckStateCineCam;
        }
    }

    /// <summary>
    /// Try to get those missing variable when Awake()
    /// </summary>
    private void Awake()
    {
        if (!m_CineAnimator)
            m_CineAnimator = GetComponent<Animator>();
        if (!m_CineBrain)
            m_CineBrain = FindObjectOfType<CinemachineBrain>();
    }

    /// <summary>
    /// Trigger the event variable in the animator
    /// </summary>
    /// <param name="_paramName">Animator Parameter Name</param>
    public void TriggerEventParam(string _paramName)
    {
        int zeAnimHash;
        if (!m_ParamNameKeyDict.TryGetValue(_paramName, out zeAnimHash))
        {
            // if the param animation name has never exists, then instantiate the animhash
            zeAnimHash = Animator.StringToHash(_paramName);
            m_ParamNameKeyDict.Add(_paramName, zeAnimHash);
        }
        TriggerEventParam(zeAnimHash);
    }

    /// <summary>
    /// Trigger the event variable in the animator
    /// </summary>
    /// <param name="_paramHash">Animator parameter hash</param>
    public void TriggerEventParam(int _paramHash)
    {
        // Will need to make sure the CineBrain is active
        SetCineBrain(true);
        CineAnimator.SetTrigger(_paramHash);
    }

    /// <summary>
    /// Set the cinebrain to active or inactive.
    /// </summary>
    /// <param name="_toggleFlag">true will set the CineBrain active. false will set the CineBrain inactive</param>
    public void SetCineBrain(bool _toggleFlag)
    {
        if (m_CineBrain.enabled != _toggleFlag)
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
                    break;
            }
        }
    }

}
