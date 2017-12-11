using UnityEngine;
using System.Collections;

public class FlightAnimation : MonoBehaviour
{
    
    public float roll;
    public float pitch;
    public float smoothTime = 1;
	
    public float driftFrequency = 1;
    public float driftHeight = 1;

    public Transform[] propellers;
    public float propellerSpeed;

    float rollVelocity, pitchVelocity, actualPropellerSpeed, propVelocity;
    Transform rollTx, pitchTx;
    float yPos;
    Vector3 restPosition;

    void Start ()
    {
        rollTx = transform.GetChild (0);
        pitchTx = rollTx.GetChild (0);
        restPosition = rollTx.localPosition;
    }

    void Update ()
    {

        pitch = Mathf.Clamp (pitch, -89, 89);
        roll = Mathf.Clamp (roll, -89, 89);

        restPosition.y = Mathf.Sin (Time.time * driftFrequency) * driftHeight;
        rollTx.localPosition = restPosition;

        var e = pitchTx.localEulerAngles;
        e.x = Mathf.SmoothDampAngle (e.x, pitch, ref pitchVelocity, smoothTime);
        pitchTx.localEulerAngles = e;

        var r = rollTx.localEulerAngles;
        r.z = Mathf.SmoothDampAngle (r.z, roll, ref rollVelocity, smoothTime);
        rollTx.localEulerAngles = r;

        foreach (var p in propellers) {
            p.Rotate (new Vector3 (0, 0, actualPropellerSpeed * Time.deltaTime), Space.Self);
        }
        actualPropellerSpeed = Mathf.SmoothDamp (actualPropellerSpeed, propellerSpeed, ref propVelocity, smoothTime);

    }
}
