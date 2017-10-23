using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitManager : MonoBehaviour {

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

    // Use this for initialization
    void Start () {
		
	}

    IEnumerator BeginUpdateOfPlayerUnits()
    {
        m_UpdateOfManager = null;
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
