using UnityEngine;
using System.Collections;

public enum WheelSide
{
    Left,
    Right
}

[System.Serializable]
public class AnimatedWheel
{
    public Transform transform;
    public bool front;
    public WheelSide side;
    [HideInInspector]
    public Transform
        parent;
}

public class WheelAnimator : MonoBehaviour
{
    public float speed;
    public float steerAngle;
    public bool allWheelSteering = false;
    public AnimatedWheel[] wheels;
    float actualSpeed = 0;
    float actualAngle = 0;

    void Start ()
    {
        foreach (var w in wheels) {
            var s = new GameObject ("Suspension");
            s.transform.position = w.transform.position;
            s.transform.rotation = w.transform.rotation;
            s.transform.parent = transform;
            w.transform.parent = s.transform;
            w.parent = s.transform;
        }
    
    }

    void Update ()
    {

        foreach (var w in wheels) {
            var wheelAngle = 0f;
            if (w.front) {
                wheelAngle = actualAngle;
            } else {
                if (allWheelSteering)
                    wheelAngle = -actualAngle;
            }
            w.parent.localEulerAngles = new Vector3 (0, wheelAngle, 0);
            w.transform.Rotate (new Vector3 (actualSpeed * Time.deltaTime, 0, 0), Space.Self);

        }

        actualSpeed = Mathf.Lerp (actualSpeed, speed, Time.deltaTime);
        actualAngle = Mathf.Lerp (actualAngle, steerAngle, Time.deltaTime);
    
    }
}
