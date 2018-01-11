using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public class TouchGestureHandler : MonoBehaviour
{
    public enum CircleGestureDirection
    {
        Clockwise,
        AntiClockwise
    };

    [SerializeField] GameEventNames m_GameEventNames = null;
    // For a gesture to be considered a pinch, the direction of the 2 swipes must be between these 2 angles.
    [SerializeField] private float m_MinPinchAngle = 150.0f;
    private const float m_MaxPinchAngle = 180.0f;

    private const float m_MinScrollAngle = 0.0f;
    [SerializeField] private float m_MaxScrollAngle = 40.0f;

    private IEnumerator m_DetectCircleGestureCoroutine;
    private bool m_DetectCircleGestureCoroutineStarted = false;

    // Double Tap
    private bool m_PreviousFrameHasTouch = false;
    private float m_MaxTimeBetweenTaps = 0.2f;
    private float m_TimeBetweenTapsCounter = 0.0f;
    private Vector2 m_FirstTapPosition = new Vector2();
    private Vector2 m_SecondTapPosition = new Vector2();

    [SerializeField] private GUIText m_DebugTextOutput = null;

    public float MaxPinchAngle
    {
        get { return m_MaxPinchAngle; }
    }

    public float MinPinchAngle
    {
        get { return m_MinPinchAngle; }
        set { m_MinPinchAngle = Mathf.Clamp(value, 90.0f, m_MaxPinchAngle); }
    }

    public float MinScrollAngle
    {
        get { return m_MinScrollAngle; }
    }

    public float MaxScrollAngle
    {
        get { return m_MaxScrollAngle; }
        set { m_MaxScrollAngle = Mathf.Clamp(value, m_MinScrollAngle, 90.0f); }
    }

    private void DebugOutput(string _message)
    {
        if (m_DebugTextOutput != null)
        {
            m_DebugTextOutput.text = "Debug: " + _message;
        }
    }

    private bool IsRoughlyEqual(float _a, float _b, float _epsilon)
    {
        return Mathf.Abs(_a - _b) <= _epsilon;
    }

    private bool DetectPinch()
    {
        // Ensure that there are only 2 touches on the screen.
        if (Input.touchCount != 2)
        {
            return false;
        }

        Touch touchA = Input.GetTouch(0);
        Touch touchB = Input.GetTouch(1);

        // Ensure that the 2 swipes are not going the same direction.
        if (Vector2.Dot(touchA.deltaPosition, touchB.deltaPosition) > 0.0f)
        {
            return false;
        }

        // Ensure the the angle between the 2 swipes are within acceptable limits.
        float angle = Vector2.Angle(touchA.deltaPosition, touchB.deltaPosition);
        if (angle < m_MinPinchAngle || angle > m_MaxPinchAngle)
        {
            return false;
        }

        // Find out if the swipes are getting closer or further apart.
        Vector2 previousPositionA = touchA.position - touchA.deltaPosition;
        Vector2 previousPositionB = touchB.position - touchB.deltaPosition;
        float previousDistance = (previousPositionA - previousPositionB).magnitude;
        float currentDistance = (touchA.position - touchB.position).magnitude;

        float magnitude = (previousDistance - currentDistance) / (float)Screen.height;

        GameEventSystem.GetInstance().TriggerEvent<float>(m_GameEventNames.GetEventName(GameEventNames.TouchGestureNames.Pinch), magnitude);

        return true;
    }

    private bool DetectScroll()
    {
        // Ensure that there are only 2 touches on the screen.
        if (Input.touchCount != 2)
        {
            return false;
        }

        Touch touchA = Input.GetTouch(0);
        Touch touchB = Input.GetTouch(1);

        // Ensure that the 2 swipes are going the same direction.
        if (Vector2.Dot(touchA.deltaPosition, touchB.deltaPosition) < 0.0f)
        {
            return false;
        }

        // Ensure the the angle between the 2 swipes are within acceptable limits.
        float angle = Vector2.Angle(touchA.deltaPosition, touchB.deltaPosition);
        if (angle < m_MinScrollAngle || angle > m_MaxScrollAngle)
        {
            return false;
        }

        Vector2 averageVector = (touchA.deltaPosition + touchB.deltaPosition) * 0.5f;
        Vector2 result = averageVector / (float)Screen.height;

        GameEventSystem.GetInstance().TriggerEvent<Vector2>(m_GameEventNames.GetEventName(GameEventNames.TouchGestureNames.Scroll), result);

        return true;
    }

    private bool DetectSwipe()
    {
        // Ensure that there is only 1 touch on the screen.
        if (Input.touchCount != 1)
        {
            return false;
        }

        Touch touchA = Input.GetTouch(0);

        GameEventSystem.GetInstance().TriggerEvent<Vector2>(m_GameEventNames.GetEventName(GameEventNames.TouchGestureNames.Swipe), touchA.deltaPosition / (float)Screen.height);

        return true;
    }

    private void DetectCircleGesture()
    {
        if (m_DetectCircleGestureCoroutineStarted || Input.touchCount != 1)
        {
            return;
        }

        int touchIndex = 0;
        Touch touch = Input.GetTouch(touchIndex);
        if (touch.phase == TouchPhase.Began)
        {
            m_DetectCircleGestureCoroutineStarted = true;
            m_DetectCircleGestureCoroutine = DetectCircleGestureCoroutine(touchIndex);
            DebugOutput("Circle Gesture Starting");
            StartCoroutine(m_DetectCircleGestureCoroutine);
        }
    }

    private IEnumerator DetectCircleGestureCoroutine(int _touchIndex)
    {
        List<Vector2> circlePositions = new List<Vector2>();

        float maxTime = 1.0f;
        float timeTaken = 0.0f;
        while (true)
        {
            timeTaken += Time.unscaledDeltaTime;
            if (timeTaken > maxTime)
            {
                DebugOutput("Time Taken too long.");
                m_DetectCircleGestureCoroutineStarted = false;
                yield break;
            }

            Touch touch = Input.GetTouch(_touchIndex);
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                break;
            }
            circlePositions.Add(touch.position);

            yield return null;
        }


        if (circlePositions.Count < 5)
        {
            DebugOutput("Circle Positions Count < 5");
            m_DetectCircleGestureCoroutineStarted = false;
            yield break;
        }

        // Get the average centre of the circle.
        Vector2 circleCentre = new Vector2(0.0f, 0.0f);
        for (int i = 0; i < circlePositions.Count; ++i)
        {
            DebugOutput("Finding Circle Centre: " + i);
            circleCentre += circlePositions[i];
        }
        circleCentre.x /= (float)circlePositions.Count;
        circleCentre.y /= (float)circlePositions.Count;
        
        // Check that the angle between the points are roughly a circle.
        float totalAngle = 0.0f;
        for (int i = 0; i < circlePositions.Count; ++i)
        {
            DebugOutput("In the other loop: " + i);

            Vector2 centreToPosition = circlePositions[i] - circleCentre;
            Vector2 centreToNextPosition = circlePositions[(i + 1) % circlePositions.Count] - circleCentre;

            totalAngle += Vector2.Angle(centreToPosition, centreToNextPosition);
        }

        if (totalAngle < 270.0f)
        {
            DebugOutput("Total Points Angle Incorrect: " + totalAngle);
            m_DetectCircleGestureCoroutineStarted = false;
            yield break;
        }

        int circleDirection = 0;
        int numAntiClockwise = 0;
        int numClockwise = 0;
        float threshold = (float)Screen.height * 0.15f;
        for (int i = 0; i < circlePositions.Count - 1; ++i)
        {
            DebugOutput("In the other loop 2: " + i);

            Vector2 centreToPosition = circlePositions[i] - circleCentre;
            // To get a vector perpendicular to another vector, just flip the x and y value and negate one of them.
            Vector2 centreToPositionPerpendicular = new Vector2(centreToPosition.y, -centreToPosition.x);
            Vector2 centreToNextPosition = circlePositions[(i + 1) % circlePositions.Count] - circleCentre;

            // Check if the next position is to the left of to the right.
            int currentDirection = (Vector2.Dot(centreToPositionPerpendicular, centreToNextPosition) >= 0.0f) ? 1 : -1;
            if (currentDirection == 1)
            {
                ++numClockwise;
            }
            else
            {
                ++numAntiClockwise;
            }

            // The gesture was not a circle if the points are not going in the same direction.
            if (circleDirection == 0)
            {
                circleDirection = currentDirection;
            }

            // Check that the position is not too close to the centre (To check that the gesture isn't just drawing a slightly curved line.)
            if (centreToPosition.sqrMagnitude < threshold * threshold)
            {
                DebugOutput("Not circlely enough.");
                m_DetectCircleGestureCoroutineStarted = false;
                yield break;
            }
        }

        if (Mathf.Min(numClockwise, numAntiClockwise) > 2)
        {
            DebugOutput("Circle Direction Not Consistent: C[" + numClockwise + "] AC[" + numAntiClockwise + "]");
            m_DetectCircleGestureCoroutineStarted = false;
            yield break;
        }

        Assert.IsTrue(circleDirection == -1 || circleDirection == 1);
        if (circleDirection == -1)
        {
            GameEventSystem.GetInstance().TriggerEvent<CircleGestureDirection>(m_GameEventNames.GetEventName(GameEventNames.TouchGestureNames.CircleGesture), CircleGestureDirection.AntiClockwise);
        }
        else
        {
            GameEventSystem.GetInstance().TriggerEvent<CircleGestureDirection>(m_GameEventNames.GetEventName(GameEventNames.TouchGestureNames.CircleGesture), CircleGestureDirection.Clockwise);
        }

        DebugOutput("Circle Gesture Event Ended");
        m_DetectCircleGestureCoroutineStarted = false;
    }

    private void DetectDoubleTap()
    {
        // Ensure that there is only 1 touch on the screen.
        if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(0))
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position / (float)Screen.height;

            if (!m_PreviousFrameHasTouch)
            {
                // It is only the first tap.
                if (m_TimeBetweenTapsCounter <= 0.0f)
                {
                    m_FirstTapPosition = touchPosition;
                    m_TimeBetweenTapsCounter = m_MaxTimeBetweenTaps;
                }
                else
                {
                    // It is the second tap.
                    m_SecondTapPosition = touchPosition;
                    GameEventSystem.GetInstance().TriggerEvent<Vector2, Vector2>(m_GameEventNames.GetEventName(GameEventNames.TouchGestureNames.DoubleTap), m_FirstTapPosition, m_SecondTapPosition);
                    m_TimeBetweenTapsCounter = 0.0f;
                }
            }
        }

        m_PreviousFrameHasTouch = (Input.touchCount > 0);
        m_TimeBetweenTapsCounter = Mathf.Max(0.0f, m_TimeBetweenTapsCounter - Time.unscaledDeltaTime);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    // Update is called once per frame
    private void FixedUpdate ()
    {
        DetectPinch();
        DetectScroll();
        DetectSwipe();
        DetectCircleGesture();
        DetectDoubleTap();
	}

#if UNITY_EDITOR
    private void OnValidate()
    {
        MinPinchAngle = m_MinPinchAngle;
        MaxScrollAngle = m_MaxScrollAngle;
    }
#endif // UNITY_EDITOR

}