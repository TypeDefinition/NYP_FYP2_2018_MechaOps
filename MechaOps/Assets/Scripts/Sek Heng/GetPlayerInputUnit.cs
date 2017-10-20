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
    public Image unitActionUIIconGO;

    [Header("Debugging purposes!")]
    [Tooltip("The player clicked on the unit")]
    public GameObject clickedPlayerUnitGO;
    [Tooltip("The number of image icons beneath it")]
    public List<Image> allOfUnitUIIcon;

    static void activateGameObjWithTag(string objTagName)
    {
        GameObject.FindGameObjectWithTag(objTagName).SetActive(true);
    }

    private void Start()
    {
        // if there is any image UI on standby, use that!
        allOfUnitUIIcon = new List<Image>(GetComponentsInChildren<Image>());
    }

    // Update is called once per frame
    void Update () {
        // Touch Input can also use GetMouseButton(0)!
        if (Input.GetMouseButton(0))
        {
            Ray clickedRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit clickedObj;
            if (Physics.Raycast(clickedRay, out clickedObj))
            {
                UnitAction[] allPossibleUnitActions = clickedObj.collider.GetComponentsInChildren<UnitAction>();
                // Instantiate more unit actions if there is not enough actions!
                while (allOfUnitUIIcon.Count < allPossibleUnitActions.Length)
                {
                    Image instantiatedUnitIcon = Instantiate(unitActionUIIconGO, transform);
                    instantiatedUnitIcon.gameObject.SetActive(true);
                    allOfUnitUIIcon.Add(instantiatedUnitIcon);
                }
                // Once clicked on the unit, transform the unit's sprite into actual icon!
                for (int num = 0; num < allOfUnitUIIcon.Count; ++num)
                {
                    if (num < allPossibleUnitActions.Length)
                    {
                        // Then assign the sprite there!
                        allOfUnitUIIcon[num].gameObject.SetActive(true);
                        allOfUnitUIIcon[num].sprite = allPossibleUnitActions[num].actionIconUI;
                        allOfUnitUIIcon[num].GetComponent<UnitActionUILogic>().m_unitActionRef = allPossibleUnitActions[num];
                    }
                    else
                    {
                        allOfUnitUIIcon[num].gameObject.SetActive(false);
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
        foreach (Image zeUIImg in allOfUnitUIIcon)
        {
            zeUIImg.gameObject.SetActive(false);
        }
    }
}
