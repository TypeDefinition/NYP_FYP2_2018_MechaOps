using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSceneLogic : MonoBehaviour {
    [SerializeField] string m_LoadingSceneName = "Loading_Menu";
    [SerializeField] CanvasGroup m_CanvasGroup;
    [SerializeField] float m_BlendSpeed = 1.5f;
    [SerializeField] GameEventNames m_EventNames;

	// Use this for initialization
	IEnumerator Start () {
        if (!m_CanvasGroup)
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
        }
        m_CanvasGroup.alpha = 0;
        while (m_CanvasGroup.alpha < 1.0f)
        {
            m_CanvasGroup.alpha += (m_BlendSpeed * Time.deltaTime);
            yield return null;
        }
        GameEventSystem.GetInstance().SubscribeToEvent(m_EventNames.GetEventName(GameEventNames.SceneManagementNames.LoadingEnded), BeginBlendFinished);
        GameEventSystem.GetInstance().TriggerEvent(m_EventNames.GetEventName(GameEventNames.SceneManagementNames.LoadingBlended));
        yield break;
	}

    void BeginBlendFinished()
    {
        StartCoroutine(BlendRoutine());
    }

    IEnumerator BlendRoutine()
    {
        while (m_CanvasGroup.alpha < 1.0f)
        {
            m_CanvasGroup.alpha -= (m_BlendSpeed * Time.deltaTime);
            yield return null;
        }
        // Unload the current scene
        SceneHelperSingleton.Instance.UnloadScene(m_LoadingSceneName);
        GameEventSystem.GetInstance().UnsubscribeFromEvent(m_EventNames.GetEventName(GameEventNames.SceneManagementNames.LoadingEnded), BeginBlendFinished);
        yield break;
    }
}
