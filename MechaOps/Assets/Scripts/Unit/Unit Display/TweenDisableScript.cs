using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// set the gameobject inactive when the credit is insufficient
/// </summary>
public class TweenDisableScript : TweenUI_Scale {
    public override void AnimateUI()
    {
        // it will set the UI to be inactive when AnimateUI
        LeanTween.scaleX(gameObject, 0, m_AnimationTime).setOnComplete(SetGameObjInactive);
    }

    protected void SetGameObjInactive()
    {
        gameObject.SetActive(false);
        transform.localScale = m_OriginalScale;
    }
}
