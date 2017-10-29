using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simper unit action for walking!
/// </summary>
public class UnitWalkAction : UnitAction {
    [Header("References and variables needed for unit walk")]
    [Tooltip("The number of tiles it can move")]
    public int m_MovementPoints;
    [Tooltip("The speed to move from 1 point to another. For animation purpose.")]
    public float m_Speed = 10.0f;

    [Header("Debugging References for Unit Walk")]
    [Tooltip("The array of tiles to move to!")]
    public TileId[] m_TilePath;
    [Tooltip("The current tile that it is moving towards to!")]
    public Tile m_CurrentDestinationTile;
    [SerializeField, Tooltip("The tile system that is needed to be linked as there is no singleton. Right now, the codes is doing the linking for you")]
    public TileSystem m_TileSys;

    public override bool UseAction()
    {
        m_UpdateOfUnitAction = StartCoroutine(UpdateActionRoutine());
        return true;
    }

    /// <summary>
    /// What is needed here will be the updating of the unit movement and calling for the A* search manager to get a path!.
    /// Followed by the animation!
    /// </summary>
    /// <returns></returns>
    public override IEnumerator UpdateActionRoutine()
    {
        // Since the start of the index in the array is 0!
        int zeNumOfIndex = 0;
        bool zeFinishedMoving = false;
        m_CurrentDestinationTile = m_TileSys.GetTile(m_TilePath[zeNumOfIndex]);
        Vector3 zeNewPos = new Vector3(m_CurrentDestinationTile.transform.position.x, transform.position.y, m_CurrentDestinationTile.transform.position.z);
        // TODO: use LeanTween to move towards the position for now
        LeanTween.move(gameObject, zeNewPos, 0.5f).setOnComplete(() => zeFinishedMoving = true);
        while (zeNumOfIndex < m_TilePath.Length)
        {
            yield return null;
            if (zeFinishedMoving)
            {
                ++zeNumOfIndex;
                if (zeNumOfIndex >= m_TilePath.Length)
                    break;
                m_CurrentDestinationTile = m_TileSys.GetTile(m_TilePath[zeNumOfIndex]);
                zeNewPos = new Vector3(m_CurrentDestinationTile.transform.position.x, transform.position.y, m_CurrentDestinationTile.transform.position.z);
                // TODO: use LeanTween to move towards the position for now
                LeanTween.move(gameObject, zeNewPos, 0.5f).setOnComplete(() => zeFinishedMoving = true);
            }
        }
        m_UpdateOfUnitAction = null;
        m_UnitStatGO.m_UnitStatsJSON.CurrentActionPoints--;
        //ObserverSystemScript.Instance.TriggerEvent()
        switch (m_UnitStatGO.m_UnitStatsJSON.CurrentActionPoints)
        {
            case 0:
                m_UnitStatGO.ResetUnitStat();
                // tell the player unit manager that it can no longer do any action
                ObserverSystemScript.Instance.StoreVariableInEvent("UnitMakeMove", gameObject);
                ObserverSystemScript.Instance.TriggerEvent("UnitMakeMove");
                break;
            default:
                break;
        }
        ObserverSystemScript.Instance.TriggerEvent("UnitFinishAction");
        yield break;
    }

    // Use this for initialization
    void Start () {
        if (m_UnitActionName == null)
        {
            m_UnitActionName = "Walk";
        }
        else
        {
            // Just in case the action name is not included!
            switch (m_UnitActionName.Length)
            {
                case 0:
                    m_UnitActionName = "Walk";
                    break;
                default:
                    break;
            }
        }
        if (!m_TileSys)
            m_TileSys = FindObjectOfType<TileSystem>();
    }

    /// <summary>
    /// This gets stopped by an action called Overwatch. Maybe
    /// </summary>
    public override void StopActionUpdate()
    {
        base.StopActionUpdate();
    }
}
