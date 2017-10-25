using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ultimate game handler that handles the both.
/// This will be needed to discard for future features like 3 different factions attacking each other!
/// This will instead handle 2 different opposing factions for now.
/// So now it will only be compatible with domination mode.
/// </summary>
public class GameHandler : MonoBehaviour {
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

    [Header("Debugging purpose")]
    [SerializeField, Tooltip("To see who's is it in. The initial variable can be changed to decide who start first")]
    protected bool m_PlayerTurn = true;

    /// <summary>
    /// So that the setter can used easily.
    /// This needs to be changed in the future for expandability.
    /// </summary>
    public bool PlayerTurn
    {
        set
        {
            m_PlayerTurn = value;
            switch (m_PlayerTurn)
            {
                case true:
                    // This weird way of start coroutine is to make sure they only update themselves!
                    m_PlayerManager.StartCoroutine(m_PlayerManager.BeginUpdateOfPlayerUnits());
                    //m_PlayerTurnDisplay.SetActive(true);
                    foreach (GameObject zeDisplay in m_PlayerTurnDisplay)
                    {
                        zeDisplay.SetActive(true);
                    }
                    m_EnemyTurnDisplay.SetActive(false);
                    break;
                default:
                    m_EnemyManager.StartCoroutine(m_EnemyManager.IterateThroughEnemyUpdate());
                    foreach (GameObject zeDisplay in m_PlayerTurnDisplay)
                    {
                        zeDisplay.SetActive(false);
                    }
                    m_EnemyTurnDisplay.SetActive(true);
                    break;
            }
        }
        get
        {
            return m_PlayerTurn;
        }
    }

    private void OnEnable()
    {
        ObserverSystemScript.Instance.SubscribeEvent("PlayerAnnihilated", DisplayLosingScreenForPlayer);
        ObserverSystemScript.Instance.SubscribeEvent("EnemyAnnihilated", DisplayWinningScreenForPlayer);
        // Lamda function will need further testing. So this is for guinea pig experimentation!
        ObserverSystemScript.Instance.SubscribeEvent("TurnEnded", () => PlayerTurn = !PlayerTurn);
    }

    // Use this for initialization
    void OnDisable () {
        ObserverSystemScript.Instance.SubscribeEvent("PlayerAnnihilated", DisplayLosingScreenForPlayer);
        ObserverSystemScript.Instance.SubscribeEvent("EnemyAnnihilated", DisplayWinningScreenForPlayer);
        ObserverSystemScript.Instance.SubscribeEvent("TurnEnded", () => PlayerTurn = !PlayerTurn);
    }

    private IEnumerator Start()
    {
        // Maybe there will be a introduction or something thus this is a coroutine to delay the start
        // be lazy and reuse the same variable lol
        PlayerTurn = m_PlayerTurn;
        yield break;
    }

    /// <summary>
    /// Displaying the winning UI for player
    /// </summary>
    void DisplayWinningScreenForPlayer()
    {
        m_PlayerWonDisplayGO.SetActive(true);
        foreach (GameObject zeDisplay in m_PlayerTurnDisplay)
        {
            zeDisplay.SetActive(false);
        }
        m_EnemyTurnDisplay.SetActive(false);
    }

    void DisplayLosingScreenForPlayer()
    {
        m_PlayerLostDisplayGO.SetActive(true);
        foreach (GameObject zeDisplay in m_PlayerTurnDisplay)
        {
            zeDisplay.SetActive(false);
        }
        m_EnemyTurnDisplay.SetActive(false);
    }
}
