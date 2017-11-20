using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanzerAnimationTester : MonoBehaviour
{
    [SerializeField] private GameObject m_Target = null;
    [SerializeField] private bool m_Hit = false;

    private MOAnimation_PanzerAttack m_AnimationAttack;
    private MOAnimation_PanzerMove m_AnimationMove;
    private MOAnimation_UnitDestroy m_AnimationDestroy;

    // Use this for initialization
    void Start () {
        m_AnimationAttack = gameObject.GetComponent<MOAnimation_PanzerAttack>();
        m_AnimationAttack.CompletionCallback = this.AttackCompletionCallback;

        m_AnimationMove = gameObject.GetComponent<MOAnimation_PanzerMove>();
        m_AnimationMove.CompletionCallback = this.MoveCompletionCallback;

        m_AnimationDestroy = gameObject.GetComponent<MOAnimation_UnitDestroy>();
        // Destroy Animation never ends. Hence it has no use for a callback.
    }

    public void StartAttackAnimation()
    {
        m_AnimationAttack.Target = m_Target;
        m_AnimationAttack.Hit = m_Hit;
        m_AnimationAttack.StartAnimation();
    }

    public void StopAttackAnimation()
    {
        m_AnimationAttack.StopAnimation();
    }

    public void StartMoveAnimation()
    {
        m_AnimationMove.Destination = m_Target.transform.position;
        m_AnimationMove.StartAnimation();
    }

    public void StopMoveAnimation()
    {
        m_AnimationMove.StopAnimation();
    }

    public void StartDestroyAnimation()
    {
        m_AnimationDestroy.StartAnimation();
    }

    public void StopDestroyAnimation()
    {
        m_AnimationDestroy.StopAnimation();
    }

    void AttackCompletionCallback()
    {
        Debug.Log("Attack Animation Completed!");
    }

    void MoveCompletionCallback()
    {
        Debug.Log("Move Animation Completed!");
    }

}