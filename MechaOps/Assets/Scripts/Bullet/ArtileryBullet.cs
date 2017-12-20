using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtileryBullet : MonoBehaviour
{
    [SerializeField] private GameObject m_ExplosionPrefab;
    [SerializeField] private string[] m_CollisionLayers;

    private Tile m_Target;
    private bool m_Paused = false;
    private Void_Void m_CompletionCallback = null;

    public Void_Void CompletionCallback
    {
        get { return m_CompletionCallback; }
        set { m_CompletionCallback = value; }
    }

    private void InvokeCompletionCallback()
    {
        if (m_CompletionCallback != null)
        {
            m_CompletionCallback();
        }
    }

    public string[] CollisionLayers
    {
        get { return m_CollisionLayers; }
        set { m_CollisionLayers = value; }
    }

    public Tile Target
    {
        get { return m_Target; }
        set { m_Target = value; }
    }

    public void SetPaused(bool _paused) { m_Paused = _paused; }

    public bool GetPaused() { return m_Paused; }

    private void OnDestroy()
    {
        InvokeCompletionCallback();
    }

    private void Update()
    {
        if (m_Paused) { return; }
    }
}