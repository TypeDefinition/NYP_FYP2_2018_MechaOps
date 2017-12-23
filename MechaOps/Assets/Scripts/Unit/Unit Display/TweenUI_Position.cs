using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It will only tween the position from 1 point to another
/// </summary>
public class TweenUI_Position : MonoBehaviour {
    [Header("Variables for TweenUI_Position")]
    [SerializeField, Tooltip("Animate Time")]
    protected float m_AnimatePositionTime = 0.2f;
    [SerializeField, Tooltip("Position to move towards to")]
    protected Vector3 m_PositionToMoveTo;

    [Header("Debugging for TweenUI_Position")]
    [SerializeField, Tooltip("Original position")]
    protected Vector3 m_OriginalPosition;
    [SerializeField, Tooltip("Flag to ensure that it will not lag out!")]
    protected bool m_CompletedFlag = true;
    [SerializeField, Tooltip("Flag to ensure that the inversion will work as intended")]
    protected bool m_AtOriginalPosition = true;

    protected virtual void Awake()
    {
        m_OriginalPosition = transform.position;
    }

    /// <summary>
    /// To move towards that position
    /// </summary>
    public virtual void DoAnimatePosition()
    {
        if (m_CompletedFlag)
        {
            LeanTween.move(gameObject, m_PositionToMoveTo, m_AnimatePositionTime).setOnComplete(CompleteTweening);
            m_CompletedFlag = false;
            m_AtOriginalPosition = false;
        }
    }

    /// <summary>
    /// It will move between 2 positions which are o
    /// </summary>
    public virtual void DoInvertPosition()
    {
        if (m_CompletedFlag)
        {
            if (m_AtOriginalPosition)
            {
                LeanTween.moveLocal(gameObject, m_PositionToMoveTo, m_AnimatePositionTime).setOnComplete(CompleteTweening);
                m_AtOriginalPosition = false;
            }
            else
            {
                LeanTween.moveLocal(gameObject, m_OriginalPosition, m_AnimatePositionTime).setOnComplete(CompleteTweening);
                m_AtOriginalPosition = true;
            }
            m_CompletedFlag = false;
        }
    }

    protected virtual void CompleteTweening()
    {
        m_CompletedFlag = true;
    }
}
