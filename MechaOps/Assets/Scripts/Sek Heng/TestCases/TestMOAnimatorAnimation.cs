using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMOAnimatorAnimation : MonoBehaviour {
    [SerializeField, Tooltip("Lift off flag")]
    protected bool m_WalkFlag = false;
    [SerializeField] protected bool m_DeadFlag = false;
    [SerializeField] MOAnimator m_Animator;
    [SerializeField] TileId[] m_SetUpTileArray;

    private void Start()
    {
        m_Animator = GetComponent<MOAnimator>();
    }

    // Update is called once per frame
    void Update () {
		if (m_WalkFlag)
        {
            m_Animator.StartMoveAnimation(m_SetUpTileArray, null, null);
            m_WalkFlag = false;
        }
        if (m_DeadFlag)
        {
            m_Animator.StartDeathAnimation(null);
            m_DeadFlag = false;
        }
	}
}
