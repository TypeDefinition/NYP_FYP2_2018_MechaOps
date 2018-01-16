using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneHelper : MonoBehaviour {
    protected const string m_LoadingSceneName = "Loading";
    protected const float m_TimeDelayForLoading = 1.25f;

    /// <summary>
    /// Transition to other scene with a default time delay of 1.25 seconds
    /// </summary>
    /// <param name="_sceneName">Game scene to be loaded</param>
    public void TransitAfterLoading(string _sceneName)
    {
        StartCoroutine(TransitToSceneAsync(_sceneName, m_TimeDelayForLoading));
    }

    /// <summary>
    /// Transition to other scene with custom time delay
    /// </summary>
    /// <param name="_sceneName">Game scene to be loaded</param>
    /// <param name="_timeDelay">custom time delay</param>
    public void TransitAfterLoading(string _sceneName, float _timeDelay)
    {
        StartCoroutine(TransitToSceneAsync(_sceneName, m_TimeDelayForLoading));
    }

    /// <summary>
    /// coroutine to wait for the other scenes finished loading!
    /// </summary>
    /// <param name="_sceneName"></param>
    /// <param name="_timeDelay"></param>
    /// <returns></returns>
    protected IEnumerator TransitToSceneAsync(string _sceneName, float _timeDelay)
    {
        // have to wait till the loading scene is finished otherwise loading scene cannot be set active!
        yield return SceneManager.LoadSceneAsync(m_LoadingSceneName, LoadSceneMode.Additive);
        // making sure that the loading scene is active
        print("Setting loading scene to be active: " + SceneManager.SetActiveScene(SceneManager.GetSceneByName(m_LoadingSceneName)));
        // moving this gameobject to the loading scene. so that it will only be destroyed at the loading scene!
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        // then unload the active scene and set the current scene to be active!
        // Need to unload the current scene 1st otherwise the event system in the other scene will get removed!
        SceneHelperSingleton.Instance.SetOtherSceneActiveAndUnloadCurrent(_sceneName);
        // Then begin loading the other scenes
        AsyncOperation OtherSceneOperation = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
        yield return OtherSceneOperation;
        // set loaded scene to be active!
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneName));
        // Maybe this line can be removed. it just delays the current loading to a further of some time! But loading of the game is done very fast
        yield return new WaitForSecondsRealtime(_timeDelay);
         // After other scene has finished loading, then unload the loading scene!
       SceneManager.UnloadSceneAsync(m_LoadingSceneName);
        yield break;
    }
}
