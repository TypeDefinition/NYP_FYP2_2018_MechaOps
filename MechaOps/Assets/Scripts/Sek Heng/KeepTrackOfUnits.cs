using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A quick class to store an array of gameobject
/// </summary>
public class KeepTrackOfUnits : MonoBehaviour {
    [Header("Debugging purpose")]
    [Tooltip("The list of player units!")]
    public List<GameObject> m_AllPlayerUnitGO;
    [Tooltip("The list of enemy units")]
    public List<GameObject> m_AllEnemyUnitGO;

    public static KeepTrackOfUnits Instance
    {
        get; private set;
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        ObserverSystemScript.Instance.SubscribeEvent("EnemyUnitIsDead", SignalAnEnemyDied);
        ObserverSystemScript.Instance.SubscribeEvent("PlayerIsDead", SignalPlayerUnitDied);
    }

    private void OnDisable()
    {
        ObserverSystemScript.Instance.UnsubscribeEvent("EnemyUnitIsDead", SignalAnEnemyDied);
        ObserverSystemScript.Instance.UnsubscribeEvent("PlayerIsDead", SignalPlayerUnitDied);
    }

    // Use this for initialization
    void Start () {
        m_AllPlayerUnitGO = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        m_AllEnemyUnitGO = new List<GameObject>(GameObject.FindGameObjectsWithTag("EnemyUnit"));
    }

    /// <summary>
    /// A quick function that iterates and keep track of any enemy units died.
    /// Removes the enemy unit from the list.
    /// </summary>
    void SignalAnEnemyDied()
    {
        GameObject theMessageGO = ObserverSystemScript.Instance.GetStoredEventVariable<GameObject>("EnemyUnitIsDead");
        m_AllEnemyUnitGO.Remove(theMessageGO);
        ObserverSystemScript.Instance.removeTheEventVariableNextFrame("EnemyUnitIsDead");
    }

    /// <summary>
    /// A quick function that iterates and keep track of any player units died.
    /// Removes the player unit from the list.
    /// </summary>
    void SignalPlayerUnitDied()
    {
        GameObject theMessageGO = ObserverSystemScript.Instance.GetStoredEventVariable<GameObject>("PlayerIsDead");
        m_AllPlayerUnitGO.Remove(theMessageGO);
        ObserverSystemScript.Instance.removeTheEventVariableNextFrame("PlayerIsDead");
    }
}
