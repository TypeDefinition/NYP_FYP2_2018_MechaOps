using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public enum MenuState
    {
        StartScreen,
        Settings,
        LevelSelection,
        UnitsSelection,
        StartGame,
        QuitApplication,
    }

    [SerializeField] private MenuState m_CurrentState = MenuState.StartScreen;
    private string m_SelectedLevelSceneName = null;

    [SerializeField] Canvas m_StartScreenCanvas = null;
    [SerializeField] Canvas m_SettingsCanvas = null;
    [SerializeField] Canvas m_LevelSelectionCanvas = null;
    [SerializeField] Canvas m_UnitsSelectionCanvas = null;

    private void Awake()
    {
        SetMenuState(m_CurrentState);
    }

    public string GetSelectedLevelSceneName()
    {
        return m_SelectedLevelSceneName;
    }

    public void SetSelectedLevelSceneName(string _sceneName)
    {
        m_SelectedLevelSceneName = _sceneName;
    }

    public void LoadSelectedLevel()
    {
        SceneManager.LoadScene(m_SelectedLevelSceneName, LoadSceneMode.Single);
    }

    public void SetMenuState(MenuState _state)
    {
        switch (_state)
        {
            case MenuState.StartScreen:
                m_StartScreenCanvas.gameObject.SetActive(true);
                m_LevelSelectionCanvas.gameObject.SetActive(false);
                m_SettingsCanvas.gameObject.SetActive(false);
                m_UnitsSelectionCanvas.gameObject.SetActive(false);
                break;
            case MenuState.Settings:
                m_StartScreenCanvas.gameObject.SetActive(false);
                m_LevelSelectionCanvas.gameObject.SetActive(false);
                m_SettingsCanvas.gameObject.SetActive(true);
                m_UnitsSelectionCanvas.gameObject.SetActive(false);
                break;
            case MenuState.LevelSelection:
                m_StartScreenCanvas.gameObject.SetActive(false);
                m_LevelSelectionCanvas.gameObject.SetActive(true);
                m_SettingsCanvas.gameObject.SetActive(false);
                m_UnitsSelectionCanvas.gameObject.SetActive(false);
                break;
            case MenuState.UnitsSelection:
                m_StartScreenCanvas.gameObject.SetActive(false);
                m_LevelSelectionCanvas.gameObject.SetActive(false);
                m_SettingsCanvas.gameObject.SetActive(false);
                m_UnitsSelectionCanvas.gameObject.SetActive(true);
                break;
            case MenuState.StartGame:
                LoadSelectedLevel();
                break;
            case MenuState.QuitApplication:
                Application.Quit();
                break;
            default:
                Assert.IsTrue(false, MethodBase.GetCurrentMethod().Name + " - Unhandled Menu State!");
                break;
        }
    }
}