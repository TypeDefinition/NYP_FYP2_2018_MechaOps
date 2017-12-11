using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SciFiArtillery : MonoBehaviour
{
        
    public Transform pivot;
    public Transform[] pitch;
    public bool enablePitch = true;
    public float angle = 0;
    public float pitchSpeed = 10;
    public float dampSpeed = 0.1f;
    float pitchVelocity;

    bool AngleInRange (float A, float D)
    {
        if (A < (360 - D) && A > 180) {
            return false;
        }
        if (A > D && A < 180) {
            return false;
        }
        return true;
    }

    bool AngleBetween (float angle, float A, float B)
    {
        angle = (360 + angle % 360) % 360;
        A = (360 + A % 360) % 360;
        B = (360 + B % 360) % 360;
        if (A < B)
            return A <= angle && angle <= B;
        return A <= angle || angle <= B;
    }

    void Update ()
    {
        if (pitch == null)
            return;
        
                
        if (enablePitch) {
            var PE = pitch [0].localEulerAngles;
            PE.x = Mathf.SmoothDampAngle (PE.x, -angle, ref pitchVelocity, dampSpeed, pitchSpeed);
            foreach (var p in this.pitch) {
                p.localEulerAngles = PE;
            }
        
        }
            

            
    }
    
   


 
}
