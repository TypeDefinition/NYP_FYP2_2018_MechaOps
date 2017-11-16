#define GOAP_AI
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Mainly to iterate and update all of the enemy units 1 by 1
/// </summary>
public class EnemyUnitManager : MonoBehaviour {
    [Header("Debugging purposes")]
    [SerializeField, Tooltip("The list of available units when it begins")]
    protected List<GameObject> m_EnemyList;
    [SerializeField, Tooltip("Tile marker where all of the player units last gathered")]
    protected TileId m_TilePlayerUnits;

    public static EnemyUnitManager Instance
    {
        private set; get;
    }

    /// <summary>
    /// The Update of this manager
    /// </summary>
    Coroutine m_UpdateOfManager;
    /// <summary>
    /// The individual update of the unit
    /// </summary>
    Coroutine m_UpdateOfEnemy;

    public TileId TilePlayerUnits
    {
        get
        {
            return m_TilePlayerUnits;
        }
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
        ObserverSystemScript.Instance.SubscribeEvent("PlayerAnnihilated", StopUpdate);
        ObserverSystemScript.Instance.SubscribeEvent("EnemyAnnihilated", StopUpdate);
    }

    private void OnDisable()
    {
        ObserverSystemScript.Instance.UnsubscribeEvent("PlayerAnnihilated", StopUpdate);
        ObserverSystemScript.Instance.UnsubscribeEvent("EnemyAnnihilated", StopUpdate);
    }

    /// <summary>
    /// The coroutine to update the manager!
    /// </summary>
    /// <returns></returns>
    public IEnumerator IterateThroughEnemyUpdate()
    {
        m_EnemyList = new List<GameObject>(KeepTrackOfUnits.Instance.m_AllEnemyUnitGO);
#if GOAP_AI
        foreach (GameObject zeEnemy in m_EnemyList)
        {
            GoapPlanner zePlanner = zeEnemy.GetComponent<GoapPlanner>();
            m_UpdateOfEnemy = StartCoroutine(zePlanner.StartPlanning());
            // wait till the update is finish then proceed to the next unit
            yield return m_UpdateOfEnemy;
        }
#else
#endif
        m_UpdateOfManager = null;
        ObserverSystemScript.Instance.TriggerEvent("TurnEnded");
        print("Finish Enemy Manager turn");
        yield break;
    }

    protected void StopUpdate()
    {
        if (m_UpdateOfManager != null)
        {
            StopCoroutine(m_UpdateOfManager);
            m_UpdateOfManager = null;
        }
        if (m_UpdateOfEnemy != null)
        {
            StopCoroutine(m_UpdateOfEnemy);
            m_UpdateOfEnemy = null;
        }
    }

    /// <summary>
    /// Meant to re-update the position of the player units gathered if the enemy unit has reached the current marker
    /// </summary>
    public void UpdateMarker()
    {
        // TODO: improve this function!
        m_TilePlayerUnits = KeepTrackOfUnits.Instance.m_AllPlayerUnitGO[0].GetComponent<UnitStats>().CurrentTileID;
    }
}
