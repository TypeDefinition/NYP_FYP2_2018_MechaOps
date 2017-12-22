using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUnitDisplay : TweenUI_Scale {
    [Tooltip("The gameobject which this UI needs to be attached to")]
    public GameObject m_UnitGO;
    [Tooltip("Game system directory")]
    public GameSystemsDirectory m_GameSystemsDirectory;

    private void Update()
    {
        Canvas screenSpaceCanvas = m_GameSystemsDirectory.GetScreenSpaceCanvas();
        Camera gameCamera = m_GameSystemsDirectory.GetGameCamera();
        Vector3 screenPoint = gameCamera.WorldToScreenPoint(m_UnitGO.transform.position);
        screenPoint.z = transform.position.z;
        transform.position = screenPoint;
    }
}
