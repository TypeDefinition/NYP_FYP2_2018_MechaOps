using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The system that handles the spawning of units
/// </summary>
public class SpawnUnitSystem : MonoBehaviour {
    /// <summary>
    /// To record down where will the units be spawning
    /// </summary>
    [System.Serializable]
    public struct UnitSpawnData
    {
        public TileId m_UnitSpawnArea;
        public string m_TypeName;
    }
    [Header("Variables for SpawnUnitSystem")]
    [SerializeField, Tooltip("GameSystemDirectory")]
    protected GameSystemsDirectory m_GameSystem;
    [SerializeField, Tooltip("Data container for the asset data")]
    protected UnitDataAndCost m_UnitsDataAsset;
    [SerializeField, Tooltip("The UI prefab to be instantiated")]
    protected GameObject m_SelectUnitUIPrefab;
    [SerializeField, Tooltip("Button prefab for selecting the unit")]
    protected Button m_SelectUnitButtonPrefab;
    [SerializeField, Tooltip("Name of the the vertical layout group to put in")]
    protected string m_TagOfLayout = "GroupingImage";
    [SerializeField, Tooltip("The name of child button of the m_SelectUnitUI")]
    protected string m_NameOfFinishButton = "Finished";
    [SerializeField, Tooltip("Amount of credit for the player to spend")]
    protected int m_PlayerCredits;

    [Header("Debugging for SpawnUnitSystem")]
    [SerializeField, Tooltip("Name of the selected unit from the UI")]
    protected string m_CurrentSelectedUnitName;
    [SerializeField, Tooltip("Reference to the instantiated prefab")]
    protected GameObject m_InstantiateSelectUnitUI;
    [SerializeField, Tooltip("List of player units that will be spawn later")]
    protected List<UnitSpawnData> m_ListOfSpawnUnits = new List<UnitSpawnData>();

	// Use this for initialization
	void Start () {
        m_InstantiateSelectUnitUI = Instantiate(m_SelectUnitUIPrefab, m_GameSystem.GetScreenSpaceCanvas().transform);
        Transform zeVerticalLayoutTransform = GameObject.FindGameObjectWithTag(m_TagOfLayout).transform;
        foreach (UnitDataAndCost.UnitUI_Data zeUnitUI in m_UnitsDataAsset.UnitUIDataArray)
        {
            GameObject zeInstantiateButtonGO = Instantiate(m_SelectUnitButtonPrefab.gameObject, zeVerticalLayoutTransform, false);
            // and then set the necessary prefabs and values for it!
            zeInstantiateButtonGO.GetComponent<Image>().sprite = zeUnitUI.m_UnitSpriteUI;
            zeInstantiateButtonGO.GetComponent<Button>().onClick.AddListener(() =>SetUnitToSpawn(zeUnitUI.m_TypeName));
        }
        // Make sure the finished button has the startspawning units function
        m_InstantiateSelectUnitUI.transform.Find(m_NameOfFinishButton).GetComponent<Button>().onClick.AddListener(StartSpawningUnits);
	}

    /// <summary>
    /// When the tile is clicked, it will record down
    /// </summary>
    /// <param name="_go"></param>
    void SetUnitPosition(GameObject _go)
    {

    }

    /// <summary>
    /// When the unit UI button is pressed
    /// </summary>
    /// <param name="_UnitTypeName">The Typename of the unit</param>
    void SetUnitToSpawn(string _UnitTypeName)
    {

    }

    /// <summary>
    /// When the player pressed the finish button
    /// </summary>
    void StartSpawningUnits()
    {
        // destroy the gameobject that this is attached to when it is done
        Destroy(gameObject);
    }
}
