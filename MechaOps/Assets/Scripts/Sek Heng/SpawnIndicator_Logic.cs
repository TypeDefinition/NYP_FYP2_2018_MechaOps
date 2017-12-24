using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// To remove the unit from the spawn when clicking on owner's gameobject
/// </summary>
public class SpawnIndicator_Logic : MonoBehaviour
{
    [Header("Variables for the spawn indicator")]
    [SerializeField, Tooltip("Remove unit sprite indicator")]
    protected TweenEnableScript m_RemovedUnitSpawnUI;
    [SerializeField, Tooltip("TextMeshPro text UI")]
    protected TextMeshPro m_TypeNameText;

    [Header("Debugging for SpawnIndicator_Logic")]
    [SerializeField, Tooltip("TileId of this spawn indicator")]
    protected TileId m_TileID;

    public TweenEnableScript RemovedUnitSpawnUI
    {
        get
        {
            return m_RemovedUnitSpawnUI;
        }
    }

    public string TypeNameTextString
    {
        set
        {
            m_TypeNameText.text = value;
        }
        get
        {
            return m_TypeNameText.text;
        }
    }

    public TileId TileID
    {
        set
        {
            m_TileID = value;
        }
        get
        {
            return m_TileID;
        }
    }
}
