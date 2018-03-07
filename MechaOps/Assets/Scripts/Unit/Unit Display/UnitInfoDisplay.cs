using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// meant to just display the stats UI only!
/// </summary>
public class UnitInfoDisplay : TweenUI_Scale
{
    [SerializeField] protected HealthBar m_HealthBar = null;
    [SerializeField] protected ActionPointsCounter m_ActionPointsCounter = null;
    [SerializeField] protected Image m_SpottedIndicator = null;
    [Tooltip("This offset is added to the unit's world space position when converting from the unit's world space to screen space position.")]
    [SerializeField] protected Vector3 m_UnitWorldPositionOffset = new Vector3(0.0f, 2.5f, 0.0f);
    [SerializeField] protected float m_PositionZ = 0.0f;

    [Header("Audio")]
    [SerializeField] protected AudioClip m_SpottedSFX = null;

    protected UnitStats m_UnitStats = null;
    protected AudioSource m_SFXAudioSource = null;

    public HealthBar GetHealthBar() { return m_HealthBar; }
    public ActionPointsCounter GetActionPointsCounter() { return m_ActionPointsCounter; }

    public void SetUnitStats(UnitStats _unitStats)
    {
        m_UnitStats = _unitStats;
        StatsChangeCallback(m_UnitStats);
    }
    public UnitStats GetUnitStats() { return m_UnitStats; }

    public Vector3 GetUnitWorldPositionOffset() { return m_UnitWorldPositionOffset; }

    private void StatsChangeCallback(UnitStats _unitStats)
    {
        // Update Health Points
        m_HealthBar.MaxHealthPoints = _unitStats.MaxHealthPoints;
        m_HealthBar.CurrentHealthPoints = _unitStats.CurrentHealthPoints;

        // Update Action Points
        m_ActionPointsCounter.MaxActionPoints = _unitStats.MaxActionPoints;
        m_ActionPointsCounter.CurrentActionPoints = _unitStats.CurrentActionPoints;
    }

    protected override void Awake()
    {
        base.Awake();
        Assert.IsTrue(m_HealthBar != null, MethodBase.GetCurrentMethod().Name + " - m_HealthBar must not be null!");
        Assert.IsTrue(m_ActionPointsCounter != null, MethodBase.GetCurrentMethod().Name + " - m_ActionPointsCounter must not be null!");

        m_SFXAudioSource = GameSystemsDirectory.GetSceneInstance().GetSFXAudioSource();
        Assert.IsNotNull(m_SFXAudioSource);
    }

    /// <summary>
    /// as unit always update it's position at Update(). this comes afterwards
    /// </summary>
    protected virtual void Update()
    {
        if (m_UnitStats != null)
        {
            // Update Position
            Camera gameCamera = m_UnitStats.GetGameSystemsDirectory().GetGameCamera();
            Vector3 screenPoint = gameCamera.WorldToScreenPoint(m_UnitStats.gameObject.transform.position + m_UnitWorldPositionOffset);
            screenPoint.z = m_PositionZ;
            transform.position = screenPoint;

            if (m_SpottedIndicator != null)
            {
                if (m_SpottedIndicator.gameObject.activeSelf)
                {
                    if (m_UnitStats.GetViewScript().GetVisibilityCount() <= 0)
                    {
                        m_SpottedIndicator.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (m_UnitStats.GetViewScript().GetVisibilityCount() > 0)
                    {
                        m_SpottedIndicator.gameObject.SetActive(true);
                        m_SFXAudioSource.PlayOneShot(m_SpottedSFX);
                    }
                }
            }

            #region Check Whether can see it!
            // TODO: I believe that this part can be improved further by taking into account of the camera field of view angle then we set the Display to be inactive!
            Vector3 directionFromCamToUnitInfoPos = m_UnitStats.gameObject.transform.position - gameCamera.transform.position;
            directionFromCamToUnitInfoPos.Normalize();
            float DotProductResult = Vector3.Dot(gameCamera.transform.forward, directionFromCamToUnitInfoPos);
            // if it is out of the camera range (more like whether is it more than 90 degrees!), the do not render the images!
            if (DotProductResult < 0)
            {
                SetInfoDisplayRender(false);
            }
            else
            {
                SetInfoDisplayRender(true);
            }
            #endregion
        }
    }

    void SetInfoDisplayRender(bool _renderActive)
    {
        if (m_SpottedIndicator && m_SpottedIndicator.enabled != _renderActive)
        {
            m_SpottedIndicator.enabled = _renderActive;
        }
        m_HealthBar.gameObject.SetActive(_renderActive);
        m_ActionPointsCounter.gameObject.SetActive(_renderActive);
    }
}