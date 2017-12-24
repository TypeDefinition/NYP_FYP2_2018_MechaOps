using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class Bullet : MonoBehaviour
{
    [SerializeField] protected float m_Lifetime = 0.5f;
    protected bool m_Paused = false;
    protected Void_Void m_CompletionCallback = null;

    public float Lifetime
    {
        get { return m_Lifetime; }
        set { m_Lifetime = Mathf.Max(0.0f, value); }
    }

    public void SetPaused(bool _paused) { m_Paused = _paused; }
    public bool GetPaused() { return m_Paused; }

    public Void_Void CompletionCallback
    {
        get { return m_CompletionCallback; }
        set { m_CompletionCallback = value; }
    }

    protected void InvokeCompletionCallback()
    {
        if (m_CompletionCallback == null) { return; }
        m_CompletionCallback();
    }

    protected void UpdateLifetime()
    {
        m_Lifetime -= Time.deltaTime;
        if (m_Lifetime <= 0.0f)
        {
            GameObject.Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        InvokeCompletionCallback();
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        Lifetime = m_Lifetime;
    }
#endif
}