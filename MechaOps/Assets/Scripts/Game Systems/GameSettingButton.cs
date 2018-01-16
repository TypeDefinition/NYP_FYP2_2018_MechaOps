using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingButton : MonoBehaviour {
    [Header("Variables for GameSettingManager")]
    [SerializeField, Tooltip("Game Setting Manager")]
    protected GameSettingManager m_GameSettingManager;
    [SerializeField, Tooltip("State to change to")]
    protected GameSettingManager.GameSettingStates m_StateToChangeTo;
    [SerializeField, Tooltip("Audio Source gameobject")]
    protected AudioSource m_PlaySFXSource;
    [SerializeField, Tooltip("Audio clip for SFX")]
    protected AudioClip m_PlaySFXClip;

    public void ClickButton()
    {
        m_PlaySFXSource.PlayOneShot(m_PlaySFXClip);
        m_GameSettingManager.ChangeState(m_StateToChangeTo);
    }
}
