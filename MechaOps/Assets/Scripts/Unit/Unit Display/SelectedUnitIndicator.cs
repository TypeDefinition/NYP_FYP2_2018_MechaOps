using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedUnitIndicator : TweenUI_Scale
{
    public GameObject m_Unit = null;

    private void Update()
    {
        Canvas screenSpaceCanvas = GameSystemsDirectory.GetSceneInstance().GetClickableScreenSpaceCanvas();
        Camera gameCamera = GameSystemsDirectory.GetSceneInstance().GetGameCamera();
        Vector3 screenPoint = gameCamera.WorldToScreenPoint(m_Unit.transform.position);
        screenPoint.z = 0.0f;
        transform.position = screenPoint;
    }
}