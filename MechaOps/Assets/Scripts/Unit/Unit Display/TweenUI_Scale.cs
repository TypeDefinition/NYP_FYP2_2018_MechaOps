using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Just tweening the scale of X-axis and uses LeanTween
/// </summary>
public class TweenUI_Scale : MonoBehaviour {
    [Header("Variables for TweenUI_Scale")]
    [Tooltip("The animation time")]
    public float m_AnimTime = 0.2f;

    [Header("Debugging for TweenUI_Scale")]
    [SerializeField, Tooltip("The original scale for the tween")]
    protected Vector3 m_OriginalScale;

    protected virtual void Awake()
    {
        m_OriginalScale = transform.localScale;
    }

    /// <summary>
    /// To just do some simple tweening of the x-axis. Override this function if other tweening is required. If complicated tweening is required, then use Unity Animation
    /// </summary>
    public virtual void AnimateUI()
    {
        Vector3 zeLocalScale = transform.localScale;
        zeLocalScale.x = 0;
        transform.localScale = zeLocalScale;
        LeanTween.scaleX(gameObject, m_OriginalScale.x, m_AnimTime);
    }
}
