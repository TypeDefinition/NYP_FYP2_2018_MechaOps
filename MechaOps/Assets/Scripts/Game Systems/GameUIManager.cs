using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ultimate game handler that handles the both.
/// This will be needed to discard for future features like 3 different factions attacking each other!
/// This will instead handle 2 different opposing factions for now.
/// So now it will only be compatible with domination mode.
/// </summary>
[DisallowMultipleComponent]
public class GameUIManager : MonoBehaviour
{
    [Header("Variables required")]
    [SerializeField, Tooltip("The Player unit manager")]
    protected PlayerUnitsManager m_PlayerManager;
    [SerializeField, Tooltip("The Enemy unit manager")]
    protected EnemyUnitsManager m_EnemyManager;
    [SerializeField, Tooltip("The UI for Player winning display prefab")]
    protected GameObject m_PlayerWonDisplay;
    [SerializeField, Tooltip("The UI for Player losing display prefab")]
    protected GameObject m_PlayerLostDisplay;
    [SerializeField, Tooltip("The UI to indicate enemy's turn prefab")]
    protected GameObject m_EnemyTurnDisplay;
    [SerializeField, Tooltip("Game Over Display UI to go back to main menu or retry prefab")]
    protected GameObject m_GameOverDisplay;
    [SerializeField, Tooltip("Game System Directory")]
    protected GameSystemsDirectory m_GameSystemDirectory;

    [Header("Shown In Inspector For Debugging Purposes.")]
    [SerializeField, Tooltip("To see whose turn it is. The initial variable can be changed to decide who starts first.")]
    protected bool m_PlayerTurn = true;
    [SerializeField, Tooltip("Instantiated Enemy Turn Display")]
    protected GameObject m_InstantiateEnemyTurnGO;

    /// <summary>
    /// So that the setter can used easily.
    /// This needs to be changed in the future for expandability.
    /// </summary>
    public bool PlayerTurn
    {
        set
        {
            if ((m_PlayerTurn = value))
            {
                // This weird way of start coroutine is to make sure they only update themselves!
                m_PlayerManager.StartCoroutine(m_PlayerManager.BeginUpdateOfPlayerUnits());
                m_EnemyTurnDisplay.SetActive(false);
            }
            else
            {
                m_EnemyManager.StartCoroutine(m_EnemyManager.IterateThroughEnemyUpdate());
                m_EnemyTurnDisplay.SetActive(true);
            }
        }
        get
        {
            return m_PlayerTurn;
        }
    }

    private void InitEvents()
    {
        GameEventSystem.GetInstance().SubscribeToEvent("PlayerAnnihilated", DisplayLoseScreen);
        GameEventSystem.GetInstance().SubscribeToEvent("EnemyAnnihilated", DisplayWinScreen);
        GameEventSystem.GetInstance().SubscribeToEvent("TurnEnded", TurnEnded);
    }

    private void DeinitEvents()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent("PlayerAnnihilated", DisplayLoseScreen);
        GameEventSystem.GetInstance().UnsubscribeFromEvent("EnemyAnnihilated", DisplayWinScreen);
        GameEventSystem.GetInstance().UnsubscribeFromEvent("TurnEnded", TurnEnded);
    }

    protected void OnEnable()
    {
        InitEvents();
    }

    // Use this for initialization
    protected void OnDisable ()
    {
        DeinitEvents();
    }

    protected void TurnEnded()
    {
        PlayerTurn = !PlayerTurn;
    }

    protected IEnumerator Start()
    {
        // Instantiate the variables of the displays!
        m_InstantiateEnemyTurnGO = Instantiate(m_EnemyTurnDisplay, m_GameSystemDirectory.GetScreenSpaceCanvas().transform);
        m_InstantiateEnemyTurnGO.SetActive(false);
        yield return null;
        GameEventSystem.GetInstance().TriggerEvent("GameStart");
        // Maybe there will be a introduction or something thus this is a coroutine to delay the start
        // be lazy and reuse the same variable lol
        PlayerTurn = m_PlayerTurn;
        yield break;
    }

    /// <summary>
    /// Displaying the winning UI for player
    /// </summary>
    protected void DisplayWinScreen()
    {
        Instantiate(m_PlayerWonDisplay, m_GameSystemDirectory.GetScreenSpaceCanvas().transform);
        m_EnemyTurnDisplay.SetActive(false);
        Instantiate(m_GameOverDisplay, m_GameSystemDirectory.GetScreenSpaceCanvas().transform);
    }

    protected void DisplayLoseScreen()
    {
        Instantiate(m_PlayerLostDisplay, m_GameSystemDirectory.GetScreenSpaceCanvas().transform);
        m_EnemyTurnDisplay.SetActive(false);
        Instantiate(m_GameOverDisplay, m_GameSystemDirectory.GetScreenSpaceCanvas().transform);
    }
}