using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A quick class to store an array of gameobject
/// </summary>
public class UnitsTracker : MonoBehaviour
{
    [Header("Shown in Inspector for debugging purposes.")]
    [Tooltip("The list of player units!")]
    public List<GameObject> m_AllPlayerUnitGO;
    [Tooltip("The list of enemy units")]
    public List<GameObject> m_AllEnemyUnitGO;

    private void OnEnable()
    {
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("EnemyUnitIsDead", SignalAnEnemyDied);
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("PlayerIsDead", SignalAnEnemyDied);
    }

    private void OnDisable()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("EnemyUnitIsDead", SignalAnEnemyDied);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("PlayerIsDead", SignalAnEnemyDied);
    }

    // Use this for initialization
    void Awake () {
        m_AllPlayerUnitGO = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        m_AllEnemyUnitGO = new List<GameObject>(GameObject.FindGameObjectsWithTag("EnemyUnit"));
    }

    /// <summary>
    /// A quick function that iterates and keep track of any player units died.
    /// Removes the player unit from the list.
    /// </summary>
    void SignalPlayerUnitDied(GameObject _deadGO)
    {
        m_AllPlayerUnitGO.Remove(_deadGO);
        // Send a notification once all of the player's units die
        switch (m_AllPlayerUnitGO.Count)
        {
            case 0:
                GameEventSystem.GetInstance().TriggerEvent("PlayerAnnihilated");
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// A quick function that iterates and keep track of any enemy units died.
    /// Removes the enemy unit from the list.
    /// </summary>
    void SignalAnEnemyDied(GameObject _deadGO)
    {
        m_AllEnemyUnitGO.Remove(_deadGO);
        // Send a notification if all of the enemy units die.
        switch (m_AllEnemyUnitGO.Count)
        {
            case 0:
                GameEventSystem.GetInstance().TriggerEvent("EnemyAnnihilated");
                break;
            default:
                break;
        }
    }
}
