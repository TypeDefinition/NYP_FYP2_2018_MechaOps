using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseSceneOnDisable : MonoBehaviour {
    [Header("Variables for CloseSceneOnDisable")]
    [SerializeField, Tooltip("Scene name to unload when this gameobject is set to inactive")]
    protected string m_SceneName;
    [SerializeField, Tooltip("Game event name asset")]
    protected GameEventNames m_EventNamesAsset;

    protected void OnDisable()
    {
        SceneHelperSingleton.Instance.UnloadScene(m_SceneName);
        GameEventSystem.GetInstance().TriggerEvent(m_EventNamesAsset.GetEventName(GameEventNames.SceneManagementNames.SceneClosed));
    }
}
