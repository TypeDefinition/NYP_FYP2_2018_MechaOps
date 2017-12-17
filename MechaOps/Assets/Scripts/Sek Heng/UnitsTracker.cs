using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A quick class to store an array of gameobject
/// </summary>
public class UnitsTracker : MonoBehaviour
{
    [Header("Shown in Inspector for debugging purposes.")]

    [Tooltip("The list of player units THAT ARE FUCKING ALIVE!")]
    public List<GameObject> m_AlivePlayerUnits = null;
    [Tooltip("The list of enemy units THAT ARE FUCKING ALIVE!")]
    public List<GameObject> m_AliveEnemyUnits = null;

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
    void Awake ()
    {
        m_AlivePlayerUnits = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        m_AliveEnemyUnits = new List<GameObject>(GameObject.FindGameObjectsWithTag("EnemyUnit"));
    }

    /// <summary>
    /// A quick function that iterates and keep track of any player units died.
    /// Removes the player unit from the list.
    /// </summary>
    void SignalPlayerUnitDied(GameObject _deadGO)
    {
        m_AlivePlayerUnits.Remove(_deadGO);
        // Send a notification once all of the player's units die
        if (m_AlivePlayerUnits.Count == 0)
        {
            GameEventSystem.GetInstance().TriggerEvent("PlayerAnnihilated");
        }
    }

    /// <summary>
    /// A quick function that iterates and keep track of any enemy units died.
    /// Removes the enemy unit from the list.
    /// </summary>
    void SignalAnEnemyDied(GameObject _deadGO)
    {
        m_AliveEnemyUnits.Remove(_deadGO);
        // Send a notification if all of the enemy units die.
        if (m_AliveEnemyUnits.Count == 0)
        {
            GameEventSystem.GetInstance().TriggerEvent("EnemyAnnihilated");
        }
    }
}