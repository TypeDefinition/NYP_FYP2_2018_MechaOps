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
    [SerializeField, Tooltip("Panzer Animator")] private PanzerAnimator m_Animator;

    // Runtime Created
    private SimpleSpriteAnimation m_Explosion = null;
    private ParticleSystem m_Flame = null;

    public PanzerAnimator PanzerAnim
    {
        get
        {
            return m_Animator;
        }
    }

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

    private void OnEnable()
    {
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>(tag + "IsDead", InvokeDeathAnimation);
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>(tag + "VisibleDead", InvokeDeathCinematic);
    }

    private void OnDisable()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>(tag + "IsDead", InvokeDeathAnimation);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>(tag + "VisibleDead", InvokeDeathCinematic);
    }

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

    /// <summary>
    /// Start the death particle!
    /// </summary>
    /// <param name="_go">The gameobject to check</param>
    public void InvokeDeathAnimation(GameObject _go)
    {
        if (_go == gameObject)
        {
            StartAnimation();
            GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>(tag + "IsDead", InvokeDeathAnimation);
        }
    }

    /// <summary>
    /// Start the death cinematic shot when the unit dies and only if it is visible!
    /// </summary>
    /// <param name="_go">The gameobject to check</param>
    public void InvokeDeathCinematic(GameObject _go)
    {
        if (_go == gameObject)
        {
            m_Animator.StartDeathAnimation();
            GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>(tag + "VisibleDead", InvokeDeathCinematic);
        }
    }
}
