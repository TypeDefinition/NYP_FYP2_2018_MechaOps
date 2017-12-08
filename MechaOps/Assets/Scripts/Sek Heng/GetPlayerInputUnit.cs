using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A simple testing to get the player to 
/// </summary>
public class GetPlayerInputUnit : MonoBehaviour {
    [Header("Debugging purposes!")]
    [Tooltip("The player clicked on the unit")]
    public GameObject m_ClickedPlayerUnitGO;

    // Update is called once per frame
    void Update () {
        // Touch Input can also use GetMouseButton(0)! and making sure that the pointer is not over some canvas UI!
        // EventSystem.alreadySelecting doesn't work. EventSystem.IsPointerOverGameObject() doesn't work too
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            Ray clickedRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit clickedObj;
            List<string> layersToCheck = new List<string>();
            layersToCheck.Add("Unit");
            layersToCheck.Add("Tile");
            int layerMask = LayerMask.GetMask(layersToCheck.ToArray());
            if (Physics.Raycast(clickedRay, out clickedObj, Mathf.Infinity, layerMask))
            {
                m_ClickedPlayerUnitGO = clickedObj.collider.gameObject;
                GameEventSystem.GetInstance().TriggerEvent<GameObject>("ClickedUnit", m_ClickedPlayerUnitGO);
            }
        }
	}
}
