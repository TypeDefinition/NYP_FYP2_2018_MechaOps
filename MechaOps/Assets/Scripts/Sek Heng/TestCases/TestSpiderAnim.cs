using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpiderAnim : MonoBehaviour {
    [SerializeField] bool m_AttackFlag = false;
    [SerializeField] MOAnimator_Spider m_SpiderAnimator;
    [SerializeField] GameObject m_TargetGO;

	// Use this for initialization
	void Start () {
		if (!m_SpiderAnimator)
        {
            m_SpiderAnimator = GetComponent<MOAnimator_Spider>();
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (m_AttackFlag)
        {
            m_SpiderAnimator.StartShootAnimation(m_TargetGO, null);
            m_AttackFlag = false;
        }
	}
}
