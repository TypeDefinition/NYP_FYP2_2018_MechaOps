using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The logic for artillery attack UI
/// </summary>
public class ArtyAttackUI_Logic : TweenUI_Scale {
    [Header("Variables for ArtyAttackUI_Logic")]
    [SerializeField, Tooltip("prefab UI indicator of the tile to be targeted")]
    protected GameObject m_UIndicatorGO;

    [Header("Debugging for ArtyAttackUI")]
    [SerializeField, Tooltip("Unit's attack action")]
    protected ArtyAttackAct m_AttckAct;
    [SerializeField, Tooltip("The targeted Tile")]
    protected Tile m_TargetTile;
    [SerializeField, Tooltip("Tile System")]
    protected TileSystem m_TileSys;
    [SerializeField, Tooltip("List of Tiles that it can attack")]
    protected List<TileId> m_AttackableTiles;
    [SerializeField, Tooltip("The world canvas UI")]
    protected GameObject m_worldCanvas;
    [SerializeField, Tooltip("Instantiated UI")]
    protected UnitDisplayUI m_InstantUI;

    private void OnEnable()
    {
        m_TileSys = FindObjectOfType<TileSystem>();
        m_worldCanvas = GameObject.FindGameObjectWithTag("WorldCanvas");
        m_InstantUI = Instantiate(m_UIndicatorGO, m_worldCanvas.transform, false).GetComponent<UnitDisplayUI>();
        m_InstantUI.gameObject.SetActive(false);
        // Animate the UI when enabled
        AnimateUI();
        GameEventSystem.GetInstance().TriggerEvent("ToggleSelectingUnit");
        GameEventSystem.GetInstance().SubscribeToEvent<IUnitAction>("SelectedAction", PressedAction);
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("ClickedUnit", ClickedUnit);
    }

    private void OnDisable()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<IUnitAction>("SelectedAction", PressedAction);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("ClickedUnit", ClickedUnit);
        m_TileSys.ClearPathMarkers();
        Destroy(m_InstantUI.gameObject);
    }

    /// <summary>
    /// When pressing the back button
    /// </summary>
    public void Back()
    {
        // ensure that the player will be able to click on unit again!
        GameEventSystem.GetInstance().TriggerEvent("ToggleSelectingUnit");
        // there is no point in keeping this UI anymore so destroy it!
        Destroy(gameObject);
    }

    public void ConfirmAttack()
    {
        if (m_TargetTile)
        {
            UnitActionScheduler zeSchedule = FindObjectOfType<UnitActionScheduler>();
            m_AttckAct.SetTarget(m_TargetTile.gameObject);
            m_AttckAct.TurnOn();
            zeSchedule.ScheduleAction(m_AttckAct);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// To get the event message from the GameEventSystem
    /// </summary>
    /// <param name="_act"></param>
    public void PressedAction(IUnitAction _act)
    {
        m_AttckAct = _act as ArtyAttackAct;
        // then we highlight all of the tiles from here
        TileId []zeAllTile = m_TileSys.GetSurroundingTiles(m_AttckAct.m_UnitStats.CurrentTileID, m_AttckAct.MaxAttackRange);
        foreach (TileId zeTileID in zeAllTile)
        {
            int zeDist = TileId.GetDistance(m_AttckAct.m_UnitStats.CurrentTileID, zeTileID);
            // check if it is within the range
            if (zeDist >= m_AttckAct.MinAttackRange && zeDist <= m_AttckAct.MaxAttackRange)
            {
                m_AttackableTiles.Add(zeTileID);
            }
        }
        m_TileSys.SetPathMarkers(m_AttackableTiles.ToArray(), null);
    }
    /// <summary>
    /// When player clicked the tile or unit!
    /// </summary>
    /// <param name="_go"></param>
    protected void ClickedUnit(GameObject _go)
    {
        Tile zeTile = null;
        switch (_go.tag)
        {
            case "TileDisplay":
                // if it is the tile / obstacle
                zeTile = _go.transform.parent.GetComponent<Tile>();
                break;
            case "TileBase":
                zeTile = _go.GetComponent<Tile>();
                break;
            case "Player":
            case "EnemyUnit":
                // need this to make sure it will be able to access the TileBase
                // if it happens to click upon the units
                zeTile = m_TileSys.GetTile(_go.GetComponent<UnitStats>().CurrentTileID);
                break;
            default:
                print("Not the correct gameobject");
                break;
        }
        if (m_AttackableTiles.Contains(zeTile.GetId()))
        {
            m_TargetTile = zeTile;
            // and make sure the ID around it will be highlighted!
            TileId[] zeSurroundTargetTiles = m_TileSys.GetSurroundingTiles(zeTile.GetId(), m_AttckAct.ExplodeRadius);
            m_TileSys.SetPathMarkers(m_AttackableTiles.ToArray(), zeSurroundTargetTiles);
            m_InstantUI.gameObject.SetActive(true);
            m_InstantUI.AnimateUI();
            m_InstantUI.SetThePosToUnit(zeTile.transform);
        }
    }
}
