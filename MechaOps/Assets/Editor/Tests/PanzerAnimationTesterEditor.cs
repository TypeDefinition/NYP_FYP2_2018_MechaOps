using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PanzerAnimationTester))]
[CanEditMultipleObjects]
public class PanzerAnimationTesterEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PanzerAnimationTester tester = (PanzerAnimationTester)target;

        if (GUILayout.Button("Start Attack Animation"))
        {
            tester.StartAttackAnimation();
        }

        if (GUILayout.Button("Stop Attack Animation"))
        {
            tester.StopAttackAnimation();
        }

        if (GUILayout.Button("Start Move Animation"))
        {
            tester.StartMoveAnimation();
        }
        
        if (GUILayout.Button("Stop Moving Animation"))
        {
            tester.StopMoveAnimation();
        }

        if (GUILayout.Button("Start Destroy Animation"))
        {
            tester.StartDestroyAnimation();
        }

        if (GUILayout.Button("Stop Destroy Animation"))
        {
            tester.StopDestroyAnimation();
        }
    }

}
