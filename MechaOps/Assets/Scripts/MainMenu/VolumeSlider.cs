using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private GameAudioSettings m_GameAudioSettings = null;
    [SerializeField] private VolumeType m_VolumeType = VolumeType.BGM;

    private Slider m_Slider = null;

    private void Awake()
    {
        m_Slider = gameObject.GetComponent<Slider>();
        Assert.IsNotNull(m_Slider, MethodBase.GetCurrentMethod().Name + " - This GameObject must have a UI.Slider!");
    }

    private void Start()
    {
        m_Slider.value = m_GameAudioSettings.GetVolume(m_VolumeType);
    }

    public void OnVolumeChange()
    {
        m_GameAudioSettings.SetVolume(m_VolumeType, m_Slider.value);
    }
}