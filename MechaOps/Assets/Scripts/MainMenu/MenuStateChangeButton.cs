using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class MenuStateChangeButton : MonoBehaviour
{
    [SerializeField] private MainMenuManager m_MainMenuManager = null;
    [SerializeField] private MainMenuManager.MenuState m_MenuState = MainMenuManager.MenuState.StartScreen;
    [SerializeField] private AudioSource m_PlaySFXSource;
    [SerializeField] private AudioClip m_SFXClip;
    private void Awake()
    {
        Assert.IsNotNull(m_MainMenuManager, MethodBase.GetCurrentMethod().Name + " - m_MainMenuManager must not be null!");
    }

    public void OnClick()
    {
        m_MainMenuManager.SetMenuState(m_MenuState);
        m_PlaySFXSource.PlayOneShot(m_SFXClip);
    }
}
