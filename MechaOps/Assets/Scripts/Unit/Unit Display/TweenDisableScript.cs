using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// set the gameobject inactive when the credit is insufficient
/// </summary>
public class TweenDisableScript : TweenUI_Scale {
    /// <summary>
    /// Need to ensure that it does not animate UI when enabled!
    /// </summary>
    public override void OnEnable()
    {
    }

    public override void AnimateUI()
    {
        // it will set the UI to be inactive when AnimateUI
        LeanTween.scaleX(gameObject, 0, m_TweenAnimationDuration).setOnComplete(SetGameObjInactive);
    }

    protected void SetGameObjInactive()
    {
        gameObject.SetActive(false);
        transform.localScale = m_OriginalScale;
    }
}
