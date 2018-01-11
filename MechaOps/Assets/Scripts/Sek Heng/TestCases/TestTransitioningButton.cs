using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTransitioningButton : MonoBehaviour {
    public void TransitToOtherScene(string _sceneName)
    {
        SceneHelperSingleton.Instance.TransitionScene(_sceneName);
    }

    public void TransitOtherSceneWithLoading(string _sceneName)
    {
        SceneHelperSingleton.Instance.TransitionSceneWithLoading(_sceneName);
    }

    public void LoadOtherScene(string _sceneName)
    {
        SceneHelperSingleton.Instance.LoadScenePermanently(_sceneName);
    }

    public void TransitBack()
    {
        SceneHelperSingleton.Instance.TransitBack();
    }

    public void TransitBackWithLoading()
    {
        SceneHelperSingleton.Instance.TransitBackWithLoading();
    }

    public void UnloadScene(string _sceneName)
    {
        SceneHelperSingleton.Instance.UnloadScene(_sceneName);
    }
}
