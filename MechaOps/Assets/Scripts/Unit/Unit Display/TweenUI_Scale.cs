using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Just tweening the scale of X-axis and uses LeanTween
/// </summary>
public class TweenUI_Scale : MonoBehaviour
{
    [SerializeField] public float m_TweenAnimationDuration = 0.2f;

    protected Vector3 m_OriginalScale;

    protected virtual void Awake()
    {
        UpdateOriginalScale();
    }

    public virtual void OnEnable()
    {
        AnimateUI();
    }

    public void UpdateOriginalScale()
    {
        m_OriginalScale = transform.localScale;
    }

    /// <summary>
    /// To just do some simple tweening of the x-axis. Override this function if other tweening is required. If complicated tweening is required, then use Unity Animation
    /// </summary>
    public virtual void AnimateUI()
    {
        transform.localScale = new Vector3(0.0f, transform.localScale.y, transform.localScale.z);
        LeanTween.scaleX(gameObject, m_OriginalScale.x, m_TweenAnimationDuration);
    }
}