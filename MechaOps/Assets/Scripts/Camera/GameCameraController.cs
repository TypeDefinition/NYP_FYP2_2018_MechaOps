using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent, RequireComponent(typeof(GameCameraMovement))]
public class GameCameraController : MonoBehaviour
{
    private GameCameraMovement m_GameCameraMovement = null;
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
        if (m_EventsInitialised) { return; }

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
        if (!m_EventsInitialised) { return; }

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
        switch (_circleDirection)
        {
            case TouchGestureHandler.CircleGestureDirection.AntiClockwise:
                m_GameCameraMovement.RotateLeft();
                break;
            case TouchGestureHandler.CircleGestureDirection.Clockwise:
                m_GameCameraMovement.RotateRight();
                break;
            default:
                // Do nothing.
                break;
        }
    }

    // Very basic Handling of Keyboard Input for development purposes.
    private void HandleKeyboardInput()
    {
        // We need a multiplier because the zoom speed on PC is too high.
        float zoomMultiplier = 0.01f;
        m_GameCameraMovement.Zoom(Input.GetAxis("Zoom") * zoomMultiplier * m_ZoomSpeed * Time.deltaTime);
        m_GameCameraMovement.MoveRight(Input.GetAxis("Horizontal") * m_MovementSpeed * Time.deltaTime);
        m_GameCameraMovement.MoveForward(Input.GetAxis("Vertical") * m_MovementSpeed * Time.deltaTime);

        if (Input.GetButtonDown("RotateLeft"))
        {
            m_GameCameraMovement.RotateLeft();
        }
        if (Input.GetButtonDown("RotateRight"))
        {
            m_GameCameraMovement.RotateRight();
        }
        if (Input.GetButton("MoveDown"))
        {
            m_GameCameraMovement.MoveDown(m_MovementSpeed * Time.deltaTime);
        }
        if (Input.GetButton("MoveUp"))
        {
            m_GameCameraMovement.MoveUp(m_MovementSpeed * Time.deltaTime);
        }
    }

    private void Awake()
    {
        m_GameCameraMovement = gameObject.GetComponent<GameCameraMovement>();
    }

    private void Update()
    {
        HandleKeyboardInput();
    }

    private void OnDestroy()
    {
        DeinitEvents();
    }

    private void OnEnable()
    {
        InitEvents();
    }

    private void OnDisable()
    {
        DeinitEvents();
    }

}