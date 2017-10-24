using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ultimate game handler that handles the both.
/// This will be needed to discard for future features like 3 different factions attacking each other!
/// This will instead handle 2 different opposing factions for now.
/// </summary>
public class GameHandler : MonoBehaviour {
    [Header("Linking references and values needed")]
    [Tooltip("The Player unit manager")]
    public PlayerUnitManager m_PlayerManager;
    [Tooltip("The Enemy unit manager")]
    public EnemyUnitManager m_EnemyManager;

    [Header("Debugging purpose")]
    [SerializeField, Tooltip("To see who's is it in. The initial variable can be changed to decide who start first")]
    protected bool m_PlayerTurn = true;

    /// <summary>
    /// So that the setter can used easily.
    /// This needs to be changed in the future for expandability
    /// </summary>
    public bool PlayerTurn
    {
        set
        {
            m_PlayerTurn = value;
            switch (m_PlayerTurn)
            {
                case true:
                    m_PlayerManager.StartCoroutine(m_PlayerManager.BeginUpdateOfPlayerUnits());
                    break;
                case false:
                    m_EnemyManager.StartCoroutine(m_EnemyManager.IterateThroughEnemyUpdate());
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
        ObserverSystemScript.Instance.SubscribeEvent("PlayerAnnihilated", DisplayWinningScreenForPlayer);
        ObserverSystemScript.Instance.SubscribeEvent("EnemyAnnihilated", DisplayLosingScreenForPlayer);
        // Lamda function will need further testing. So this is for guinea pig experimentation!
        ObserverSystemScript.Instance.SubscribeEvent("TurnEnded", () => PlayerTurn = !PlayerTurn);
    }

    // Use this for initialization
    void OnDisable () {
        ObserverSystemScript.Instance.SubscribeEvent("PlayerAnnihilated", DisplayWinningScreenForPlayer);
        ObserverSystemScript.Instance.SubscribeEvent("EnemyAnnihilated", DisplayLosingScreenForPlayer);
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

    }

    void DisplayLosingScreenForPlayer()
    {

    }
}
