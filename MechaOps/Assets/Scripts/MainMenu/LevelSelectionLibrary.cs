using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[System.Serializable]
public class LevelSelectionData
{
    [SerializeField] private string m_SceneName = null;

    [SerializeField] private string m_LevelName = null;
    [SerializeField] private string m_LevelDescription = null;
    [SerializeField] private Sprite m_LevelIcon = null;

    public string GetSceneName() { return m_SceneName; }

    public string GetLevelName() { return m_LevelName; }
    public string GetLevelDescription() { return m_LevelDescription; }
    public Sprite GetLevelIcon() { return m_LevelIcon; }
}

[CreateAssetMenu]
public class LevelSelectionLibrary : ScriptableObject
{
    [SerializeField] private LevelSelectionData[] m_Levels = null;

    public LevelSelectionData[] GetLevelSelectionData() { return m_Levels; }
}