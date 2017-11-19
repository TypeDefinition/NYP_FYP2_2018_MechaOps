using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBullet : MonoBehaviour
{
    [SerializeField] private float m_Speed = 100.0f;
    [SerializeField] private float m_Lifetime = 0.5f;
    [SerializeField] private bool m_ExplodeOnContact = true;
    [SerializeField] private GameObject m_Explosion;
    [SerializeField] private string[] m_CollisionLayers;
    [SerializeField] private GameObject m_Target;
    private Void_Void m_CompletionCallback = null;

    public Void_Void CompletionCallback
    {
        get { return m_CompletionCallback; }
        set { m_CompletionCallback = value; }
    }

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

    public string[] CollisionLayers
    {
        get { return m_CollisionLayers; }
        set { m_CollisionLayers = value; }
    }

    public GameObject Target
    {
        get { return m_Target; }
        set { m_Target = value; }
    }

    
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
        if (m_ExplodeOnContact && Physics.Raycast(transform.position, transform.forward, out hitInfo, m_Speed * Time.deltaTime, LayerMask.GetMask(m_CollisionLayers)))
        {
            TileDisplay_Shield tileDisplayShield = hitInfo.collider.gameObject.GetComponent<TileDisplay_Shield>();
            if (tileDisplayShield != null)
            {
                tileDisplayShield.GetShieldAnimator().StartShieldBreakAnimation();
            }

            if (hitInfo.collider.gameObject == m_Target)
            {
                GameObject explosion = GameObject.Instantiate(m_Explosion);
                explosion.transform.position = hitInfo.point;
                m_Lifetime = 0.0f;
            }
        }

        // Every frame move forward.
        transform.position += transform.forward * m_Speed * Time.deltaTime;

        // Destroy this bullet once it's lifetime is over.
        if ((m_Lifetime -= Time.deltaTime) < 0.0f)
        {
            if (m_CompletionCallback != null)
            {
                m_CompletionCallback();
            }
            GameObject.Destroy(gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Speed = m_Speed;
        Lifetime = m_Lifetime;
    }
#endif

}