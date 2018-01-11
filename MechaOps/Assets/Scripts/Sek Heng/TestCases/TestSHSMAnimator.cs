using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSHSMAnimator : MonoBehaviour {
    [SerializeField] bool m_AttackFlag;
    [SerializeField] protected MOAnimator_SHSM m_SHSMAnimator;
    [SerializeField] GameObject m_TargetGO;

	// Use this for initialization
	void Start () {
        if (!m_SHSMAnimator)
            m_SHSMAnimator = GetComponent<MOAnimator_SHSM>();
	}

    private void Update()
    {
        if (m_AttackFlag)
        {
            m_SHSMAnimator.StartShootAnimation(m_TargetGO.GetComponent<Tile>(), null);
            m_AttackFlag = false;
        }
    }
}
