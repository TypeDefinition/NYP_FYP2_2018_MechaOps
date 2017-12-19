using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This is responsible for checking for when the player clicks a unit.
/// </summary>
[DisallowMultipleComponent]
public class DetectClickedPlayerUnit : MonoBehaviour
{
    [Tooltip("This is shown in the Inspector for debugging purposes.")]
    [SerializeField] private GameObject m_ClickedPlayerUnit = null;
    [SerializeField] private string[] m_LayersToCheck = { "Unit", "Tile" };
    [SerializeField] private int m_PointerId = 0;

    public GameObject ClickedPlayerUnit
    {
        get { return m_ClickedPlayerUnit; }
    }

    public string[] LayersToCheck
    {
        get { return m_LayersToCheck; }
    }

    public int PointerId
    {
        get { return m_PointerId; }
    }

    // Update is called once per frame.
    void Update ()
    {
        // Touch Input can also use GetMouseButton(0)!
        if (!Input.GetMouseButton(m_PointerId)) { return; }
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