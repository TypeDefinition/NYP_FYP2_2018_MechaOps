using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Reflection;

/// <summary>
/// Copy and paste from UnitInfoDisplay. but this is dedicated for World canvas.
/// </summary>
public class MyUnitInfoDisplay : UnitInfoDisplay
{
    [SerializeField] protected float m_ScaleZ = 1.0f;
    [Tooltip("The scale of this UI is 1 if the distance to the camera is this amount!")]
    [SerializeField] protected float m_DistanceYToCameraScale = 12.5f;
    [Tooltip("Scale of this UI is 1 if the FOV of the camera is this amount")]
    [SerializeField] protected float m_FOVToCameraScale = 50.0f;
    [SerializeField] protected Camera m_UnitInfoCamera;
    [SerializeField] protected float m_MaxScaleLimit = 5.0f;
    [SerializeField] protected float m_MinScaleLimit = 1.0f;

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

        m_UnitInfoCamera = MyGameSystemDirectory.GetSceneInstance().GetUnitInfoCamera();
    }

    /// <summary>
    /// as unit always update it's position at Update(). this comes afterwards
    /// </summary>
    protected override void LateUpdate()
    {
        // set the position offset
        transform.position = m_UnitStats.transform.position + m_UnitWorldPositionOffset;
        // then scale according to the camera distance away from it!
        float HeightToCamera = transform.position.y - m_UnitInfoCamera.transform.position.y;
        // we will also need to take into account of the field of view!
        // Unfortunately, this is more or less estimation
        float MultiplyResult = (HeightToCamera / m_DistanceYToCameraScale) * (m_UnitInfoCamera.fieldOfView / m_FOVToCameraScale);
        MultiplyResult = Mathf.Clamp(MultiplyResult, m_MinScaleLimit, m_MaxScaleLimit);
        transform.localScale = new Vector3(MultiplyResult, MultiplyResult, m_ScaleZ);
    }
}
