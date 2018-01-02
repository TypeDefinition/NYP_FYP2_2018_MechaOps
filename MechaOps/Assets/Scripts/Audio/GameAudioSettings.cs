using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VolumeType
{
    BGM,
    Ambient,
    SFX,

    Num_VolumeType
};

[System.Serializable, CreateAssetMenu(fileName = "GameAudioSettings", menuName = "Audio/Game Audio Settings")]
public class GameAudioSettings : ScriptableObject
{
    [SerializeField] private GameEventNames m_GameEventNames = null;
    [SerializeField] private float[] m_Volumes = new float[(int)VolumeType.Num_VolumeType];

#if UNITY_EDITOR
    public void SetGameEventNames(GameEventNames _gameEventNames)
    {
        m_GameEventNames = _gameEventNames;
    }

    public GameEventNames GetGameEventNames()
    {
        return m_GameEventNames;
    }
#endif

    public void SetVolume(VolumeType _volumeType, float _volume)
    {
        m_Volumes[(int)_volumeType] = Mathf.Clamp(_volume, 0.0f, 1.0f);

        GameEventSystem.GetInstance().TriggerEvent(m_GameEventNames.GetEventName(GameEventNames.GameAudioNames.VolumeUpdated));
    }

    public float GetVolume(VolumeType _volumeType) { return m_Volumes[(int)_volumeType]; }

#if UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < (int)VolumeType.Num_VolumeType; ++i)
        {
            SetVolume((VolumeType)i, m_Volumes[i]);
        }
    }
#endif // UNITY_EDITOR
}