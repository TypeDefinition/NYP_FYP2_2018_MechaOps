using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : MonoBehaviour {

    [SerializeField, Range(0.0f, 360.0f)]
    private float m_UpdateInterval = 0.5f;
    private float m_TimeUntilUpdate = 0.0f;
    private float m_AccumulatedTime = 0.0f;
    private int m_FramesPassed = 0;

	// Use this for initialization
	void Start () {
		if (gameObject.GetComponent<GUIText>() == null) {
            Debug.Log("FPSCounter requires a GUIText Component!");
            enabled = false;
        } else {
            m_TimeUntilUpdate = m_UpdateInterval;
        }
	}
	
	// Update is called once per frame
	void Update () {
        ++m_FramesPassed;
        // We do not want our FPSCounter to be affected by Time.timeScale.
        m_TimeUntilUpdate -= Time.unscaledDeltaTime;
        m_AccumulatedTime += Time.unscaledDeltaTime;

        if (m_TimeUntilUpdate <= 0.0f) {
            gameObject.GetComponent<GUIText>().text = "FPS: " + ((float)m_FramesPassed/m_AccumulatedTime).ToString("f1");
            m_AccumulatedTime = 0.0f;
            m_TimeUntilUpdate = m_UpdateInterval;
            m_FramesPassed = 0;
        }
	}
}
