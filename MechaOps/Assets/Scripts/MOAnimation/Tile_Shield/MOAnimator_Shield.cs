using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent]
public class MOAnimator_Shield : MOAnimator
{
    [SerializeField] private MeshRenderer m_MeshRenderer = null;

    [SerializeField] private Color m_OnColor = Color.yellow;
    [SerializeField] private Color m_OffColor = Color.red;
    [SerializeField] private float m_AnimateTurnOffSpeed = 0.1f;
    [SerializeField] private float m_AnimateTurnOnSpeed = 0.5f;
    [SerializeField] private float m_AnimateShieldBreakSpeed = 5.0f;
    [SerializeField] private float m_AnimateShieldRegenSpeed = 5.0f;

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
    
    private void Start()
    {
        Assert.IsTrue(m_MeshRenderer != null, MethodBase.GetCurrentMethod().Name + " - MeshRenderer required for this to work!");
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

        Debug.Log(MethodBase.GetCurrentMethod().Name + " - Completed");
    }

    private IEnumerator TurnOnAnimationCoroutine()
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

        Debug.Log(MethodBase.GetCurrentMethod().Name + " - Completed");
    }

    private IEnumerator ShieldBreakAnimationCoroutine()
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

        yield return new WaitForSeconds(2.0f);

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
           
        Debug.Log(MethodBase.GetCurrentMethod().Name + " - Completed");
    }

    // Interface Function(s)
    // NOTE: Only 1 animation can be running at any given time.
    // If an animation is started while another animation is running, the old animation will be stopped.
    public void StartTurnOffAnimation()
    {
        StopAllAnimations();
        StartCoroutine("TurnOffAnimationCoroutine");
    }

    public void StartTurnOnAnimation()
    {
        StopAllAnimations();
        StartCoroutine("TurnOnAnimationCoroutine");
    }

    public void StartShieldBreakAnimation()
    {
        StopAllAnimations();
        StartCoroutine("ShieldBreakAnimationCoroutine");
    }

    public void StopAllAnimations()
    {
        StopAllCoroutines();
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