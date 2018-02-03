using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtileryBullet : Bullet
{
    [SerializeField] private GameObject m_ExplosionPrefab = null;
    [SerializeField] private MeshRenderer m_BulletMeshRenderer = null;

    [SerializeField] private string[] m_CollisionLayers = { "Tile" };
    private int m_CollisionLayerMask = 0;

    private Vector3 m_InitialVelocity = new Vector3();
    private Vector3 m_CurrentVelocity = new Vector3();
    private Vector3 m_Gravity = new Vector3(0.0f, -9.8f, 0.0f);

    private Tile m_TargetTile;
    private bool m_HitTarget = false;
    private float m_AfterHitLifetime = 1.0f;

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

    public Tile TargetTile
    {
        get { return m_TargetTile; }
        set { m_TargetTile = value; }
    }

    protected virtual void Awake()
    {
        m_CollisionLayerMask = LayerMask.GetMask(m_CollisionLayers);
    }

    protected virtual void Update()
    {
        if (m_Paused) { return; }

        if (!m_HitTarget)
        {
            // Detect Hit
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, transform.forward, out hitInfo, m_CurrentVelocity.magnitude * Time.deltaTime, m_CollisionLayerMask))
            {
                GameObject explosion = GameObject.Instantiate(m_ExplosionPrefab);
                explosion.transform.position = m_TargetTile.transform.position;
                
                if (m_BulletMeshRenderer)
                {
                    m_BulletMeshRenderer.enabled = false;
                }
                
                m_Lifetime = m_AfterHitLifetime;
                
                m_HitTarget = true;
            }

            transform.position += m_CurrentVelocity * Time.deltaTime;
            m_CurrentVelocity += (m_Gravity * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(m_CurrentVelocity);
        }

        // Destroy this bullet once it's lifetime is over.
        UpdateLifetime();
    }
}