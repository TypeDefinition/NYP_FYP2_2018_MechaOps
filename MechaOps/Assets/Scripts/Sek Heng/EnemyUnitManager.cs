using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitManager : MonoBehaviour {

    /// <summary>
    /// The Update of this manager
    /// </summary>
    Coroutine m_UpdateOfManager;

	// Use this for initialization
	void Start () {
		
	}

    /// <summary>
    /// The coroutine to update the manager!
    /// </summary>
    /// <returns></returns>
    IEnumerator IterateThroughEnemyUpdate()
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
