using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceVolumeUpdater : MonoBehaviour
{
    [SerializeField] private AudioSource m_AudioSource = null;
    [SerializeField] private GameAudioSettings m_GameAudioSettings = null;
    [SerializeField] private VolumeType m_VolumeType = VolumeType.BGM;

    private void InitialiseEvents()
    {
        GameEventSystem.GetInstance().SubscribeToEvent(m_GameAudioSettings.GetVolumeChangedEventName(), OnVolumeChanged);
    }

    private void DeinitialiseEvents()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent(m_GameAudioSettings.GetVolumeChangedEventName(), OnVolumeChanged);
    }

    private void Awake()
    {
        InitialiseEvents();
    }

    private void Start()
    {
        UpdateVolume();
    }

    private void OnDestroy()
    {
        DeinitialiseEvents();
    }

    private void UpdateVolume()
    {
        m_AudioSource.volume = m_GameAudioSettings.GetVolume(m_VolumeType);
    }

    private void OnVolumeChanged()
    {
        UpdateVolume();
    }
}