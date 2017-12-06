using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour {

    public float m_Time = 5.0f;
	private float m_TimeLeft = 0.0f;

    void Start()
    {
        m_TimeLeft = m_Time;
    }

	// Update is called once per frame
	void Update ()
    {
		if ((m_TimeLeft -= Time.deltaTime) < 0.0f)
        {
            GameObject.Destroy(gameObject);
        }
	}
}
