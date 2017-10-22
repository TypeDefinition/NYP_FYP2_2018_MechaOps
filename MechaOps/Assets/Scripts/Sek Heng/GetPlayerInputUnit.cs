using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A simple testing to get the player to 
/// </summary>
public class GetPlayerInputUnit : MonoBehaviour {
    [Header("Values and objects that need to linked!")]
    [Tooltip("The holder for the unit's actions!")]
    public Image m_UnitActionUIIconGO;

    [Header("Debugging purposes!")]
    [Tooltip("The player clicked on the unit")]
    public GameObject m_ClickedPlayerUnitGO;
    [Tooltip("The number of image icons beneath it")]
    public List<Image> m_AllOfUnitUIIcon;
    [Tooltip("The flag to allow the player to select unit when in certain unit's action")]
    public bool m_CanSelectUnit = true;

    /// <summary>
    /// Setting the bool flag of selection of unit to be true!
    /// </summary>
    void ToggleSelectionOfUnit()
    {
        m_CanSelectUnit = !m_CanSelectUnit;
    }

    private void OnEnable()
    {
        ObserverSystemScript.Instance.SubscribeEvent("ToggleSelectingUnit", ToggleSelectionOfUnit);
    }

    private void OnDisable()
    {
        ObserverSystemScript.Instance.UnsubscribeEvent("ToggleSelectingUnit", ToggleSelectionOfUnit);
    }

    private void Start()
    {
        // if there is any image UI on standby, use that!
        m_AllOfUnitUIIcon = new List<Image>(GetComponentsInChildren<Image>());
    }

    // Update is called once per frame
    void Update () {
        // Touch Input can also use GetMouseButton(0)!
        if (m_CanSelectUnit && Input.GetMouseButton(0))
        {
            Ray clickedRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit clickedObj;
            if (Physics.Raycast(clickedRay, out clickedObj))
            {
                UnitAction[] allPossibleUnitActions = clickedObj.collider.GetComponentsInChildren<UnitAction>();
                // Instantiate more unit actions if there is not enough actions!
                while (m_AllOfUnitUIIcon.Count < allPossibleUnitActions.Length)
                {
                    Image instantiatedUnitIcon = Instantiate(m_UnitActionUIIconGO, transform);
                    instantiatedUnitIcon.gameObject.SetActive(true);
                    m_AllOfUnitUIIcon.Add(instantiatedUnitIcon);
                }
                // Once clicked on the unit, transform the unit's sprite into actual icon!
                for (int num = 0; num < m_AllOfUnitUIIcon.Count; ++num)
                {
                    if (num < allPossibleUnitActions.Length)
                    {
                        // Then assign the sprite there!
                        m_AllOfUnitUIIcon[num].gameObject.SetActive(true);
                        m_AllOfUnitUIIcon[num].sprite = allPossibleUnitActions[num].m_ActionIconUI;
                        m_AllOfUnitUIIcon[num].GetComponent<UnitActionUILogic>().m_unitActionRef = allPossibleUnitActions[num];
                    }
                    else
                    {
                        m_AllOfUnitUIIcon[num].gameObject.SetActive(false);
                    }
                }
            }
        }
	}

    /// <summary>
    /// A simple function which sets all current function 
    /// </summary>
    public void SetAllUnitActionIconInactive()
    {
        foreach (Image zeUIImg in m_AllOfUnitUIIcon)
        {
            zeUIImg.gameObject.SetActive(false);
        }
    }
}
