using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditSceneScript : MonoBehaviour {
    [Header("Variables for CreditSceneScript")]
    [SerializeField, Tooltip("SFX to closet the scene")]
    protected AudioClip m_CloseSFX;

    [Header("Debugging purpose")]
    [SerializeField, Tooltip("Script to close scene on disable")]
    protected TweenDisableScript m_CloseScene;
    [SerializeField, Tooltip("Audio source to play the sfx")]
    protected AudioSource m_SFXSource;

    private void Awake()
    {
        if (!m_CloseScene)
        {
            m_CloseScene = GetComponent<TweenDisableScript>();
        }
        if (!m_SFXSource)
        {
            // get all available audio source!
            AudioSource[] allAudioSource = FindObjectsOfType<AudioSource>();
            foreach (AudioSource audioSource in allAudioSource)
            {
                AudioSourceVolumeUpdater volumeScript = audioSource.GetComponent<AudioSourceVolumeUpdater>();
                // if the script does exists!
                if (volumeScript)
                {
                    if (volumeScript.VolType == VolumeType.SFX)
                    {
                        m_SFXSource = audioSource;
                        break;
                    }
                }
            }
        }
    }

    public void PressedDone()
    {
        m_CloseScene.AnimateUI();
        if (m_SFXSource)
        {
            m_SFXSource.PlayOneShot(m_CloseSFX);
        }
    }
}
