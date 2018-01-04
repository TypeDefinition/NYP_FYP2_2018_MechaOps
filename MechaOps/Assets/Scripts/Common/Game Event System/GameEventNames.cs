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
        UnitFinishedAction,
        UnitFinishedTurn,
        GameOver,
        UnitDead,
        FactionDead,
        UnitMovedToTile,

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

        Num_TouchNames
    }
    [SerializeField] private string[] m_TouchGestureNames = new string[(int)TouchGestureNames.Pinch];
    public string GetEventName(TouchGestureNames _enumValue) { return m_TouchGestureNames[(int)_enumValue]; }

    /*
    // Game Systems
    public enum GameSystemsNames
    {
        SystemsReady,

        Num_GameSystemsNames
    }
    [SerializeField] public string[] m_GameSystemsNames = new string[(int)GameSystemsNames.Num_GameSystemsNames];
    public string GetEventName(GameSystemsNames _enumValue) { return m_GameSystemsNames[(int)_enumValue]; }
    */

    /// <summary>
    /// Update this function whenever a new enum or array is declared!
    /// IMPORTANT: Make sure to update GameEventNameEditor as well!
    /// </summary>
    private void OnValidate()
    {
        AutoResizeArray(ref m_SpawnSystemNames, (int)SpawnSystemNames.Num_SpawnSystemNames);
        AutoResizeArray(ref m_GameAudioNames, (int)GameAudioNames.Num_GameAudioNames);
        AutoResizeArray(ref m_GameplayNames, (int)GameplayNames.Num_GameplayNames);
        AutoResizeArray(ref m_GameUINames, (int)GameUINames.Num_GameUINames);
        AutoResizeArray(ref m_TouchGestureNames, (int)TouchGestureNames.Num_TouchNames);
        //AutoResizeArray(ref m_GameSystemsNames, (int)GameSystemsNames.Num_GameSystemsNames);
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
}