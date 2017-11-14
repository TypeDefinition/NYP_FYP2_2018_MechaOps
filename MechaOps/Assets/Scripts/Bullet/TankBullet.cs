using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBullet : MonoBehaviour
{
    [SerializeField] private float m_Speed = 100.0f;
    [SerializeField] private float m_Lifetime = 0.5f;
    //[SerializeField] private string m_TargetTag;
    [SerializeField] private bool m_ExplodeOnContact = true;
    [SerializeField] private GameObject m_Explosion;

    public float Speed
    {
        get { return m_Speed; }
        set { m_Speed = Mathf.Max(0.0f, value); }
    }

    public float Lifetime
    {
        get { return m_Lifetime; }
        set { m_Lifetime = Mathf.Max(0.0f, value); }
    }

    /*public string TargetTag
    {
        get { return m_TargetTag; }
        set { m_TargetTag = value; }
    }*/

    public bool ExplodeOnContact
    {
        get { return m_ExplodeOnContact; }
        set { m_ExplodeOnContact = value; }
    }

    // Update is called once per frame
    void Update ()
    {
        // Detect Hit
        RaycastHit hitInfo;
        if (m_ExplodeOnContact && Physics.Raycast(transform.position, transform.forward, out hitInfo, m_Speed * Time.deltaTime))
        {
            GameObject explosion = GameObject.Instantiate(m_Explosion);
            explosion.transform.position = hitInfo.point;
            m_Lifetime = 0.0f;
        }

        // Destroy this bullet once it's lifetime is over.
        if ((m_Lifetime -= Time.deltaTime) < 0.0f)
        {
            GameObject.Destroy(gameObject);
        }

        // Every frame move forward.
        transform.position += transform.forward * m_Speed * Time.deltaTime;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Speed = m_Speed;
        Lifetime = m_Lifetime;
    }
#endif

}
