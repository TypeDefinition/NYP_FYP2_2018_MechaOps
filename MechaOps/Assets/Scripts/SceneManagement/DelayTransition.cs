using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Delay the time to do scene transition with loading
/// </summary>
public class DelayTransition : MonoBehaviour {
    [SerializeField] string m_SceneName;
    [SerializeField] float m_TimeDelay = 1.0f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(m_TimeDelay);
        SceneHelperSingleton.Instance.TransitionSceneWithLoading(m_SceneName);
        yield break;
    }
}
