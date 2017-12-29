using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Making all enemy bvisible. dont use it in actual gameplay!
/// </summary>
public class SetAllEnemyVisible : MonoBehaviour {
    private void OnEnable()
    {
        GameEventSystem.GetInstance().SubscribeToEvent("GameStart", SetEnemyUnitsVisible);
    }

    void SetEnemyUnitsVisible()
    {
        GameObject[] AllEnemyUnitsGO = GameObject.FindGameObjectsWithTag("EnemyUnit");
        foreach (GameObject EnemyGO in AllEnemyUnitsGO)
        {
            EnemyGO.GetComponent<ViewScript>().IncreaseVisibility();
            EnemyGO.GetComponent<ViewScript>().IncreaseVisibility();
        }
    }

    private void OnDisable()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent("GameStart", SetEnemyUnitsVisible);
    }
}
