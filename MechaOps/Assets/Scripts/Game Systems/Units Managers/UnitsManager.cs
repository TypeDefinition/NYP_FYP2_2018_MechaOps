using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Assertions;
using UnityEngine;

public abstract class UnitsManager : MonoBehaviour
{
    // Serialised Variable(s)
    [SerializeField, Tooltip("The faction managed by this UnitsManager.")]
    protected FactionType m_ManagedFaction = FactionType.None;

    // Non-Serialised Variable(s)
    protected GameSystemsDirectory m_GameSystemsDirectory = null;
    protected GameEventNames m_GameEventNames = null;
    protected TileSystem m_TileSystem = null;
    protected UnitsTracker m_UnitsTracker = null;

    protected List<UnitStats> m_ManagedUnits = new List<UnitStats>();
    protected List<UnitStats> m_SeenEnemies = new List<UnitStats>();

    public List<UnitStats> GetSeenEnemies() { return m_SeenEnemies; }

    protected virtual void InitEvents()
    {
        // Gameplay
        GameEventSystem.GetInstance().SubscribeToEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.TurnStart), TurnStart);
        GameEventSystem.GetInstance().SubscribeToEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.GameOver), GameOver);

        GameEventSystem.GetInstance().SubscribeToEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitSeen), AddToSeenEnemies);
        GameEventSystem.GetInstance().SubscribeToEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitUnseen), RemoveFromSeenEnemies);

        GameEventSystem.GetInstance().SubscribeToEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitFinishedAction), UnitFinishedAction);
        GameEventSystem.GetInstance().SubscribeToEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitFinishedTurn), UnitFinishedTurn);
    }

    protected virtual void DeinitEvents()
    {
        // Gameplay
        GameEventSystem.GetInstance().UnsubscribeFromEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.TurnStart), TurnStart);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.GameOver), GameOver);

        GameEventSystem.GetInstance().UnsubscribeFromEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitSeen), AddToSeenEnemies);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitUnseen), RemoveFromSeenEnemies);

        GameEventSystem.GetInstance().UnsubscribeFromEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitFinishedAction), UnitFinishedAction);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitFinishedTurn), UnitFinishedTurn);
    }

    protected virtual void Awake()
    {
        // Game Systems Directory
        m_GameSystemsDirectory = GameSystemsDirectory.GetSceneInstance();
        Assert.IsNotNull(m_GameSystemsDirectory, MethodBase.GetCurrentMethod().Name + " - m_GameSystemsDirectory must not be null!");

        // Game Event Names
        m_GameEventNames = m_GameSystemsDirectory.GetGameEventNames();
        Assert.IsNotNull(m_GameEventNames, MethodBase.GetCurrentMethod().Name + " - m_GameEventNames must not be null!");

        // Tile System
        m_TileSystem = m_GameSystemsDirectory.GetTileSystem();
        Assert.IsNotNull(m_TileSystem, MethodBase.GetCurrentMethod().Name + " - TileSystem not found in m_GameSystemsDirectory.");

        // Units Tracker
        m_UnitsTracker = m_GameSystemsDirectory.GetUnitsTracker();
        Assert.IsNotNull(m_UnitsTracker, MethodBase.GetCurrentMethod().Name + " - UnitsTracker not found in m_GameSystemsDirectory.");
    }

    // Gameplay
    protected abstract void GameOver(FactionType _winner);

    protected abstract void TurnStart(FactionType _factionType);

    /// <summary>
    /// To recognize that the unit has already made a move and remove it from the list!
    /// </summary>
    protected abstract void UnitFinishedTurn(UnitStats _unit);

    /// <summary>
    /// Starts polling for user input from GetPlayerInput
    /// </summary>
    protected abstract void UnitFinishedAction(UnitStats _unit);

    /// <summary>
    /// To add the enemy gameobject to global visible enemy list which which will be used at the Attack_Logic
    /// </summary>
    /// <param name="_unit">The enemy unit gameobject</param>
    protected void AddToSeenEnemies(UnitStats _unit)
    {
        if (_unit.UnitFaction != m_ManagedFaction)
        {
            Assert.IsTrue(!m_SeenEnemies.Contains(_unit), "Something is wrong with AddToGlobalVisibilityList");
            m_SeenEnemies.Add(_unit);
        }
    }
    /// <summary>
    /// To remove the enemy gameobject from the global visible enemy list
    /// </summary>
    /// <param name="_go">enemy unit gameobject</param>
    protected void RemoveFromSeenEnemies(UnitStats _unit)
    {
        if (_unit.UnitFaction != m_ManagedFaction)
        {
            Assert.IsTrue(m_SeenEnemies.Contains(_unit), "Something is wrong with RemoveFromGlobalVisibilityList");
            m_SeenEnemies.Remove(_unit);
        }
    }

    protected void GetManagedUnitsFromUnitTracker()
    {
        UnitStats[] aliveManagedUnits = m_UnitsTracker.GetAliveUnits(m_ManagedFaction);
        m_ManagedUnits.Clear();
        for (int i = 0; i < aliveManagedUnits.Length; ++i)
        {
            m_ManagedUnits.Add(aliveManagedUnits[i]);
        }
    }
}