using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SimpleSpriteAnimation))]
[CanEditMultipleObjects]
public class SimpleSpriteAnimationEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        SimpleSpriteAnimation simpleSpriteAnimation = (SimpleSpriteAnimation)target;
        
        if (GUILayout.Button("Start Animation"))
        {
            simpleSpriteAnimation.StartAnimation();
        }

        if (GUILayout.Button("Pause Animation"))
        {
            simpleSpriteAnimation.PauseAnimation();
        }

        if (GUILayout.Button("Resume Animation"))
        {
            simpleSpriteAnimation.ResumeAnimation();
        }

        if (GUILayout.Button("Stop Animation"))
        {
            simpleSpriteAnimation.StopAnimation();
        }
    }
}