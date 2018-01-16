using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameEventNames m_EventNamesAsset;
    public enum MenuState
    {
        NONE,
        StartScreen,
        Settings,
        LevelSelection,
        UnitsSelection,
        StartGame,
        QuitApplication,
    }

    [SerializeField] private MenuState m_CurrentState = MenuState.StartScreen;
    [SerializeField] private string m_MainMenuSceneName = "Main_Menu";
    private string m_SelectedLevelSceneName = null;

    [SerializeField] Canvas m_StartScreenCanvas = null;
    [SerializeField] Canvas m_SettingsCanvas = null;
    [SerializeField] Canvas m_LevelSelectionCanvas = null;
    [SerializeField] Canvas m_UnitsSelectionCanvas = null;

    private void Awake()
    {
        SetMenuState(MenuState.StartScreen);
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
        //SceneManager.LoadScene(m_SelectedLevelSceneName, LoadSceneMode.Single);
        SceneHelperSingleton.Instance.TransitionSceneWithLoading(m_SelectedLevelSceneName);
    }

    public void SetMenuState(MenuState _state)
    {
        if (m_CurrentState != _state)
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
                    //m_SettingsCanvas.gameObject.SetActive(true);
                    SceneHelperSingleton.Instance.LoadScenePermanently("Setting_Menu");
                    // and make sure that the current scene is still active!
                    GameEventSystem.GetInstance().SubscribeToEvent(m_EventNamesAsset.GetEventName(GameEventNames.SceneManagementName.SceneClosed), WaitForOtherSceneClosed);
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
            m_CurrentState = _state;
        }
    }

    /// <summary>
    /// This will wait till other permanent scene is finished loaded then loads the current scene
    /// </summary>
    void WaitForOtherSceneClosed()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent(m_EventNamesAsset.GetEventName(GameEventNames.SceneManagementName.SceneClosed), WaitForOtherSceneClosed);
        // set back to main menu
        SetMenuState(MenuState.StartScreen);
    }
}