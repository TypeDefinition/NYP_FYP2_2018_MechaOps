using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[DisallowMultipleComponent, RequireComponent(typeof(Canvas))]
public class ScreenSpaceCanvas : MonoBehaviour
{
    [SerializeField] private RectTransform m_GameWinTransform;
    [SerializeField] private RectTransform m_GameLoseTransform;
    [SerializeField] private RectTransform m_EnemyActivityTransform;
    [SerializeField] private UnitSelection m_UnitSelection;
    [SerializeField] private ScrollRect m_UnitActionSelectionScrollRect;
    [SerializeField] private Button m_SettingsButton;

    public RectTransform GetGameWinTransform() { return m_GameWinTransform; }
    public RectTransform GetGameLoseTransform() { return m_GameLoseTransform; }
    public RectTransform GetEnemyActivityTransform() { return m_EnemyActivityTransform; }
    public UnitSelection GetUnitSelection() { return m_UnitSelection; }
    public ScrollRect GetUnitActionSelectionScrollRect() { return m_UnitActionSelectionScrollRect; }
    public Button GetSettingsButton() { return m_SettingsButton; }
}