using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This is responsible for checking for when the player clicks a unit.
/// </summary>
[DisallowMultipleComponent]
public class GetPlayerInputUnit : MonoBehaviour
{
    [Header("Debugging purposes!")]
    [Tooltip("The player clicked on the unit")]
    [SerializeField] private GameObject m_ClickedPlayerUnit = null;
    [SerializeField] private string[] m_LayersToCheck = { "Unit", "Tile" };

    public GameObject ClickedPlayerUnit
    {
        get { return m_ClickedPlayerUnit; }
        set { m_ClickedPlayerUnit = value; }
    }

    // Update is called once per frame.
    void Update ()
    {
        // Touch Input can also use GetMouseButton(0)!
        int pointerId = 0;
        if (!Input.GetMouseButton(pointerId)) { return; }
        // Make sure that the pointer is not over some canvas UI!
        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask(m_LayersToCheck)))
        {
            m_ClickedPlayerUnit = hitInfo.collider.gameObject;
            GameEventSystem.GetInstance().TriggerEvent<GameObject>("ClickedUnit", m_ClickedPlayerUnit);
        }
	}
}