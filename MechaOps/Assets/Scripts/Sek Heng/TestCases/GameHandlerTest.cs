#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To be used only for testing out AI or different scene.
/// Not to be used at actual gameplay!
/// </summary>
public class GameHandlerTest : GameUIManager {
    [Header("Debugging for GameHandlerTest")]
    [Tooltip("How many times should the enemy unit manager runs before player unit manager")]
    public int m_EnemyUnitManagerRuns = 3;
    [Tooltip("the counter for how many times it run")]
    public int m_Counter = 0;

    protected new void OnEnable()
    {
        base.OnEnable();
        // we need to unsubscribe from it
        GameEventSystem.GetInstance().UnsubscribeFromEvent("TurnEnded", TurnEnded);
        GameEventSystem.GetInstance().SubscribeToEvent("TurnEnded", EnemyTurnEnded);
        m_Counter = m_EnemyUnitManagerRuns;
    }

    protected new void OnDisable()
    {
        base.OnDisable();
    }

    protected void EnemyTurnEnded()
    {
        switch (m_PlayerTurn)
        {
            case false:
                // because this is only meant for the enemy unit manager
                --m_Counter;
                if (m_Counter <= 0)
                {
                    PlayerTurn = true;
                    m_Counter = m_EnemyUnitManagerRuns;
                }
                PlayerTurn = false;
                break;
            default:
                PlayerTurn = !PlayerTurn;
                break;
        }
    }
}
#endif