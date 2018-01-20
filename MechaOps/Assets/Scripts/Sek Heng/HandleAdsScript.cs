//#define LIVE_BUILD

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class HandleAdsScript : MonoBehaviour {
    [SerializeField, Tooltip("Uncheck this if this build is going live")]
    protected bool m_TestBuild = true;
#if UNITY_ANDROID || UNITY_EDITOR
    const string m_AdsGameID = "1676230";
#elif UNITY_IOS
    const string m_AdsGameID = "1676229";
#endif

    // Use this for initialization
    void Start () {
#if LIVE_BUILD
        m_TestBuild = false;
#endif
        Advertisement.Initialize(m_AdsGameID, m_TestBuild);
	}
}
