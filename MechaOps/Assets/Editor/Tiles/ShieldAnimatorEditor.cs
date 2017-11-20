using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MOAnimator_Shield))]
[CanEditMultipleObjects]
public class ShieldAnimatorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MOAnimator_Shield shieldAnimator = (MOAnimator_Shield)target;

        if (GUILayout.Button("Start Turn Off Animation"))
        {
            shieldAnimator.StartTurnOffAnimation();
        }

        if (GUILayout.Button("Start Turn On Animation"))
        {
            shieldAnimator.StartTurnOnAnimation();
        }

        if (GUILayout.Button("Start Shield Break Animation"))
        {
            shieldAnimator.StartShieldBreakAnimation();
        }
    }

}
