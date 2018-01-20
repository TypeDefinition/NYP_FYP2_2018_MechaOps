//#define LIVE_BUILD

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class HandleAdsScript : MonoBehaviour {
    [Header("Variables for HandleAdsScript")]
    [SerializeField, Tooltip("Uncheck this if this build is going live")]
    protected bool m_TestBuild = true;
    [SerializeField, Tooltip("Placement ID of the video which can be seen at the unity operate sites")]
    protected string m_PlacementID = "video";

    protected ShowOptions m_ShowOption;
    public ShowOptions ShowOpt
    {
        get { return m_ShowOption; }
    }

#if UNITY_ANDROID || UNITY_EDITOR
    const string m_AdsGameID = "1676230";
#elif UNITY_IOS
    const string m_AdsGameID = "1676229";
#endif

    // Use this for initialization
    void Awake () {
        // if there is no internet connection at all, dont display the ads at all!
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Destroy(gameObject);
            return;
        }
#if LIVE_BUILD
        m_TestBuild = false;
#endif
        Advertisement.Initialize(m_AdsGameID, m_TestBuild);
        m_ShowOption = new ShowOptions();
    }

    IEnumerator Start()
    {
        // Have to loop until it is ready!
        while (!Advertisement.IsReady(m_PlacementID))
        {
            yield return null;
        }
        Advertisement.Show(m_PlacementID, m_ShowOption);
        yield break;
    }
}
