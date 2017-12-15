using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ultimate game handler that handles the both.
/// This will be needed to discard for future features like 3 different factions attacking each other!
/// This will instead handle 2 different opposing factions for now.
/// So now it will only be compatible with domination mode.
/// </summary>
public class GameHandler : MonoBehaviour
{
    [Header("Linking references and values needed")]
    [Tooltip("The Player unit manager")]
    public PlayerUnitManager m_PlayerManager;
    [Tooltip("The Enemy unit manager")]
    public EnemyUnitManager m_EnemyManager;
    [Tooltip("The UI for Player winning display")]
    public GameObject m_PlayerWonDisplayGO;
    [Tooltip("The UI for Player losing display")]
    public GameObject m_PlayerLostDisplayGO;
    [Tooltip("The UI to indicate player's turn")]
    public GameObject[] m_PlayerTurnDisplay;
    [Tooltip("The UI to indicate enemy's turn")]
    public GameObject m_EnemyTurnDisplay;

    [Header("Shown In Inspector For Debugging Purposes.")]
    [SerializeField, Tooltip("To see whose turn it is. The initial variable can be changed to decide who starts first.")]
    protected bool m_PlayerTurn = true;

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
                foreach (GameObject display in m_PlayerTurnDisplay)
                {
                    display.SetActive(true);
                }
                m_EnemyTurnDisplay.SetActive(false);
            }
            else
            {
                m_EnemyManager.StartCoroutine(m_EnemyManager.IterateThroughEnemyUpdate());
                foreach (GameObject display in m_PlayerTurnDisplay)
                    {
                        display.SetActive(false);
                    }
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
        yield return null;
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
        m_PlayerWonDisplayGO.SetActive(true);
        foreach (GameObject zeDisplay in m_PlayerTurnDisplay)
        {
            zeDisplay.SetActive(false);
        }
        m_EnemyTurnDisplay.SetActive(false);
    }

    protected void DisplayLoseScreen()
    {
        m_PlayerLostDisplayGO.SetActive(true);
        foreach (GameObject zeDisplay in m_PlayerTurnDisplay)
        {
            zeDisplay.SetActive(false);
        }
        m_EnemyTurnDisplay.SetActive(false);
    }
}