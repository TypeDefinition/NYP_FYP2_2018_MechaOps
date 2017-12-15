using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ScreenCanvas : MonoBehaviour
{
    [SerializeField] private GameObject m_GameWinUI;
    [SerializeField] private GameObject m_GameLoseUI;
    [SerializeField] private GameObject m_EnemyActivityUI;
    [SerializeField] private GameObject m_UnitSelectionUI;
    [SerializeField] private GameObject m_UnitActionSelectionUI;
    [SerializeField] private Button m_SettingsButton;

    public GameObject GetGameWinUI() { return m_GameWinUI; }
    public GameObject GetGameLoseUI() { return m_GameLoseUI; }
    public GameObject GetEnemyActivityUI() { return m_EnemyActivityUI; }
    public GameObject GetUnitSelectionUI() { return m_UnitSelectionUI; }
    public GameObject GetUnitActionSelectionUI() { return m_UnitActionSelectionUI; }
    public Button GetSettingsButton() { return m_SettingsButton; }
}