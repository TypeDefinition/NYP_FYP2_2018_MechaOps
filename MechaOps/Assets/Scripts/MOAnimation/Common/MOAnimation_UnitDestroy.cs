using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class MOAnimation_UnitDestroy : MOAnimation
{
    // Prefabs
    [SerializeField] private SimpleSpriteAnimation m_ExplosionPrefab = null;
    [SerializeField] private ParticleSystem m_FlamePrefab = null;

    // Runtime Created
    private SimpleSpriteAnimation m_Explosion = null;
    private ParticleSystem m_Flame = null;

    private void DeleteAnimationObjects()
    {
        if (m_Explosion != null)
        {
            GameObject.Destroy(m_Explosion.gameObject);
        }

        if (m_Flame != null)
        {
            GameObject.Destroy(m_Flame.gameObject);
        }
    }

    public override MOAnimator GetMOAnimator() { return null; }

    public override void StartAnimation()
    {
        Assert.IsTrue(m_ExplosionPrefab != null);
        Assert.IsTrue(m_FlamePrefab != null);

        DeleteAnimationObjects();

        m_Explosion = GameObject.Instantiate(m_ExplosionPrefab.gameObject, gameObject.transform, false).GetComponent<SimpleSpriteAnimation>();
        m_Flame = GameObject.Instantiate(m_FlamePrefab.gameObject, gameObject.transform, false).GetComponent<ParticleSystem>();
    }

    public override void PauseAnimation()
    {
        if (m_Explosion != null)
        {
            m_Explosion.PauseAnimation();
        }

        if (m_Flame != null)
        {
            m_Flame.Pause();
        }
    }

    public override void ResumeAnimation()
    {
        if (m_Explosion != null)
        {
            m_Explosion.ResumeAnimation();
        }

        if (m_Flame != null)
        {
            m_Flame.Play();
        }
    }

    public override void StopAnimation()
    {
        DeleteAnimationObjects();
    }

}
