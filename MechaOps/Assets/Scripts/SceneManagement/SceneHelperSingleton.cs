﻿using System.Collections;
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
                // and make sure that current active scene is added to the scene history!
                m_Instance.AddSceneNameToHistory(SceneManager.GetActiveScene().name);
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
        SetOtherSceneActiveAndUnloadCurrent(_sceneName);
    }

    /// <summary>
    /// Transition to loading scene then transition to requested scene.
    /// </summary>
    /// <param name="_sceneName"></param>
    public void TransitionSceneWithLoading(string _sceneName)
    {
        GameObject NewGameObj = new GameObject("Loading gameObject", typeof(LoadingSceneHelper));
        LoadingSceneHelper LoadHelper = NewGameObj.GetComponent<LoadingSceneHelper>();
        LoadHelper.TransitAfterLoading(_sceneName);
        // it appears that this doesn't work
        // Speculation is LoadingScene is yet to be an active scene then SceneManager wasn't able to unload the active scene
        //SetOtherSceneActiveAndUnloadCurrent(_sceneName);
    }

    /// <summary>
    /// Copy and paste from other functions but with customized time delay!
    /// </summary>
    /// <param name="_sceneName"></param>
    /// <param name="_time"></param>
    public void TransitionSceneWithLoading(string _sceneName, float _time)
    {
         GameObject NewGameObj = new GameObject("Loading gameObject", typeof(LoadingSceneHelper));
        LoadingSceneHelper LoadHelper = NewGameObj.GetComponent<LoadingSceneHelper>();
        LoadHelper.TransitAfterLoading(_sceneName, _time);
        // we will have to make sure that the loading scene comes 1st as the current active scene will never be unloaded!
        // it appears that this doesn't work
        // Speculation is LoadingScene is yet to be an active scene then SceneManager wasn't able to unload the active scene
        //SetOtherSceneActiveAndUnloadCurrent(_sceneName);
    }

    /// <summary>
    /// Unloading the scene then set the other scene to be the current active scene.
    /// However it does not set the scene to be active in the SceneManager but only at the SceneHelperSingleton!
    /// </summary>
    /// <param name="_sceneName"></param>
    public void SetOtherSceneActiveAndUnloadCurrent(string _sceneName)
    {
        if (m_ActiveGameSceneName != "")
        {
            UnloadScene(m_ActiveGameSceneName);
            AddSceneNameToHistory(m_ActiveGameSceneName);
        }
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
        if (m_ListOfSceneHistory.Count >= 1)
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
        if (m_ActiveGameSceneName == "")
        {
            m_ActiveGameSceneName = _sceneName;
        }
        else
        {
            int lastIndexOfHistory = m_ListOfSceneHistory.Count - 1;
            // if it is the same, dont bother!
            if (m_ListOfSceneHistory[lastIndexOfHistory] == _sceneName)
            {
                return;
            }
        }
        m_ListOfSceneHistory.Add(_sceneName);
        if (m_ListOfSceneHistory.Count == MAX_NUMBER_OF_OBJECTS_IN_LIST)
        {
            // Remove the 1st index in the history
            m_ListOfSceneHistory.RemoveAt(0);
        }
    }

    /// <summary>
    /// Similar to SceneManager.SetActiveScene but user won't need to mess with Unity Scene Management
    /// </summary>
    /// <param name="_sceneName">Scene Name which the user wants to set it active</param>
    public void SetSceneActive(string _sceneName)
    {
        Scene WantedScene = SceneManager.GetSceneByName(_sceneName);
        SceneManager.SetActiveScene(WantedScene);
    }

    /// <summary>
    /// transits to loading scene then reloads the current scene
    /// </summary>
    public void ReloadCurrentSceneWithLoadingScreen()
    {
        TransitionSceneWithLoading(m_ActiveGameSceneName);
    }
}
