using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWaspAnimator : MonoBehaviour {
    [SerializeField] bool m_AttackFlag = false;
    [SerializeField] MOAnimator_Wasp m_WaspAnimator;
    [SerializeField] GameObject m_TargetGO;

	// Use this for initialization
	void Start () {
        if (!m_WaspAnimator)
            m_WaspAnimator = GetComponent<MOAnimator_Wasp>();
    }

    private void Update()
    {
        if (m_AttackFlag)
        {
            m_WaspAnimator.StartShootAnimation(m_TargetGO, null);
            m_AttackFlag = false;
        }
    }
}
