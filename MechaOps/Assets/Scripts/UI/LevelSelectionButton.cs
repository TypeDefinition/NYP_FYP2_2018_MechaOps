using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

[RequireComponent(typeof(Toggle))]
public class LevelSelectionButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_LevelName;
    [SerializeField] private TextMeshProUGUI m_LevelDescription;
    [SerializeField] private Image m_LevelIcon;

    private LevelSelectionCanvas m_LevelSelectionCanvas = null;
    private MainMenuManager m_MainMenuManager = null;
    private LevelSelectionData m_LevelSelectionData = null;

    private void Awake()
    {
        Assert.IsNotNull(m_LevelName, MethodBase.GetCurrentMethod().Name + " - Level Name must not be null!");
        Assert.IsNotNull(m_LevelDescription, MethodBase.GetCurrentMethod().Name + " - Level Description must not be null!");
        Assert.IsNotNull(m_LevelIcon, MethodBase.GetCurrentMethod().Name + " - Level Icon must not be null!");
    }

    public void SetLevelSelectionCanvas(LevelSelectionCanvas _levelSelectionCanvas) { m_LevelSelectionCanvas = _levelSelectionCanvas;  }

    public LevelSelectionCanvas GetLevelSelectionCanvas() { return m_LevelSelectionCanvas; }

    public void SetMainMenuManager(MainMenuManager _mainMenuManager)
    {
        m_MainMenuManager = _mainMenuManager;
    }

    public MainMenuManager GetMainMenuManager() { return m_MainMenuManager; }

    public void SetLevelSelectionData(LevelSelectionData _data)
    {
        m_LevelSelectionData = _data;

        if (m_LevelSelectionData == null)
        {
            m_LevelName.text = "<Insert Level Name>";
            m_LevelDescription.text = "<Insert Level Description>";
            m_LevelIcon.sprite = null;
        }
        else
        {
            m_LevelName.text = m_LevelSelectionData.GetLevelName();
            m_LevelDescription.text = m_LevelSelectionData.GetLevelDescription();
            m_LevelIcon.sprite = m_LevelSelectionData.GetLevelIcon();
        }
    }

    LevelSelectionData GetLevelSelectionData() { return m_LevelSelectionData; }

    public void OnValueChanged()
    {
        Toggle toggle = gameObject.GetComponent<Toggle>();
        if (toggle.isOn)
        {
            m_LevelSelectionCanvas.ToggleAllButtonsOff(this);
            m_MainMenuManager.SetSelectedLevelSceneName(m_LevelSelectionData.GetSceneName());
        }
    }
}