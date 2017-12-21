using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtileryBullet : MonoBehaviour
{
    [SerializeField] private GameObject m_ExplosionPrefab;
    //[SerializeField] private string[] m_CollisionLayers;

    private Vector3 m_InitialVelocity = new Vector3();
    private Vector3 m_CurrentVelocity = new Vector3();
    private Vector3 m_Gravity = new Vector3(0.0f, -9.8f, 0.0f);

    private Tile m_TargetTile;
    private float m_Lifetime = 5.0f;
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

    public Vector3 InitialVelocity
    {
        get { return m_InitialVelocity; }
        set { m_InitialVelocity = value; m_CurrentVelocity = value; }
    }

    public Vector3 CurrentVelocity
    {
        get { return m_CurrentVelocity; }
    }

    public Vector3 Gravity
    {
        get { return m_Gravity; }
        set { m_Gravity = value; }
    }

    /*public string[] CollisionLayers
    {
        get { return m_CollisionLayers; }
        set { m_CollisionLayers = value; }
    }*/

    public float Lifetime
    {
        get { return m_Lifetime; }
        set { m_Lifetime = Mathf.Max(0.0f, value); }
    }

    public Tile TargetTile
    {
        get { return m_TargetTile; }
        set { m_TargetTile = value; }
    }

    public void SetPaused(bool _paused) { m_Paused = _paused; }

    public bool GetPaused() { return m_Paused; }

    private void OnDestroy()
    {
        GameObject explosion = GameObject.Instantiate(m_ExplosionPrefab);
        explosion.transform.position = m_TargetTile.transform.position;
        InvokeCompletionCallback();
    }

    private void Update()
    {
        if (m_Paused) { return; }

        transform.position += m_CurrentVelocity * Time.deltaTime;
        m_CurrentVelocity += (m_Gravity * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(m_CurrentVelocity);

        // Detect Hit
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, m_CurrentVelocity.magnitude * Time.deltaTime))
        {
            // If we touch any shield, play the break animation.
            MOAnimation_ShieldBreak shieldBreak = hitInfo.collider.gameObject.GetComponent<MOAnimation_ShieldBreak>();
            if (shieldBreak != null)
            {
                shieldBreak.StartAnimation();
            }

            if (hitInfo.collider.gameObject == m_TargetTile.gameObject)
            {
                Debug.Log("UMR");
                m_Lifetime = 0.0f;
            }
        }

        // Destroy this bullet once it's lifetime is over.
        m_Lifetime -= Time.deltaTime;
        if (m_Lifetime <= 0.0f)
        {
            GameObject.Destroy(gameObject);
        }
    }
}