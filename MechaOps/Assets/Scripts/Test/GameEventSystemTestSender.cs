using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystemTestSender : MonoBehaviour {

    private int m_Counter = 0;
    private float m_Timer = 1.0f;

	// Use this for initialization
	void Start()
    {
	}

    private void Update()
    {
        if ((m_Timer -= Time.deltaTime) < 0.0f)
        {
            m_Timer = 1.0f;
            GameEventSystem.GetInstance().TriggerEvent<int>("テスト", m_Counter++);
        }
    }

}