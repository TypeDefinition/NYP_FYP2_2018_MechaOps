using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "GameEventNames", menuName = "Game Event System/Game Event Names")]
public class GameEventNames : ScriptableObject
{
    // Spawn System
    public enum SpawnSystemNames
    {
        UnitsSpawned,

        Num_SpawnSystemNames
    }
    [SerializeField] private string[] m_SpawnSystemNames = new string[(int)SpawnSystemNames.Num_SpawnSystemNames];
    public string GetEventName(SpawnSystemNames _enumValue) { return m_SpawnSystemNames[(int)_enumValue]; }

    // Game Audio
    public enum GameAudioNames
    {
        VolumeUpdated,

        Num_GameAudioNames
    }
    [SerializeField] private string[] m_GameAudioNames = new string[(int)GameAudioNames.Num_GameAudioNames];
    public string GetEventName(GameAudioNames _enumValue) { return m_GameAudioNames[(int)_enumValue]; }

    // Gameplay
    public enum GameplayNames
    {
        TurnStart,
        TurnEnd,
        UnitSeen,
        UnitUnseen,
        UnitStartAction,
        UnitFinishedAction,
        UnitFinishedTurn,
        GameOver,
        UnitDead,
        FactionDead,
        UnitMovedToTile,
        SetCineUserTransform,
        SetCineTargetTransform,
        StartCinematic,
        StopCinematic,

        Num_GameplayNames
    }
    [SerializeField] private string[] m_GameplayNames = new string[(int)GameplayNames.Num_GameplayNames];
    public string GetEventName(GameplayNames _enumValue) { return m_GameplayNames[(int)_enumValue]; }

    // Game UI
    public enum GameUINames
    {
        ClickedUnit,
        ClickedTile,
        ToggleSelectingUnit,
        SelectedAction,
        FocusOnTarget,
        FollowTarget,

        Num_GameUINames
    }
    [SerializeField] private string[] m_GameUINames = new string[(int)GameUINames.Num_GameUINames];
    public string GetEventName(GameUINames _enumValue) { return m_GameUINames[(int)_enumValue]; }

    // Touch Events
    public enum TouchGestureNames
    {
        Pinch,
        Swipe,
        Scroll,
        CircleGesture,
        DoubleTap,

        Num_TouchNames
    }
    [SerializeField] private string[] m_TouchGestureNames = new string[(int)TouchGestureNames.Pinch];
    public string GetEventName(TouchGestureNames _enumValue) { return m_TouchGestureNames[(int)_enumValue]; }

    public enum SceneManagementNames
    {
        SceneClosed,

        Num_SceneManagementNames
    }
    [SerializeField] private string[] m_SceneManagementNames = new string[(int)SceneManagementNames.Num_SceneManagementNames];
    public string GetEventName(SceneManagementNames _enumValue) { return m_SceneManagementNames[(int)_enumValue]; }

    /// <summary>
    /// Update this function whenever a new enum or array is declared!
    /// IMPORTANT: Make sure to update GameEventNameEditor as well!
    /// ALSO 西北 THE IMPORTANT: Give the enum a string name in the Game Event Names Prefab!
    /// ALSO IMPORTANT: Update HasDuplicateNames!
    /// </summary>
    private void OnValidate()
    {
        AutoResizeArray(ref m_SpawnSystemNames, (int)SpawnSystemNames.Num_SpawnSystemNames);
        AutoResizeArray(ref m_GameAudioNames, (int)GameAudioNames.Num_GameAudioNames);
        AutoResizeArray(ref m_GameplayNames, (int)GameplayNames.Num_GameplayNames);
        AutoResizeArray(ref m_GameUINames, (int)GameUINames.Num_GameUINames);
        AutoResizeArray(ref m_TouchGestureNames, (int)TouchGestureNames.Num_TouchNames);
        AutoResizeArray(ref m_SceneManagementNames, (int)SceneManagementNames.Num_SceneManagementNames);
    }

    /// <summary>
    /// Update this function whenever a new enum or array is declared!
    /// IMPORTANT: Make sure to update GameEventNameEditor as well!
    /// ALSO 西北 THE IMPORTANT: Give the enum a string name in the Game Event Names Prefab!
    /// ALSO IMPORTANT: Update OnValidate!
    /// </summary>
    public bool HasDuplicateNames()
    {
        HashSet<string> allEventNames = new HashSet<string>();

        if (!AddEventNamesToHashSet(allEventNames, m_SpawnSystemNames)) { return true; }
        if (!AddEventNamesToHashSet(allEventNames, m_GameAudioNames)) { return true; }
        if (!AddEventNamesToHashSet(allEventNames, m_GameplayNames)) { return true; }
        if (!AddEventNamesToHashSet(allEventNames, m_GameUINames)) { return true; }
        if (!AddEventNamesToHashSet(allEventNames, m_TouchGestureNames)) { return true; }
        if (!AddEventNamesToHashSet(allEventNames, m_SceneManagementNames)) { return true; }

        return false;
    }

    private void AutoResizeArray(ref string[] _array, int _size)
    {
        if (_array.Length != _size)
        {
            string[] currentArray = _array;
            _array = new string[_size];
            for (int i = 0; i < currentArray.Length && i < _array.Length; ++i)
            {
                _array[i] = currentArray[i];
            }
        }
    }

    private bool AddEventNamesToHashSet(HashSet<string> _allEventNames, string[] _eventNameArray)
    {
        for (int i = 0; i < _eventNameArray.Length; ++i)
        {
            if (_allEventNames.Contains(_eventNameArray[i]))
            {
                return false;
            }
            _allEventNames.Add(_eventNameArray[i]);
        }

        return true;
    }
}