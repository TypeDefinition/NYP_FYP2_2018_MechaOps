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
    [SerializeField, Tooltip("CineMachine Brain Script. There should only be 1!")]
    protected CinemachineBrain m_CineBrain;

    [Header("Debugging for CineMachineHandler")]
    [SerializeField, Tooltip("Array of cinematic camera for all units. All cinematicData components should be attached under this gameobject parent!")]
    protected CinematicData[] m_ArrayOfCinematicData;
    [SerializeField, Tooltip("Original position of the camera")]
    protected Vector3 m_OriginalCamPos;
    [SerializeField, Tooltip("Original rotation of the camera")]
    protected Quaternion m_OriginalCamRotation;
    [SerializeField, Tooltip("Active Cinematic Data")]
    protected CinematicData m_ActiveCinematicData;
    [SerializeField, Tooltip("Game Event Names related")]
    protected GameEventNames m_GameEventNames;
    [SerializeField] protected Transform m_UserFollow;
    [SerializeField] protected Transform m_UserLookAt;
    [SerializeField] protected Transform m_TargetFollow;
    [SerializeField] protected Transform m_TargetLookAt;

    protected Dictionary<string, List<CinematicData>> m_NameCineDataDict = new Dictionary<string, List<CinematicData>>();

    /// <summary>
    /// Try to get those missing variable when Awake()
    /// </summary>
    private void Awake()
    {
        if (!m_CineBrain)
        {
            m_CineBrain = FindObjectOfType<CinemachineBrain>();
            // making sure that the cinemachine brain is inactive
            m_CineBrain.enabled = false;
        }
        if (m_ArrayOfCinematicData.Length == 0)
        {
            m_ArrayOfCinematicData = GetComponentsInChildren<CinematicData>();
        }
        foreach (CinematicData CineData in m_ArrayOfCinematicData)
        {
            List<CinematicData> ListOfCineData;
            if (m_NameCineDataDict.TryGetValue(CineData.NameID, out ListOfCineData))
            {
                ListOfCineData.Add(CineData);
            }
            else
            {
                ListOfCineData = new List<CinematicData>();
                ListOfCineData.Add(CineData);
                m_NameCineDataDict.Add(CineData.NameID, ListOfCineData);
            }
            CineData.gameObject.SetActive(false);
        }
        m_GameEventNames = GameSystemsDirectory.GetSceneInstance().GetGameEventNames();
    }

    protected void InitEvents()
    {
        GameEventSystem.GetInstance().SubscribeToEvent<Transform, Transform>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.SetCineUserTransform), SetUserTransform);
        GameEventSystem.GetInstance().SubscribeToEvent<Transform, Transform>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.SetCineTargetTransform), SetTargetTransform);
        GameEventSystem.GetInstance().SubscribeToEvent<string, float>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.StartCinematic), SetCinematic);
        GameEventSystem.GetInstance().SubscribeToEvent(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.StopCinematic), TurnOffCinematic);
    }

    protected void DeinitEvents()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<Transform, Transform>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.SetCineUserTransform), SetUserTransform);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<Transform, Transform>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.SetCineTargetTransform), SetTargetTransform);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<string, float>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.StartCinematic), SetCinematic);
        GameEventSystem.GetInstance().UnsubscribeFromEvent(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.StopCinematic), TurnOffCinematic);
    }

    protected void OnEnable()
    {
        InitEvents();
    }

    protected void OnDisable()
    {
        DeinitEvents();
    }

    /// <summary>
    /// Set the cinebrain to active or inactive.
    /// </summary>
    /// <param name="_toggleFlag">true will set the CineBrain active. false will set the CineBrain inactive</param>
    protected void SetCineBrain(bool _toggleFlag)
    {
        // and ensure that the coroutine is not null
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
                    Transform cameraTransform = Camera.main.transform;
                    // set the camera back to normal. it appears that user prefers it to be left off where the cinematic camera at the XZ axis!
                    cameraTransform.position = new Vector3(cameraTransform.position.x, m_OriginalCamPos.y, cameraTransform.position.z);
                    //cameraTransform.localRotation = m_OriginalCamRotation;
                    break;
            }
        }
    }

    protected void SetUserTransform(Transform _userFollow, Transform _userLookAt)
    {
        m_UserFollow = _userFollow;
        m_UserLookAt = _userLookAt;
    }

    protected void SetTargetTransform(Transform _targetFollow, Transform _targetLookAt)
    {
        m_TargetFollow = _targetFollow;
        m_TargetLookAt = _targetLookAt;
    }

    /// <summary>
    /// Set the specific cinematic camera to be active
    /// </summary>
    /// <param name="_nameID">Name of the cinematic camera</param>
    /// <param name="_time">Automatically set the time to turn it off through coroutine. if it is below 0, then it will always be active</param>
    protected void SetCinematic(string _nameID, float _time)
    {
        // need to ensure that the previous m_activeCinematicData will not affect the soon-to-be activated cinematic camera
        if (m_ActiveCinematicData)
        {
            m_ActiveCinematicData.CompleteCinematicCallback -= FinishedCinematic;
            m_ActiveCinematicData.gameObject.SetActive(false);
        }
        List<CinematicData> ListOfCineData = null;
        Assert.IsTrue(m_NameCineDataDict.TryGetValue(_nameID, out ListOfCineData), "There should be a cinematic data at CineMachinehandler");
        // random between the cinematic data
        int result = Random.Range(0, ListOfCineData.Count);
        m_ActiveCinematicData = ListOfCineData[result];
        m_ActiveCinematicData.SetUserTransform(m_UserFollow, m_UserLookAt);
        m_ActiveCinematicData.SetTargetTransform(m_TargetFollow, m_TargetLookAt);
        m_ActiveCinematicData.gameObject.SetActive(true);
        SetCineBrain(true);
        m_ActiveCinematicData.CompleteCinematicCallback += FinishedCinematic;
        m_ActiveCinematicData.BeginCinematic(_time);
    }

    /// <summary>
    /// Waiting for cinematic data to call this function
    /// </summary>
    protected void FinishedCinematic()
    {
        // set all of the transform to be null
        m_TargetFollow = null;
        m_TargetLookAt = null;
        m_UserFollow = null;
        m_UserLookAt = null;
        m_ActiveCinematicData.CompleteCinematicCallback -= FinishedCinematic;
        m_ActiveCinematicData.gameObject.SetActive(false);
        m_ActiveCinematicData = null;
        SetCineBrain(false);
    }

    /// <summary>
    /// Turns off cinematic camera
    /// </summary>
    public void TurnOffCinematic()
    {
        Assert.IsNotNull(m_ActiveCinematicData, "Something is wrong as the cinematic camera is already off in the 1st place!");
        FinishedCinematic();
    }
}
