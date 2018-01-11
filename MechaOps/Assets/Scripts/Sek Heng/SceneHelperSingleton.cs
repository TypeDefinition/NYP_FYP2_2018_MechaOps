using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHelperSingleton {
    private static SceneHelperSingleton m_Instance;

    public static SceneHelperSingleton Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new SceneHelperSingleton();
            }
            return m_Instance;
        }
    }

    protected List<string> m_ListOfSceneHistory = new List<string>();
    protected string m_ActiveGameSceneName = "";

    /// <summary>
    /// There is a chance that m_ListOfSceneHistory will result in stack overflow.
    /// This hardcoded value is to ensure that list only contains a maximum of 40 objects inside
    /// </summary>
    const int MAX_NUMBER_OF_OBJECTS_IN_LIST = 40;

    /// <summary>
    /// Transition to the game scene straight away
    /// </summary>
    /// <param name="_sceneName"></param>
    public void TransitionScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName, LoadSceneMode.Additive);
        if (m_ActiveGameSceneName != "")
        {
            // only unload the game scene if the name isnt an empty string
            SceneManager.UnloadSceneAsync(m_ActiveGameSceneName);
            AddSceneNameToHistory(m_ActiveGameSceneName);
        }
        m_ActiveGameSceneName = _sceneName;
    }

    /// <summary>
    /// Transition to loading scene then transition to requested scene.
    /// </summary>
    /// <param name="_sceneName"></param>
    public void TransitionSceneWithLoading(string _sceneName)
    {
        if (m_ActiveGameSceneName != "")
        {
            SceneManager.UnloadSceneAsync(m_ActiveGameSceneName);
            AddSceneNameToHistory(m_ActiveGameSceneName);
        }
        GameObject NewGameObj = new GameObject("Loading gameObject", typeof(LoadingSceneHelper));
        LoadingSceneHelper LoadHelper = NewGameObj.GetComponent<LoadingSceneHelper>();
        LoadHelper.TransitAfterLoading(_sceneName);
        m_ActiveGameSceneName = _sceneName;
    }

    /// <summary>
    /// Copy and paste from other functions but with customized time delay!
    /// </summary>
    /// <param name="_sceneName"></param>
    /// <param name="_time"></param>
    public void TransitionSceneWithLoading(string _sceneName, float _time)
    {
        if (m_ActiveGameSceneName != "")
        {
            SceneManager.UnloadSceneAsync(m_ActiveGameSceneName);
            AddSceneNameToHistory(m_ActiveGameSceneName);
        }
        GameObject NewGameObj = new GameObject("Loading gameObject", typeof(LoadingSceneHelper));
        LoadingSceneHelper LoadHelper = NewGameObj.GetComponent<LoadingSceneHelper>();
        LoadHelper.TransitAfterLoading(_sceneName, _time);
        m_ActiveGameSceneName = _sceneName;
    }

    /// <summary>
    /// Transit to the previous scene
    /// </summary>
    public void TransitBack()
    {
        if (m_ListOfSceneHistory.Count == 0)
        {
            Debug.Log("Cannot transit back due to there is nothing at scene history!");
        }
        else
        {
            // unload the current scene
            SceneManager.UnloadSceneAsync(m_ActiveGameSceneName);
            // then make sure to access the last index of the list, remove it and load the current scene
            int LastIndex = m_ListOfSceneHistory.Count - 1;
            m_ActiveGameSceneName = m_ListOfSceneHistory[LastIndex];
            m_ListOfSceneHistory.RemoveAt(LastIndex);
            SceneManager.LoadScene(m_ActiveGameSceneName, LoadSceneMode.Additive);
        }
    }

    /// <summary>
    /// Transition to the loading scene before going to the previous scene
    /// </summary>
    public void TransitBackWithLoading()
    {
        if (m_ListOfSceneHistory.Count == 0)
        {
            Debug.Log("Cannot transit back due to there is nothing at scene history!");
        }
        else
        {
            // unload the current scene
            SceneManager.UnloadSceneAsync(m_ActiveGameSceneName);
            // Get the last index of the scene
            int LastIndex = m_ListOfSceneHistory.Count - 1;
            // set the active scene to the last index since it will be the active scene
            m_ActiveGameSceneName = m_ListOfSceneHistory[LastIndex];
            m_ListOfSceneHistory.RemoveAt(LastIndex);
            GameObject NewGameObj = new GameObject("Loading gameObject", typeof(LoadingSceneHelper));
            LoadingSceneHelper LoadHelper = NewGameObj.GetComponent<LoadingSceneHelper>();
            // Loading scene to do the transitioning!
            LoadHelper.TransitAfterLoading(m_ActiveGameSceneName);
        }
    }

    /// <summary>
    /// make sure that it is not the game scene that will be loaded permanently!
    /// There will be no loading scene and it will be load in additive.
    /// </summary>
    /// <param name="_sceneName"></param>
    public void LoadScenePermanently(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Works similar to SceneManager.UnloadSceneAsync.
    /// Do note that it will not be added to the History list.
    /// </summary>
    /// <param name="_sceneName">Scene name to be unloaded</param>
    public void UnloadScene(string _sceneName)
    {
        SceneManager.UnloadSceneAsync(_sceneName);
    }

    /// <summary>
    /// Helps to standardize when adding the scene to the list of history
    /// </summary>
    /// <param name="_sceneName"></param>
    protected void AddSceneNameToHistory(string _sceneName)
    {
        m_ListOfSceneHistory.Add(_sceneName);
        if (m_ListOfSceneHistory.Count == MAX_NUMBER_OF_OBJECTS_IN_LIST)
        {
            // Remove the 1st index in the history
            m_ListOfSceneHistory.RemoveAt(0);
        }
    }
}
