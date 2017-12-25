using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The system that handles the spawning of units
/// </summary>
[RequireComponent(typeof(DetectPlayerClicks))]
public class SpawnUnitSystem : MonoBehaviour {
    [Header("Variables for SpawnUnitSystem")]
    [SerializeField, Tooltip("GameSystemDirectory")]
    protected GameSystemsDirectory m_GameSystem;
    [SerializeField, Tooltip("Data container for the asset data")]
    protected UnitDataAndCost m_UnitsDataAsset;
    [SerializeField, Tooltip("The UI prefab to be instantiated")]
    protected GameObject m_SelectUnitUIPrefab;
    [SerializeField, Tooltip("Button prefab for selecting the unit")]
    protected Button m_SelectUnitButtonPrefab;
    [SerializeField, Tooltip("Indicator prefab for spawning of the units")]
    protected GameObject m_UnitSpawnIndicatorPrefab;
    [SerializeField, Tooltip("Amount of credit for the player to spend")]
    protected int m_PlayerCredits;

    [Header("Debugging for SpawnUnitSystem")]
    [SerializeField, Tooltip("Name of the selected unit from the UI")]
    protected string m_CurrentSelectedUnitName;
    [SerializeField, Tooltip("Reference to the instantiated prefab")]
    protected GameObject m_InstantiateSelectUnitUI;
    [SerializeField, Tooltip("Script that handles the spawn UI logic. Should be attached to m_InstantiateSelectUnitUI")]
    protected SpawnUnitUI_Logic m_SpawnUI_Logic;
    [SerializeField, Tooltip("List of player units that will be spawn later")]
    protected List<SpawnIndicator_Logic> m_ListOfSpawnUnits = new List<SpawnIndicator_Logic>();

    protected HashSet<TileId> m_TileSets = new HashSet<TileId>();

    public int PlayerCredits
    {
        set
        {
            m_PlayerCredits = value;
            m_SpawnUI_Logic.PlayerCreditText = m_PlayerCredits.ToString();
        }
        get
        {
            return m_PlayerCredits;
        }
    }

    /// <summary>
    /// To subscribe to these events
    /// </summary>
    private void OnEnable()
    {
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("ClickedTile", SetUnitPosition);
        GameEventSystem.GetInstance().SubscribeToEvent<GameObject>("ClickedSpawnIndicator", RemoveUnitFromSpawn);
    }

    /// <summary>
    /// To unsubscribe from the events
    /// </summary>
    private void OnDisable()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("ClickedTile", SetUnitPosition);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<GameObject>("ClickedSpawnIndicator", RemoveUnitFromSpawn);
    }

    // Use this for initialization
    void Start () {
        m_InstantiateSelectUnitUI = Instantiate(m_SelectUnitUIPrefab, m_GameSystem.GetScreenSpaceCanvas().transform);
        // Need to make sure it will always be active
        m_InstantiateSelectUnitUI.SetActive(true);
        m_SpawnUI_Logic = m_InstantiateSelectUnitUI.GetComponent<SpawnUnitUI_Logic>();
        Transform zeVerticalLayoutTransform = m_SpawnUI_Logic.UnitLayoutUI.transform;
        // iterate through the unit UI array and spawn the UI buttons
        foreach (UnitDataAndCost.UnitUI_Data zeUnitUI in m_UnitsDataAsset.UnitUIDataArray)
        {
            GameObject zeInstantiateButtonGO = Instantiate(m_SelectUnitButtonPrefab.gameObject, zeVerticalLayoutTransform, false);
            // and then set the necessary prefabs and values for it!
            zeInstantiateButtonGO.GetComponent<Image>().sprite = zeUnitUI.m_UnitSpriteUI;
            zeInstantiateButtonGO.GetComponent<Button>().onClick.AddListener(() =>SetUnitToSpawn(zeUnitUI.m_TypeName, zeUnitUI.m_UnitSpriteUI));
        }
        // Make sure the finished button has the startspawning units function
        m_SpawnUI_Logic.FinishedButton.onClick.AddListener(StartSpawningUnits);
        m_SpawnUI_Logic.PlayerCreditText = m_PlayerCredits.ToString();
    }

    /// <summary>
    /// When the tile is clicked, it will record down
    /// </summary>
    /// <param name="_go">Clicked tile gameobject</param>
    void SetUnitPosition(GameObject _go)
    {
        // need to ensure that the tiles and it is the correct layer
        if (m_CurrentSelectedUnitName == "" || LayerMask.NameToLayer("SpawnIndicator") == _go.layer)
            return;
        Tile zeTile = _go.GetComponent<Tile>();
        // ensure it will not duplicate the spawn indicator on the same tiles
        if (m_TileSets.Contains(zeTile.GetTileId()))
            return;
        int zeUnitCost = m_UnitsDataAsset.GetUnitCost(m_CurrentSelectedUnitName);
        if (m_PlayerCredits < zeUnitCost)
        {
            m_SpawnUI_Logic.SetInsufficientUI_Active();
            return;
        }
        PlayerCredits -= zeUnitCost;
        m_TileSets.Add(zeTile.GetTileId());
        SpawnIndicator_Logic zeNewData;
        zeNewData = Instantiate(m_UnitSpawnIndicatorPrefab.gameObject, _go.transform, true).GetComponent<SpawnIndicator_Logic>();
        zeNewData.gameObject.SetActive(true);
        // set the position of this UI there
        Vector3 zePositionOfGO = _go.transform.position;
        zePositionOfGO.y = zeNewData.transform.position.y;
        zeNewData.transform.position = zePositionOfGO;

        m_ListOfSpawnUnits.Add(zeNewData);
        zeNewData.TileID = zeTile.GetTileId();
        zeNewData.TypeNameTextString = m_CurrentSelectedUnitName;
        zeNewData.GetComponent<TweenUI_Scale>().AnimateUI();
    }

    /// <summary>
    /// To remove the units from the tile spawn
    /// </summary>
    /// <param name="_go">The tile that will be spawn</param>
    void RemoveUnitFromSpawn(GameObject _go)
    {
        SpawnIndicator_Logic zeSpawnLogic = _go.GetComponent<SpawnIndicator_Logic>();
        if (zeSpawnLogic)
        {
            zeSpawnLogic.RemovedUnitSpawnUI.gameObject.SetActive(!zeSpawnLogic.RemovedUnitSpawnUI.gameObject.activeSelf);
        }
        else
        {
            // if it is the cancel object
            zeSpawnLogic = _go.transform.parent.GetComponent<SpawnIndicator_Logic>();
            // remove the data completely and the tile id
            m_ListOfSpawnUnits.Remove(zeSpawnLogic);
            m_TileSets.Remove(zeSpawnLogic.TileID);
            // then destroy that game object
            Destroy(zeSpawnLogic.gameObject);
            PlayerCredits += m_UnitsDataAsset.GetUnitCost(zeSpawnLogic.TypeNameTextString);
        }
    }

    /// <summary>
    /// When the unit UI button is pressed
    /// </summary>
    /// <param name="_UnitTypeName">The Typename of the unit</param>
    /// <param name="_UnitUI_Sprite">Unit UI Sprite</param>
    void SetUnitToSpawn(string _UnitTypeName, Sprite _UnitUI_Sprite)
    {
        m_CurrentSelectedUnitName = _UnitTypeName;
        m_SpawnUI_Logic.UnitTypenameText = _UnitTypeName;
        m_SpawnUI_Logic.UnitUI_ImageSprite = _UnitUI_Sprite;
        m_SpawnUI_Logic.UnitCostText = m_UnitsDataAsset.GetUnitCost(_UnitTypeName).ToString();
    }

    /// <summary>
    /// When the player pressed the finish button
    /// </summary>
    void StartSpawningUnits()
    {
        foreach (SpawnIndicator_Logic zeSpawnData in m_ListOfSpawnUnits)
        {
            // spawn / instantiate the unit accordingly
            GameObject zeUnitGO = Instantiate(m_UnitsDataAsset.GetUnitGO(zeSpawnData.TypeNameTextString));
            // then set the unit's stats accordingly
            UnitStats zeUnitStat = zeUnitGO.GetComponent<UnitStats>();
            zeUnitStat.CurrentTileID = zeSpawnData.TileID;
            zeUnitGO.transform.position = new Vector3(zeSpawnData.transform.position.x, zeUnitGO.transform.position.y, zeSpawnData.transform.position.z);
            Destroy(zeSpawnData.gameObject);
        }
        // and then set the gamesystem to be active
        m_GameSystem.gameObject.SetActive(true);
        // destroy the gameobject that this is attached to when it is done
        Destroy(gameObject);
    }

    /// <summary>
    /// When this component is destroyed, destroy the UI too!
    /// </summary>
    private void OnDestroy()
    {
        Destroy(m_InstantiateSelectUnitUI);
    }
}
