using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// A simper unit action for walking!
/// </summary>
public class UnitWalkAction : IUnitAction {
    [Header("References and variables needed for UnitWalkAction")]
    [Tooltip("The number of tiles it can move")]
    public int m_MovementPoints;
    //[Tooltip("The speed to move from 1 point to another. For animation purpose.")]
    //public float m_Speed = 10.0f;

    [Header("Debugging References for UnitWalkAction")]
    [Tooltip("The array of tiles to move to!")]
    public TileId[] m_TilePath;
    [Tooltip("The current tile that it is moving towards to!")]
    public Tile m_CurrentDestinationTile;
    [SerializeField, Tooltip("The tile system that is needed to be linked as there is no singleton. Right now, the codes is doing the linking for you")]
    public TileSystem m_TileSys;

    public override void StartAction()
    {
        base.StartAction();
        GetUnitStats().CurrentActiveAction = this;
        m_UpdateOfUnitAction = StartCoroutine(UpdateActionRoutine());
    }

    /// <summary>
    /// What is needed here will be the updating of the unit movement and calling for the A* search manager to get a path!.
    /// Followed by the animation!
    /// </summary>
    /// <returns></returns>
    public override IEnumerator UpdateActionRoutine()
    {
        m_ActionState = ActionState.Running;
        // Since the start of the index in the array is 0!
        int zeNumOfIndex = 0;
        bool zeFinishedMoving = true;
        // TODO: use LeanTween to move towards the position for now
        while (zeNumOfIndex < m_TilePath.Length)
        {
            yield return null;
            if (zeFinishedMoving)
            { 
                // Only move when the current state is running!
                switch (m_ActionState)
                {
                    case ActionState.Running:
                        ++zeNumOfIndex;
                        if (zeNumOfIndex >= m_TilePath.Length)
                            break;
                        Tile m_CurrentDestinationTile = m_TileSys.GetTile(m_TilePath[zeNumOfIndex]);
                        Vector3 zeNewPos = new Vector3(m_CurrentDestinationTile.transform.position.x, transform.position.y, m_CurrentDestinationTile.transform.position.z);
                        // TODO: Right now it is using the same terminal velocity to travel between points
                        yield return StartCoroutine(WalkBetPtsRoutine(transform.position, zeNewPos));
                        // Then assign the arrived tile to be the current tile at the unit Stat
                        GetUnitStats().CurrentTileID = m_CurrentDestinationTile.GetId();
                        break;
                    default:
                        break;
                }
            }
        }
        m_UpdateOfUnitAction = null;
        GetUnitStats().CurrentActionPoints--;
        //ObserverSystemScript.Instance.TriggerEvent()
        switch (GetUnitStats().CurrentActionPoints)
        {
            case 0:
                GetUnitStats().ResetUnitStat();
                // tell the player unit manager that it can no longer do any action
                ObserverSystemScript.Instance.StoreVariableInEvent("UnitMakeMove", gameObject);
                ObserverSystemScript.Instance.TriggerEvent("UnitMakeMove");
                break;
            default:
                break;
        }
        GameEventSystem.GetInstance().TriggerEvent("UnitFinishAction");
        m_ActionState = ActionState.Completed;
        yield break;
    }

    // Use this for initialization
    void Start () {
        Assert.IsTrue(m_UnitActionName != "", "No name is given to this action");
        if (!m_TileSys)
            m_TileSys = FindObjectOfType<TileSystem>();
    }

    /// <summary>
    /// This gets stopped by an action called Overwatch. Maybe
    /// </summary>
    public override void StopActionUpdate()
    {
        
    }

    public override void PauseAction()
    {
        m_ActionState = ActionState.Paused;
    }

    public override void ResumeAction()
    {
        m_ActionState = ActionState.Running;
    }

    public override void StopAction()
    {
        if (m_UpdateOfUnitAction != null)
        {
            StopCoroutine(m_UpdateOfUnitAction);
            m_UpdateOfUnitAction = null;
        }
        m_ActionState = ActionState.Completed;
    }

    /// <summary>
    /// A simper update function that only allow the current gameobject move at the same velocity
    /// </summary>
    /// <param name="_StartPt">Starting point</param>
    /// <param name="_EndPt">End Point</param>
    /// <returns></returns>
    protected virtual IEnumerator WalkBetPtsRoutine(Vector3 _StartPt, Vector3 _EndPt)
    {
        PanzerAnimationHandler_Move zeMoveAnim = (PanzerAnimationHandler_Move)m_AnimHandler;
        zeMoveAnim.CompletionCallback += CallAnimDone;
        WaitForFixedUpdate zeFixedUpdateWait = new WaitForFixedUpdate();
        zeMoveAnim.Destination = _EndPt;
        zeMoveAnim.StartAnimation();
        while (m_ActionState != ActionState.Completed && m_ActionState != ActionState.None && !m_AnimDone)
        {
            switch (m_ActionState)
            {
                case ActionState.Running:
                    yield return zeFixedUpdateWait;
                    break;
                default:
                    break;
            }
        }
        zeMoveAnim.CompletionCallback -= CallAnimDone;
        m_AnimDone = false;
        // Sending out an event that this game object has moved
        GameEventSystem.GetInstance().TriggerEvent<GameObject>("UnitMoveToTile", gameObject);
        if (CompletedCallBack != null)
        {
            CompletedCallBack.Invoke();
        }
        yield break;
    }

    protected override void StartTurnCallback()
    {
        throw new System.NotImplementedException();
    }

    protected override void EndTurnCallback()
    {
        throw new System.NotImplementedException();
    }

    protected override void InitializeEvents()
    {
        throw new System.NotImplementedException();
    }

    protected override void DeinitializeEvents()
    {
        throw new System.NotImplementedException();
    }

    public override bool VerifyRunCondition()
    {
        // if there is hardly any tile to move, then ignore it!
        if (m_TilePath.Length <= 1)
            return false;

        return true;
    }

    protected override void OnTurnOn()
    {
        Assert.IsTrue(VerifyRunCondition());
    }
}