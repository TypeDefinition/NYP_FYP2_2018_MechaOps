using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// To show the tile info on the UI logic
/// </summary>
public class ShowTileInfoUI : MonoBehaviour
{
    // Serialized Variable(s)
    [Header("Variables for ShowTileInfoUI")]
    [SerializeField, Tooltip("Tile information gameobject")]
    protected GameObject m_TileInfoGO;
    [SerializeField, Tooltip("Minimize button")]
    protected Button m_MinimizeButton;
    [SerializeField, Tooltip("Expand button")]
    protected Button m_ExpandButton;
    [SerializeField, Tooltip("Text UI for the display of tile name")]
    protected TextMeshProUGUI m_TileNameTxt;
    [SerializeField, Tooltip("Text UI for tile movement cost")]
    protected TextMeshProUGUI m_MoveCostTxt;
    [SerializeField, Tooltip("Text UI for tile concealment")]
    protected TextMeshProUGUI m_TileConcealTxt;
    [SerializeField, Tooltip("Text UI for tile evasion")]
    protected TextMeshProUGUI m_TileEvasionTxt;
    [SerializeField, Tooltip("Text UI for tile description")]
    protected TextMeshProUGUI m_TileDescriptionTxt;
    [SerializeField, Tooltip("Audio to play the minimizing sound effect")]
    protected AudioClip m_MinimizeSFX;
    [SerializeField, Tooltip("Audio to play the expand sound effect")]
    protected AudioClip m_ExpandSFX;
    [SerializeField, Tooltip("Tweening to disable this script")]
    protected TweenDisableScript m_TweenDisableScript;

    // Non-Serialized Variable(s)
    protected GameEventNames m_GameEventNames;
    protected AudioSource m_SFXAudioSource;

    // [Header("Debugging for ShowTileInfoUI")]
    // [SerializeField, Tooltip("Tile that the player has clicked!")]
    protected Tile m_ClickedTile;

    private void Awake()
    {
        m_GameEventNames = GameSystemsDirectory.GetSceneInstance().GetGameEventNames();
        Assert.IsNotNull(m_GameEventNames, "Event asset is still null at ShowTileInfoUI.Awake()");
        m_SFXAudioSource = GameSystemsDirectory.GetSceneInstance().GetSFXAudioSource();
        Assert.IsNotNull(m_SFXAudioSource);
        Assert.IsNotNull(m_TweenDisableScript, "The tween disable script is null at ShowTileInfoUI.Awake()");

        // Ensure that the text references are not null.
        Assert.IsNotNull(m_TileNameTxt);
        Assert.IsNotNull(m_MoveCostTxt);
        Assert.IsNotNull(m_TileConcealTxt);
        Assert.IsNotNull(m_TileEvasionTxt);
        Assert.IsNotNull(m_TileDescriptionTxt);

        InitEvents();
    }

    private void Start()
    {
        // Set it back to inactive afterwards
        m_TileInfoGO.SetActive(false);
    }

    protected void InitEvents()
    {
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.ClickedTile), GetClickedTileGO);
    }

    protected void DeinitEvents()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.ClickedTile), GetClickedTileGO);
    }

    /// <summary>
    /// We are not using OnDisable because this gameobject will become active and inactive repeatedly
    /// </summary>
    private void OnDestroy()
    {
        DeinitEvents();
    }

    protected void GetClickedTileGO(GameObject _tileGO)
    {
        if (_tileGO.tag == "Tile" || _tileGO.tag == "TileDisplay")
        {
            if (_tileGO.tag == "TileDisplay")
            {
                // get the parent since parent will be the tile!
                m_ClickedTile = _tileGO.transform.parent.GetComponent<Tile>();
            }
            else
            {
                m_ClickedTile = _tileGO.GetComponent<Tile>();
            }
            // we need to know whether is it a known / unknown tile
            if (!m_ClickedTile.Known)
            {
                m_TileNameTxt.text = "Unknown Tile";
                m_MoveCostTxt.text = "Movement Cost: ?";
                m_TileConcealTxt.text = "Concealment: ?";
                m_TileEvasionTxt.text = "Evasion: ?";
                m_TileDescriptionTxt.text = "";
            }
            else
            {
                // then set the values for display!
                m_TileNameTxt.text = m_ClickedTile.GetTileType().ToString() + " Tile";
                m_TileConcealTxt.text = "Concealment: " + m_ClickedTile.GetTotalConcealmentPoints();
                m_MoveCostTxt.text = "Movement Cost: " + m_ClickedTile.GetTotalMovementCost();
                m_TileEvasionTxt.text = "Evasion: " + m_ClickedTile.GetTileAttributes().EvasionPoints;
                m_TileDescriptionTxt.text = m_ClickedTile.GetTileAttributes().Description + "\n";

                // check whether it is visible to the player otherwise not much information will be shown
                if (m_ClickedTile.IsVisible())
                {
                    Hazard hazard = m_ClickedTile.GetHazard();
                    if (hazard != null)
                    {
                        m_TileDescriptionTxt.text += hazard.Attributes.Description;
                    }
                }
            }
        }
    }

    /// <summary>
    /// When player pressed the close button on this UI
    /// </summary>
    public void MinimizeThis()
    {
        m_MinimizeButton.gameObject.SetActive(false);
        m_ExpandButton.gameObject.SetActive(true);
        AnimateToInactive();
        m_SFXAudioSource.PlayOneShot(m_MinimizeSFX);
    }

    public void ExpandThis()
    {
        m_MinimizeButton.gameObject.SetActive(true);
        m_ExpandButton.gameObject.SetActive(false);
        m_TileInfoGO.SetActive(true);
        m_SFXAudioSource.PlayOneShot(m_ExpandSFX);
    }

    /// <summary>
    /// Animating the tile info UI to inactive automatically
    /// </summary>
    public void AnimateToInactive()
    {
        m_TweenDisableScript.AnimateUI();
        m_ClickedTile = null;
    }
}