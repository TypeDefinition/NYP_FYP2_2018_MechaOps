using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// The ultimate game handler that handles the both.
/// This will be needed to discard for future features like 3 different factions attacking each other!
/// This will instead handle 2 different opposing factions for now.
/// So now it will only be compatible with domination mode.
/// </summary>
[DisallowMultipleComponent]
public class GameFlowManager : MonoBehaviour
{
    // Non-Serialised Variable(s)
    protected GameObject m_EnemyTurnDisplay;
    protected bool m_IsGameOver = false;

    // Serialised Variable(s)
    [SerializeField]
    protected FactionType m_PlayerFaction = FactionType.None;
    [SerializeField, Tooltip("These are the factions that will get a turn.")]
    FactionType[] m_FactionsInPlay = new FactionType[0];
    [SerializeField, Tooltip("To see whose turn it is. The initial variable can be changed to decide who starts first.")]
    protected int m_CurrentTurnFaction = 0;

    [SerializeField, Tooltip("The UI for Player winning display prefab")]
    protected GameObject m_PlayerWonDisplay;
    [SerializeField, Tooltip("The UI for Player losing display prefab")]
    protected GameObject m_PlayerLostDisplay;
    [SerializeField, Tooltip("The UI to indicate enemy's turn prefab")]
    protected GameObject m_EnemyTurnDisplayPrefab;
    [SerializeField, Tooltip("Game Over Display UI to go back to main menu or retry prefab")]
    protected GameObject m_GameOverDisplayPrefab;
    [SerializeField] protected GameEventNames m_GameEventNames = null;

    /// <summary>
    /// So that the setter can used easily.
    /// This needs to be changed in the future for expandability.
    /// </summary>
    public FactionType CurrentTurnFaction
    {
        get { return m_FactionsInPlay[m_CurrentTurnFaction]; }
    }

    public bool IsGameOver()
    {
        return m_IsGameOver;
    }

    protected virtual void InitEvents()
    {
        GameEventSystem.GetInstance().SubscribeToEvent(m_GameEventNames.GetEventName(GameEventNames.SpawnSystemNames.UnitsSpawned), OnUnitsSpawned);
        GameEventSystem.GetInstance().SubscribeToEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.FactionDead), OnFactionDead);
        GameEventSystem.GetInstance().SubscribeToEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.TurnStart), OnTurnStart);
        GameEventSystem.GetInstance().SubscribeToEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.TurnEnd), OnTurnEnd);
    }

    protected virtual void DeinitEvents()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent(m_GameEventNames.GetEventName(GameEventNames.SpawnSystemNames.UnitsSpawned), OnUnitsSpawned);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.FactionDead), OnFactionDead);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.TurnStart), OnTurnStart);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.TurnEnd), OnTurnEnd);
    }

    protected virtual void Awake()
    {
        m_EnemyTurnDisplay = Instantiate(m_EnemyTurnDisplayPrefab, GameSystemsDirectory.GetSceneInstance().GetScreenSpaceCanvas().transform);
        m_EnemyTurnDisplay.SetActive(false);

        Assert.IsTrue(m_FactionsInPlay.Length >= 2, MethodBase.GetCurrentMethod().Name + " - There must be at least 2 factions in play!");

        InitEvents();
    }

    protected virtual void OnDestroy()
    {
        DeinitEvents();
    }

    // Gameplay
    /// <summary>
    /// This function signals the GameFlowManager to begin the game.
    /// </summary>
    protected virtual void OnUnitsSpawned()
    {
        GameEventSystem.GetInstance().TriggerEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.TurnStart), CurrentTurnFaction);
    }

    protected virtual void OnTurnStart(FactionType _factionType)
    {
        m_EnemyTurnDisplay.SetActive(CurrentTurnFaction != m_PlayerFaction);
    }

    /// <summary>
    /// This function signals the end of the turn, and to begin the next turn.
    /// </summary>
    /// <param name="_factionType"></param>
    protected virtual void OnTurnEnd(FactionType _factionType)
    {
        SetNextTurnFaction();
        GameEventSystem.GetInstance().TriggerEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.TurnStart), CurrentTurnFaction);
    }

    protected virtual void SetNextTurnFaction()
    {
        m_CurrentTurnFaction = (m_CurrentTurnFaction + 1) % m_FactionsInPlay.Length;
    }

    protected virtual void OnFactionDead(FactionType _factionType)
    {
        FactionType[] aliveFactions = GameSystemsDirectory.GetSceneInstance().GetUnitsTracker().GetAliveFactions();

        // It's a draw. Nobody is alive.
        if (aliveFactions.Length == 0)
        {
            m_IsGameOver = true;
            // Treat a draw as a lost.
            DisplayLoseScreen();
            GameEventSystem.GetInstance().TriggerEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.GameOver), FactionType.None);
        }
        // Only 1 faction is left alive. It is the winner.
        if (aliveFactions.Length == 1)
        {
            m_IsGameOver = true;
            if (aliveFactions[0] == m_PlayerFaction)
            {
                DisplayWinScreen();
            }
            else
            {
                DisplayLoseScreen();
            }
            GameEventSystem.GetInstance().TriggerEvent<FactionType>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.GameOver), aliveFactions[0]);
        }
    }

    /// <summary>
    /// Displaying the winning UI for player
    /// </summary>
    protected virtual void DisplayWinScreen()
    {
        m_EnemyTurnDisplay.SetActive(false);
        Instantiate(m_PlayerWonDisplay, GameSystemsDirectory.GetSceneInstance().GetScreenSpaceCanvas().transform).SetActive(true);
        Instantiate(m_GameOverDisplayPrefab, GameSystemsDirectory.GetSceneInstance().GetScreenSpaceCanvas().transform).SetActive(true);
    }

    protected virtual void DisplayLoseScreen()
    {
        m_EnemyTurnDisplay.SetActive(false);
        Instantiate(m_PlayerLostDisplay, GameSystemsDirectory.GetSceneInstance().GetScreenSpaceCanvas().transform).SetActive(true);
        Instantiate(m_GameOverDisplayPrefab, GameSystemsDirectory.GetSceneInstance().GetScreenSpaceCanvas().transform).SetActive(true);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        m_CurrentTurnFaction = Mathf.Max(m_CurrentTurnFaction, 0);
        if (m_FactionsInPlay.Length > 0)
        {
            m_CurrentTurnFaction %= m_FactionsInPlay.Length;
        }
        else
        {
            m_CurrentTurnFaction = 0;
        }
    }
#endif
}