using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// loads the scenes in permanent with the help of SceneHelperSingleton
/// </summary>
public class LoadScenesPermanent : MonoBehaviour {
    [SerializeField, Tooltip("Names of the scene to be added to the Hierachy")]
    string[] m_ArrayOfSceneNames;

    private void Awake()
    {
        foreach (string sceneName in m_ArrayOfSceneNames)
        {
            SceneHelperSingleton.Instance.LoadScenePermanently(sceneName);
        }
    }
}
