using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class GameCameraController : MonoBehaviour
{
    [SerializeField] private GameCameraMovement m_GameCameraMovement = null;
    private bool m_EventsInitialised = false;

    [SerializeField] private float m_ZoomSpeed = 50.0f;
    [SerializeField] private float m_MovementSpeed = 10.0f;

    public GameCameraMovement GetGameCameraMovement()
    {
        return m_GameCameraMovement;
    }

    public float ZoomSpeed
    {
        get { return m_ZoomSpeed; }
        set { m_ZoomSpeed = Mathf.Max(0.0f, value); }
    }

    public float MovementSpeed
    {
        get { return m_MovementSpeed; }
        set { m_MovementSpeed = Mathf.Max(0.0f, value); }
    }

    private void InitEvents()
    {
        // Check that we are not already initialised.
        if (m_EventsInitialised == true)
        {
            return;
        }

        // Initialise events
        GameEventSystem.GetInstance().SubscribeToEvent<float>("Pinch Event", PinchCallback);
        GameEventSystem.GetInstance().SubscribeToEvent<Vector2>("Scroll Event", ScrollCallback);
        GameEventSystem.GetInstance().SubscribeToEvent<Vector2>("Swipe Event", SwipeCallback);
        GameEventSystem.GetInstance().SubscribeToEvent<TouchGestureHandler.CircleGestureDirection>("Circle Gesture Event", CircleGestureCallback);

        m_EventsInitialised = true;
    }

    private void DeinitEvents()
    {
        // Ensure that we are initialised.
        if (m_EventsInitialised == false)
        {
            return;
        }

        // Uninitialise events here.
        GameEventSystem.GetInstance().UnsubscribeFromEvent<float>("Pinch Event", PinchCallback);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<Vector2>("Scroll Event", ScrollCallback);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<Vector2>("Swipe Event", SwipeCallback);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<TouchGestureHandler.CircleGestureDirection>("Circle Gesture Event", CircleGestureCallback);

        m_EventsInitialised = false;
    }

    // I did not multiply by Time.deltaTime since the magnitude is already only the magnitude for this frame.
    private void PinchCallback(float _magnitude)
    {
        Assert.IsTrue(m_GameCameraMovement != null);
        m_GameCameraMovement.Zoom(_magnitude * m_ZoomSpeed);
    }

    // I did not multiply by Time.deltaTime since the magnitude is already only the magnitude for this frame.
    private void ScrollCallback(Vector2 _scroll)
    {
        m_GameCameraMovement.MoveDown(_scroll.y * m_MovementSpeed);
    }

    // I did not multiply by Time.deltaTime since the magnitude is already only the magnitude for this frame.
    private void SwipeCallback(Vector2 _swipe)
    {
        m_GameCameraMovement.MoveBackwards(_swipe.y * m_MovementSpeed);
        m_GameCameraMovement.MoveLeft(_swipe.x * m_MovementSpeed);
    }

    private void CircleGestureCallback(TouchGestureHandler.CircleGestureDirection _circleDirection)
    {
        if (_circleDirection == TouchGestureHandler.CircleGestureDirection.AntiClockwise)
        {
            m_GameCameraMovement.RotateLeft();
        }

        if (_circleDirection == TouchGestureHandler.CircleGestureDirection.Clockwise)
        {
            m_GameCameraMovement.RotateRight();
        }
    }

    private void Awake()
    {
        InitEvents();
    }

    private void OnDestroy()
    {
        DeinitEvents();
    }

}