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
    [SerializeField, Tooltip("Image of the unit")]
    protected Image m_UnitUI_Image;
    [SerializeField, Tooltip("Layout group of the unit's UI")]
    protected VerticalLayoutGroup m_UnitLayoutUI;
    [SerializeField, Tooltip("Finished button")]
    protected Button m_FinishedButton;

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
}
