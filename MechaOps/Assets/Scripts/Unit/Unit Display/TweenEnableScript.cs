using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Just Scale the UI when gameobject or this component is enabled
/// </summary>
public class TweenEnableScript : TweenUI_Scale {
    private void OnEnable()
    {
        AnimateUI();
    }
}
