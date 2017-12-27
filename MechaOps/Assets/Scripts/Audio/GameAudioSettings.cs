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

[CreateAssetMenu]
public class GameAudioSettings : ScriptableObject
{
    private string m_VolumeChangedEventName = "VolumeChanged";
    [SerializeField] private float[] m_Volumes = new float[(int)VolumeType.Num_VolumeType];

    public string GetVolumeChangedEventName() { return m_VolumeChangedEventName; }

    public void SetVolume(VolumeType _volumeType, float _volume)
    {
        m_Volumes[(int)_volumeType] = Mathf.Clamp(_volume, 0.0f, 1.0f);

        GameEventSystem.GetInstance().TriggerEvent(m_VolumeChangedEventName);
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