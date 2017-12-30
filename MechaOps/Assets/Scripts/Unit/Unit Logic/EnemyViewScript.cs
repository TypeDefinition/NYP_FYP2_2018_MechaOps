using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will be used to render this unit invisible! inherit from PlayerViewScript to be lazy
/// </summary>
[DisallowMultipleComponent]
public class EnemyViewScript : ViewScript
{
    [Header("Debugging for EnemyViewTile")]
    [SerializeField, Tooltip("Enemy Visiblity counter. Do not touch")]
    protected int m_VisibilityCount = 0;
    [SerializeField, Tooltip("The array of MeshRenderer inside this unit")]
    protected MeshRenderer[] m_AllRenderers;
    [SerializeField, Tooltip("Flag to ensure that the renderer will always be active. So that the unit dies, it will always be rendering!")]
    protected bool m_AlwayRender = false;

    public int VisibilityCount
    {
        set
        {
            m_VisibilityCount = Mathf.Max(0, value);
            switch (m_VisibilityCount)
            {
                case 0:
                    // if the render is active to begin with, make everything else inactive including itself!
                    if (m_AllRenderers[0].enabled && !m_AlwayRender)
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

    protected override void Awake()
    {
        base.Awake();
        m_AllRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    private void OnEnable()
    {
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject, bool>("EnemyUnitIsDead", UnitDied);
    }

    private void OnDisable()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject, bool>("EnemyUnitIsDead", UnitDied);
    }

    public override void DecreaseVisibility()
    {
        --VisibilityCount;
        if (VisibilityCount == 0)
        {
            m_UnitStats.UnitInfoDisplayUI.gameObject.SetActive(false);
            GameEventSystem.GetInstance().TriggerEvent<GameObject>("UnitUnseen", gameObject);
        }
    }

    public override void IncreaseVisibility()
    {
        ++VisibilityCount;
        if (VisibilityCount == 1)
        {
            m_UnitStats.UnitInfoDisplayUI.gameObject.SetActive(true);
            GameEventSystem.GetInstance().TriggerEvent<GameObject>("UnitSeen", gameObject);
        }
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

    public override void Initialise()
    {
        if (VisibilityCount == 0)
        {
            m_UnitStats.UnitInfoDisplayUI.gameObject.SetActive(false);
        }
    }

    protected void UnitDied(GameObject _UnitGO, bool _IsVisible)
    {
        if (_UnitGO == gameObject)
        {
            m_AlwayRender = true;
            // ensuring that the renderer will be set from inactive to active
            if (!m_AllRenderers[0].enabled)
            {
                foreach (MeshRenderer zeRender in m_AllRenderers)
                {
                    zeRender.enabled = true;
                }
            }
        }
    }
}