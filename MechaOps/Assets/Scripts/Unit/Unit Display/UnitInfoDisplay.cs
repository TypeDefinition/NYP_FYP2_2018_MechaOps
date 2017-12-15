using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

/// <summary>
/// meant to just display the stats UI only!
/// </summary>
public class UnitInfoDisplay : TweenUI_Scale
{
    [SerializeField]
    protected HealthBar m_HealthBar;
    [SerializeField, Tooltip("The action point text of the unit")]
    protected TextMeshProUGUI m_ActionPointsText;

    public HealthBar GetHealthBar() { return m_HealthBar; }
    public TextMeshProUGUI GetActionPointsText() { return m_ActionPointsText; }

    private void OnEnable()
    {
        AnimateUI();
    }

    protected override void Awake()
    {
        base.Awake();
        Assert.IsTrue(m_HealthBar != null, MethodBase.GetCurrentMethod().Name + " - m_HealthBar must not be null!");
        Assert.IsTrue(m_ActionPointsText != null, MethodBase.GetCurrentMethod().Name + " - m_ActionPointsText must not be null!");
    }

#if UNITY_EDITOR
    private void OnValidate() {}
#endif // UNITY_EDITOR

}
