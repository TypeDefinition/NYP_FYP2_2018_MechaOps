using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBullet : Bullet
{
    [SerializeField] private float m_Speed = 100.0f;
    [SerializeField] private bool m_Hit = true;
    [SerializeField] private GameObject m_ExplosionPrefab;

    private GameObject m_Target;

    public float Speed
    {
        get { return m_Speed; }
        set { m_Speed = Mathf.Max(0.0f, value); }
    }

    public GameObject Target
    {
        get { return m_Target; }
        set { m_Target = value; }
    }
    
    public bool Hit
    {
        get { return m_Hit; }
        set { m_Hit = value; }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (m_Paused) { return; }

        // Detect Hit
        if (m_Hit)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, transform.forward, out hitInfo, m_Speed * Time.deltaTime))
            {
                if (hitInfo.collider.gameObject == m_Target)
                {
                    GameObject explosion = GameObject.Instantiate(m_ExplosionPrefab);
                    explosion.transform.position = hitInfo.point;
                    m_Lifetime = 0.0f;
                }
            }
        }

        // Move the bullet every frame.
        transform.position += transform.forward * m_Speed * Time.deltaTime;

        UpdateLifetime();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        Speed = m_Speed;
    }
#endif

}