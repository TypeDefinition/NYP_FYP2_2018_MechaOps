using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

public class AnimationLock
{
}

[DisallowMultipleComponent]
public class ShieldAnimator : MonoBehaviour
{
    [SerializeField] private Color m_OnColor = Color.yellow;
    [SerializeField] private Color m_OffColor = Color.red;
    [SerializeField] private float m_AnimateTurnOffSpeed = 0.1f;
    [SerializeField] private float m_AnimateTurnOnSpeed = 0.5f;
    [SerializeField] private float m_AnimateShieldBreakSpeed = 5.0f;
    [SerializeField] private float m_AnimateShieldRegenSpeed = 5.0f;

    private MeshRenderer m_MeshRenderer = null;
    private bool m_IsAnimationRunning = false;
    private AnimationLock m_AnimationLock; // This doesn't do shit probably because Coroutines are done in the main thread. I'm too fucking pissed off to remove it. Just keep it there.

    public float AnimateTurnOffSpeed
    {
        get { return m_AnimateTurnOffSpeed; }
        set { m_AnimateTurnOffSpeed = Mathf.Max(0.0005f, value); }
    }

    public float AnimateTurnOnSpeed
    {
        get { return m_AnimateTurnOnSpeed; }
        set { m_AnimateTurnOnSpeed = Mathf.Max(0.0005f, value); }
    }

    public float AnimateShieldBreakSpeed
    {
        get { return m_AnimateShieldBreakSpeed; }
        set { m_AnimateShieldBreakSpeed = Mathf.Max(0.0005f, value); }
    }

    public float AnimateShieldRegenSpeed
    {
        get { return m_AnimateShieldRegenSpeed; }
        set { m_AnimateShieldRegenSpeed = Mathf.Max(0.0005f, value); }
    }

    public bool IsAnimationRunning
    {
        get { return m_IsAnimationRunning; }
    }

    private void Start()
    {
        m_MeshRenderer = gameObject.GetComponent<MeshRenderer>();
        Assert.IsTrue(m_MeshRenderer != null, MethodBase.GetCurrentMethod().Name + " - MeshRenderer required for this to work!");

        m_AnimationLock = new AnimationLock();
    }

    private Color ChangeColor(Color _color, float _r, float _g, float _b, float _a)
    {
        Color result = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        result.r = Mathf.Clamp(_color.r + _r, 0.0f, 1.0f);
        result.g = Mathf.Clamp(_color.g + _g, 0.0f, 1.0f);
        result.b = Mathf.Clamp(_color.b + _b, 0.0f, 1.0f);
        result.a = Mathf.Clamp(_color.a + _a, 0.0f, 1.0f);

        return result;
    }

    private Color ChangeColor(Color _source, Color _target, float _speed)
    {
        Color result = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        result.r = Mathf.Lerp(_source.r, _target.r, _speed);
        result.g = Mathf.Lerp(_source.g, _target.g, _speed);
        result.b = Mathf.Lerp(_source.b, _target.b, _speed);
        result.a = Mathf.Lerp(_source.a, _target.a, _speed);

        return result;
    }

    private bool EqualFloats(float _a, float _b)
    {
        return Mathf.Abs(_a - _b) <= 0.001f;
    }

    private IEnumerator TurnOffAnimationCoroutine()
    {
        if (m_IsAnimationRunning)
        {
            Debug.Log(MethodBase.GetCurrentMethod().Name + " - Cannot start an animation when another animation is currently running!");
            yield break;
        }
        m_IsAnimationRunning = true;

        Assert.IsTrue(m_AnimationLock != null);
        if (Monitor.TryEnter(m_AnimationLock, 0))
        {
            //Debug.Log(MethodBase.GetCurrentMethod().Name + " - Obtained Lock");
            try
            {
                bool redDone = false;
                bool greenDone = false;
                bool blueDone = false;
                bool alphaDone = false;

                while (true)
                {
                    redDone = EqualFloats(m_MeshRenderer.material.color.r, m_OffColor.r);
                    greenDone = EqualFloats(m_MeshRenderer.material.color.g, m_OffColor.g);
                    blueDone = EqualFloats(m_MeshRenderer.material.color.b, m_OffColor.b);
                    alphaDone = EqualFloats(m_MeshRenderer.material.color.a, m_OffColor.a);

                    if (redDone && greenDone && blueDone && alphaDone)
                    {
                        break;
                    }
                    else
                    {
                        m_MeshRenderer.material.color = ChangeColor(m_MeshRenderer.material.color, m_OffColor, m_AnimateTurnOffSpeed * Time.deltaTime);
                        yield return null;
                    }
                }
            }
            finally
            {
                Monitor.Exit(m_AnimationLock);
                Debug.Log(MethodBase.GetCurrentMethod().Name + " - Completed");
            }
        }
        else
        {
            //Debug.Log(MethodBase.GetCurrentMethod().Name + " - Cannot start an animation when another animation is currently running!");
        }

        m_IsAnimationRunning = false;
    }

    private IEnumerator TurnOnAnimationCoroutine()
    {
        if (m_IsAnimationRunning)
        {
            Debug.Log(MethodBase.GetCurrentMethod().Name + " - Cannot start an animation when another animation is currently running!");
            yield break;
        }
        m_IsAnimationRunning = true;

        Assert.IsTrue(m_AnimationLock != null);
        if (Monitor.TryEnter(m_AnimationLock, 0))
        {
            //Debug.Log(MethodBase.GetCurrentMethod().Name + " - Obtained Lock");
            try
            {
                bool redDone = false;
                bool greenDone = false;
                bool blueDone = false;
                bool alphaDone = false;

                while (true)
                {
                    redDone = EqualFloats(m_MeshRenderer.material.color.r, m_OnColor.r);
                    greenDone = EqualFloats(m_MeshRenderer.material.color.g, m_OnColor.g);
                    blueDone = EqualFloats(m_MeshRenderer.material.color.b, m_OnColor.b);
                    alphaDone = EqualFloats(m_MeshRenderer.material.color.a, m_OnColor.a);

                    if (redDone && greenDone && blueDone && alphaDone)
                    {
                        break;
                    }
                    else
                    {
                        m_MeshRenderer.material.color = ChangeColor(m_MeshRenderer.material.color, m_OnColor, m_AnimateTurnOnSpeed * Time.deltaTime);
                        yield return null;
                    }
                }
            }
            finally
            {
                Monitor.Exit(m_AnimationLock);
                Debug.Log(MethodBase.GetCurrentMethod().Name + " - Completed");
            }
        }
        else
        {
            //Debug.Log(MethodBase.GetCurrentMethod().Name + " - Cannot start an animation when another animation is currently running!");
        }

        m_IsAnimationRunning = false;
    }

    private IEnumerator ShieldBreakAnimationCoroutine()
    {
        if (m_IsAnimationRunning)
        {
            Debug.Log(MethodBase.GetCurrentMethod().Name + " - Cannot start an animation when another animation is currently running!");
            yield break;
        }
        m_IsAnimationRunning = true;

        Assert.IsTrue(m_AnimationLock != null);
        if (Monitor.TryEnter(m_AnimationLock, 0))
        {
            //Debug.Log(MethodBase.GetCurrentMethod().Name + " - Obtained Lock");
            try
            {
                // Turn off shield.
                bool redDone = false;
                bool greenDone = false;
                bool blueDone = false;
                bool alphaDone = false;

                while (true)
                {
                    redDone = EqualFloats(m_MeshRenderer.material.color.r, m_OffColor.r);
                    greenDone = EqualFloats(m_MeshRenderer.material.color.g, m_OffColor.g);
                    blueDone = EqualFloats(m_MeshRenderer.material.color.b, m_OffColor.b);
                    alphaDone = EqualFloats(m_MeshRenderer.material.color.a, m_OffColor.a);

                    if (redDone && greenDone && blueDone && alphaDone)
                    {
                        break;
                    }
                    else
                    {
                        m_MeshRenderer.material.color = ChangeColor(m_MeshRenderer.material.color, m_OffColor, m_AnimateShieldBreakSpeed * Time.deltaTime);
                        yield return null;
                    }
                }

                // Wait for a while.
                //yield return new WaitForSeconds(2.0f);

                // Turn on Shields
                redDone = false;
                greenDone = false;
                blueDone = false;
                alphaDone = false;

                while (true)
                {
                    redDone = EqualFloats(m_MeshRenderer.material.color.r, m_OnColor.r);
                    greenDone = EqualFloats(m_MeshRenderer.material.color.g, m_OnColor.g);
                    blueDone = EqualFloats(m_MeshRenderer.material.color.b, m_OnColor.b);
                    alphaDone = EqualFloats(m_MeshRenderer.material.color.a, m_OnColor.a);

                    if (redDone && greenDone && blueDone && alphaDone)
                    {
                        break;
                    }
                    else
                    {
                        m_MeshRenderer.material.color = ChangeColor(m_MeshRenderer.material.color, m_OnColor, m_AnimateShieldRegenSpeed * Time.deltaTime);
                        yield return null;
                    }
                }
            }
            finally
            {
                Monitor.Exit(m_AnimationLock);
                Debug.Log(MethodBase.GetCurrentMethod().Name + " - Completed");
            }
        }
        else
        {
            //Debug.Log(MethodBase.GetCurrentMethod().Name + " - Cannot start an animation when another animation is currently running!");
        }
        
        m_IsAnimationRunning = false;
    }

    public void StartTurnOffAnimation()
    {
        StartCoroutine("TurnOffAnimationCoroutine");
    }

    public void StartTurnOnAnimation()
    {
        StartCoroutine("TurnOnAnimationCoroutine");
    }

    public void StartShieldBreakAnimation()
    {
        StartCoroutine("ShieldBreakAnimationCoroutine");
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        AnimateTurnOffSpeed = m_AnimateTurnOffSpeed;
        AnimateTurnOnSpeed = m_AnimateTurnOnSpeed;
        AnimateShieldBreakSpeed = m_AnimateShieldBreakSpeed;
        AnimateShieldRegenSpeed = m_AnimateShieldRegenSpeed;
    }
#endif

}