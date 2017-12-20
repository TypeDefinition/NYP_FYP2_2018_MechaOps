using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructTimer : MonoBehaviour
{
    [SerializeField] private float m_LifeTime = 0.1f;
    private float m_TimeLeft = 0.0f;

    private void Awake()
    {
        m_TimeLeft = m_LifeTime;
    }

    // Update is called once per frame
    void Update ()
    {
        m_TimeLeft -= Time.deltaTime;
        if (m_TimeLeft < 0.0f)
        {
            GameObject.Destroy(gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        m_LifeTime = Mathf.Max(0.0f, m_LifeTime);
    }
#endif // UNITY_EDITOR

}