using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitManager : MonoBehaviour {
    [SerializeField, Tooltip("The array of how many units have yet to make their turn. Meant for debugging purpose")]
    protected List<GameObject> m_UnitsYetToMakeMoves;

    /// <summary>
    /// The update of this manager. So that it can be controlled anytime
    /// </summary>
    Coroutine m_UpdateOfManager;

    private void OnEnable()
    {
        ObserverSystemScript.Instance.SubscribeEvent("PlayerAnnihilated", StopUpdate);
        ObserverSystemScript.Instance.SubscribeEvent("EnemyAnnihilated", StopUpdate);
    }

    private void OnDisable()
    {
        ObserverSystemScript.Instance.UnsubscribeEvent("PlayerAnnihilated", StopUpdate);
        ObserverSystemScript.Instance.UnsubscribeEvent("EnemyAnnihilated", StopUpdate);
    }

    public IEnumerator BeginUpdateOfPlayerUnits()
    {
        // Get a shallow copy of the list of all available units!
        m_UnitsYetToMakeMoves = new List<GameObject>(KeepTrackOfUnits.Instance.m_AllPlayerUnitGO);
        while (m_UnitsYetToMakeMoves.Count > 0)
        {
            yield return null;
        }
        m_UpdateOfManager = null;
        ObserverSystemScript.Instance.TriggerEvent("TurnEnded");
        yield break;
    }
	
    void StopUpdate()
    {
        if (m_UpdateOfManager != null)
        {
            StopCoroutine(m_UpdateOfManager);
            m_UpdateOfManager = null;
        }
    }
}
