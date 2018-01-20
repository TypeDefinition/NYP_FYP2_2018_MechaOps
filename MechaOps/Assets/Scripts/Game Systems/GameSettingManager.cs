using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Handles the game setting stuff!
/// </summary>
public class GameSettingManager : MonoBehaviour {
    [System.Serializable]
    public enum GameSettingStates
    {
        Playing = 0,
        Setting,
        MainMenuSetting,
        MainMenu,
    }

    [Header("Variables for GameSettingManager")]
    [SerializeField, Tooltip("In game setting tweening script")]
    TweenDisableScript m_InGameSettingUI;
    [SerializeField, Tooltip("Game Event Names")]
    protected GameEventNames m_EventNamesAsset;

    [Header("Debugging for GameSettingManager")]
    [SerializeField, Tooltip("Current state of this setting UI")]
    protected GameSettingStates m_CurrentState;

	// Use this for initialization
	void Start () {
        m_CurrentState = GameSettingStates.Playing;
    }

    public void ChangeState(GameSettingStates _state)
    {
        if (m_CurrentState != _state)
        {
            switch (_state)
            {
                case GameSettingStates.Playing:
                    m_InGameSettingUI.AnimateUI();
                    break;
                case GameSettingStates.Setting:
                    m_InGameSettingUI.gameObject.SetActive(true);
                    break;
                case GameSettingStates.MainMenuSetting:
                    m_InGameSettingUI.gameObject.SetActive(false);
                    SceneHelperSingleton.Instance.LoadScenePermanently("Setting_Menu");
                    GameEventSystem.GetInstance().SubscribeToEvent(m_EventNamesAsset.GetEventName(GameEventNames.SceneManagementNames.SceneClosed), OtherScenesClosed);
                    break;
                case GameSettingStates.MainMenu:
                    SceneHelperSingleton.Instance.TransitionSceneWithLoading("Main_Menu");
                    break;
                default:
                    Assert.IsTrue(true == false, "Changing to the wrong state at GameSettingManager");
                    break;
            }
            m_CurrentState = _state;
        }
    }

    void OtherScenesClosed()
    {
        ChangeState(GameSettingStates.Setting);
        GameEventSystem.GetInstance().UnsubscribeFromEvent(m_EventNamesAsset.GetEventName(GameEventNames.SceneManagementNames.SceneClosed), OtherScenesClosed);
    }
}
