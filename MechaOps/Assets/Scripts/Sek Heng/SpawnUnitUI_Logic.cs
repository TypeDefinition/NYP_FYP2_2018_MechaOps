using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Set the current selected image and name to the player selected unit!
/// </summary>
public class SpawnUnitUI_Logic : MonoBehaviour {
    [Header("Variable for SpawnUnitUI_Logic")]
    [SerializeField, Tooltip("Text UI of the unit type name")]
    protected TextMeshProUGUI m_TMProTextUI;
    [SerializeField, Tooltip("Text for the player credits")]
    protected TextMeshProUGUI m_TMProCreditText;
    [SerializeField, Tooltip("Image of the unit")]
    protected Image m_UnitUI_Image;
    [SerializeField, Tooltip("Layout group of the unit's UI")]
    protected VerticalLayoutGroup m_UnitLayoutUI;
    [SerializeField, Tooltip("Finished button")]
    protected Button m_FinishedButton;
    [SerializeField, Tooltip("To signal insufficient credit UI")]
    protected TweenDisableScript m_InsufficientCreditUI;
    [SerializeField, Tooltip("Time taken to disable Insufficient credit UI")]
    protected float m_TimeToDisableInsufficientUI = 2.0f;

    Coroutine m_InsufficientUICoroutine;

    public Sprite UnitUI_ImageSprite
    {
        set
        {
            // need to ensure that the image color is normal
            if (m_UnitUI_Image.color.a < 255)
            {
                // change the alpha to be 255
                m_UnitUI_Image.color = new Color(m_UnitUI_Image.color.r, m_UnitUI_Image.color.g, m_UnitUI_Image.color.b, 255);
            }
            m_UnitUI_Image.sprite = value;
        }
        get
        {
            return m_UnitUI_Image.sprite;
        }
    }

    public string UnitTypenameText
    {
        set
        {
            m_TMProTextUI.text = value;
        }
        get
        {
            return m_TMProTextUI.text;
        }
    }

    public string PlayerCreditText
    {
        set
        {
            m_TMProCreditText.text = value;
        }
        get
        {
            return m_TMProCreditText.text;
        }
    }

    public VerticalLayoutGroup UnitLayoutUI
    {
        get
        {
            return m_UnitLayoutUI;
        }
    }

    public Button FinishedButton
    {
        get
        {
            return m_FinishedButton;
        }
    }

    /// <summary>
    /// It will only
    /// </summary>
    public void SetInsufficientUI_Active()
    {
        if (m_InsufficientUICoroutine == null)
        {
            m_InsufficientCreditUI.gameObject.SetActive(true);
            m_InsufficientUICoroutine = StartCoroutine(SetInsufficientUICoroutine());
        }
    }

    IEnumerator SetInsufficientUICoroutine()
    {
        yield return new WaitForSeconds(m_TimeToDisableInsufficientUI);
        m_InsufficientCreditUI.AnimateUI();
        m_InsufficientUICoroutine = null;
        yield break;
    }
}
