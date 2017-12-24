using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunshipBullet : Bullet
{
    [SerializeField] private float m_Speed = 100.0f;

    protected GameObject m_Target = null;

    public GameObject Target
    {
        get { return m_Target; }
        set { m_Target = value; }
    }

    protected virtual void Update()
    {
        if (m_Paused) { return; }

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, m_Speed * Time.deltaTime))
        {
            if (hitInfo.collider.gameObject == m_Target)
            {
                m_Lifetime = 0.0f;
            }
        }

        // Move the bullet every frame.
        transform.position += transform.forward * m_Speed * Time.deltaTime;

        UpdateLifetime();
    }
}