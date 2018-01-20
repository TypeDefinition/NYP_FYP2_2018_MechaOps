using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To deal with the sound effects and future in-game setting. maybe like controls
/// </summary>
public class SettingMenuScript : MonoBehaviour {
    [Header("Variables for SettingMenuScript")]
    [SerializeField, Tooltip("Remove this UI script")]
    protected TweenDisableScript m_DisableScript;
    [SerializeField, Tooltip("Name of this scene")]
    protected string m_SceneName = "Setting_Menu";
    [SerializeField, Tooltip("Game Event Name asset")]
    protected GameEventNames m_EventNamesAsset;
    [SerializeField, Tooltip("Audio Source to play sound sfx")]
    protected AudioSource m_AudioSource;
    [SerializeField, Tooltip("Sound effects for confirming")]
    protected AudioClip m_SFXClip;

	// Use this for initialization
	void Start () {
		if (!m_DisableScript)
        {
            m_DisableScript = GetComponent<TweenDisableScript>();
        }
	}

    public void PressedDone()
    {
        // this will help to set this gameobject to be inactive
        m_DisableScript.AnimateUI();
        m_AudioSource.PlayOneShot(m_SFXClip);
    }

    /// <summary>
    /// When this get disabled, unload this setting menu scene and send out event
    /// </summary>
    private void OnDisable()
    {
        SceneHelperSingleton.Instance.UnloadScene(m_SceneName);
        //TODO: Send out events!
        GameEventSystem.GetInstance().TriggerEvent(m_EventNamesAsset.GetEventName(GameEventNames.SceneManagementNames.SceneClosed));
    }
}
