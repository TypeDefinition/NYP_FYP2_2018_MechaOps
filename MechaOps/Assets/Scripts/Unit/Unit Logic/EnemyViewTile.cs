using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will be used to render this unit invisible! inherit from PlayerViewScript to be lazy
/// </summary>
[DisallowMultipleComponent]
public class EnemyViewTile : ViewTileScript
{
    [Header("Debugging for EnemyViewTile")]
    [SerializeField, Tooltip("Enemy Visiblity counter. Do not touch")]
    protected int m_VisibilityCount = 0;
    [SerializeField, Tooltip("The array of MeshRenderer inside this unit")]
    protected MeshRenderer[] m_AllRenderers;

    public int VisibilityCount
    {
        set
        {
            m_VisibilityCount = Mathf.Max(0, value);
            switch (m_VisibilityCount)
            {
                case 0:
                    // if the render is active to begin with, make everything else inactive including itself!
                    if (m_AllRenderers[0].enabled)
                    {
                        foreach (MeshRenderer zeRender in m_AllRenderers)
                        {
                            zeRender.enabled = false;
                        }
                    }
                    break;
                default:
                    // if the render is inactive, make everything else active including itself!
                    if (!m_AllRenderers[0].enabled)
                    {
                        foreach (MeshRenderer zeRender in m_AllRenderers)
                        {
                            zeRender.enabled = true;
                        }
                    }
                    break;
            }
        }
        get
        {
            return m_VisibilityCount;
        }
    }

    protected void Awake()
    {
        m_AllRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    public override void DecreaseVisibility()
    {
        --VisibilityCount;
    }

    public override void IncreaseVisibility()
    {
        ++VisibilityCount;
    }

    /// <summary>
    /// Need this to check whether it renders itself or not!
    /// </summary>
    public override void SetVisibleTiles()
    {
        VisibilityCount = VisibilityCount;
    }

    /// <summary>
    /// Checks whether is the Enemy is visible or not
    /// </summary>
    /// <returns></returns>
    public override bool IsVisible()
    {
        return VisibilityCount != 0;
    }
}