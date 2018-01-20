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
    [Tooltip("This offset is added to the unit's world space position when converting from the unit's world space to screen space position.")]
    [SerializeField] protected Vector3 m_UnitWorldPositionOffset = new Vector3(0.0f, 2.5f, 0.0f);
    [SerializeField] protected float m_PositionZ = 0.0f;
    [Tooltip("The scale of this UI is 1 if the distance to the camera is this amount!")]
    [SerializeField] protected float m_DistanceToCameraScale = 12.5f;
    [Tooltip("Scale of this UI is 1 if the FOV of the camera is this amount")]
    [SerializeField] protected float m_FOVToCameraScale = 50.0f;
    [SerializeField] protected Camera m_UnitInfoCamera;
    [SerializeField] protected float m_ScaleLimit = 5.0f;

    protected UnitStats m_UnitStats = null;

    public HealthBar GetHealthBar() { return m_HealthBar; }
    public ActionPointsCounter GetActionPointsCounter() { return m_ActionPointsCounter; }

    public void SetUnitStats(UnitStats _unitStats) { m_UnitStats = _unitStats; }
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
        m_UnitInfoCamera = GameSystemsDirectory.GetSceneInstance().GetUnitInfoCamera();
    }

    //protected void Update()
    //{
    //    if (m_UnitStats != null)
    //    {
    //        // Update Position
    //        Camera gameCamera = m_UnitStats.GetGameSystemsDirectory().GetGameCamera();
    //        Vector3 screenPoint = gameCamera.WorldToScreenPoint(m_UnitStats.gameObject.transform.position + m_UnitWorldPositionOffset);
    //        Vector2 canvasPoint;
    //        Camera cameraToUse = m_UnitInfoCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : gameCamera;
    //        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_UnitInfoCanvas.transform as RectTransform, screenPoint, cameraToUse, out canvasPoint);
    //        //screenPoint.z = m_PositionZ;
    //        //transform.position = screenPoint;
    //        transform.position = new Vector3(canvasPoint.x, canvasPoint.y, m_PositionZ);
    //    }
    //}

    #region WorldSpaceScalingLogic
        /// <summary>
        /// as unit always update it's position at Update(). this comes afterwards
        /// </summary>
    private void LateUpdate()
    {
        // set the position offset
        transform.position = m_UnitStats.transform.position + m_UnitWorldPositionOffset;
        // then scale according to the camera distance away from it!
        float DistanceToCamera = (transform.position - m_UnitInfoCamera.transform.position).magnitude;
        // we will also need to take into account of the field of view!
        // Unfortunately, this is more or less estimation
        float MultiplyResult = (DistanceToCamera / m_DistanceToCameraScale) * (m_UnitInfoCamera.fieldOfView / m_FOVToCameraScale);
        MultiplyResult = Mathf.Min(MultiplyResult, m_ScaleLimit);
        transform.localScale = new Vector3(MultiplyResult, MultiplyResult, m_PositionZ);
    }
    #endregion
}