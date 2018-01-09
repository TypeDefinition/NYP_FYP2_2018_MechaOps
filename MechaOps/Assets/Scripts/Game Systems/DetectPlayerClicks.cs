using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This is responsible for checking for when the player clicks a unit.
/// </summary>
public class DetectPlayerClicks : MonoBehaviour
{
    [SerializeField]
    private int m_PointerId = 0;
    [SerializeField]
    private bool m_IgnoreClicksOnUI = true;
    [SerializeField]
    private GameEventNames m_GameEventNames = null;
    [SerializeField, Tooltip("The event that is triggered when an object is clicked on.")]
    private GameEventNames.GameUINames m_TriggeredEvent = GameEventNames.GameUINames.ClickedUnit;
    [SerializeField]
    private string[] m_LayersToDetect = { "Unit" };

    public int PointerId
    {
        get { return m_PointerId; }
    }

    public bool IgnoreUI
    {
        get { return m_IgnoreClicksOnUI; }
    }

    public string[] LayersToDetect
    {
        get { return m_LayersToDetect; }
    }

    // Update is called once per frame.
    void Update ()
    {
        // Touch Input can also use GetMouseButton(0)!
        if (!Input.GetMouseButtonDown(m_PointerId)) { return; }
        // Make sure that the pointer is not over some canvas UI!
#if UNITY_EDITOR
        if (m_IgnoreClicksOnUI && EventSystem.current.IsPointerOverGameObject()) { return; }
#else
        if (m_IgnoreClicksOnUI && EventSystem.current.IsPointerOverGameObject(m_PointerId)) { return; }
#endif

        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask(m_LayersToDetect)))
        {
            GameEventSystem.GetInstance().TriggerEvent<GameObject>(m_GameEventNames.GetEventName(m_TriggeredEvent), hitInfo.collider.gameObject);
        }
	}
}