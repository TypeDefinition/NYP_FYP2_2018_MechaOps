using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsDestroyScene : MonoBehaviour {
    [Header("Debugging for AdsDestroyScene")]
    [SerializeField, Tooltip("Script of HandleAdScript")]
    protected HandleAdsScript m_HandleAds;
    [SerializeField, Tooltip("Name of the scene to unload when ads is completed")]
    protected string m_SceneNameToUnload = "Ads_Menu";
#if UNITY_IOS || UNITY_ANDROID
    private void OnEnable()
    {
        if (!m_HandleAds)
        {
            m_HandleAds = GetComponent<HandleAdsScript>();
        }
        m_HandleAds.ShowOpt.resultCallback += CompleteAds;
    }

    private void OnDisable()
    {
        m_HandleAds.ShowOpt.resultCallback -= CompleteAds;
    }

    private void CompleteAds(ShowResult _result)
    {
        // regardless of the result just unload the scene
        Destroy(gameObject);
    }
#endif
    private void OnDestroy()
    {
        SceneHelperSingleton.Instance.UnloadScene(m_SceneNameToUnload);
    }
}
