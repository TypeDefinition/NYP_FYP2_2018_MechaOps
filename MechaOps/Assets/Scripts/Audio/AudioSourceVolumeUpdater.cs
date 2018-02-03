using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceVolumeUpdater : MonoBehaviour
{
    [SerializeField] private GameEventNames m_GameEventNames = null;
    [SerializeField] private GameAudioSettings m_GameAudioSettings = null;
    [SerializeField] private AudioSource[] m_AudioSources = null;
    [SerializeField] private VolumeType m_VolumeType = VolumeType.BGM;

    public VolumeType VolType { get { return m_VolumeType; } }

    private void InitialiseEvents()
    {
        GameEventSystem.GetInstance().SubscribeToEvent(m_GameEventNames.GetEventName(GameEventNames.GameAudioNames.VolumeUpdated), OnVolumeChanged);
    }

    private void DeinitialiseEvents()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent(m_GameEventNames.GetEventName(GameEventNames.GameAudioNames.VolumeUpdated), OnVolumeChanged);
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
        for (int i = 0; i < m_AudioSources.Length; ++i)
        {
            if (m_AudioSources[i] == null) { continue; }
            m_AudioSources[i].volume = m_GameAudioSettings.GetVolume(m_VolumeType);
        }
    }

    private void OnVolumeChanged()
    {
        UpdateVolume();
    }
}