using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent, RequireComponent(typeof(GameCameraMovement))]
public class GameCameraController : MonoBehaviour
{
    /*
     * Note: Order of movement control priority
     * 1. Camera Follow Target
     * 2. Camera Focus on Target
     * 3. Player Input
     * 
     * Player Input Zoom, Rotate etc. are not affected by priority and can be done at the same time.
     */

    // Serialised Variable(s)
    [SerializeField] private GameEventNames m_GameEventNames = null;
    [SerializeField] private float m_ZoomSpeed = 50.0f;
    [SerializeField] private float m_MovementSpeed = 10.0f;

    // Non-Serialised Variable(s)
    private GameCameraMovement m_GameCameraMovement = null;
    private bool m_EventsInitialised = false;

    // Following / Focus On Unit
    private float m_MoveToPositionDuration = 0.3f;

    // The GameCameraController will attempt to follow this target.
    private bool m_FollowingTarget = false;
    private UnitStats m_FollowedTarget = null;

    // The GameCameraController will attempt to move the GameCamera to the m_SelectedUnitPosition for this amound of time.
    private float m_FocusOnTargetTimeLeft = 0.0f;
    private Vector3 m_FocusPosition = new Vector3(0.0f, 0.0f, 0.0f);

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

    private bool CanFollowTarget()
    {
        return m_FollowingTarget && (m_FollowedTarget != null);
    }

    private bool CanFocusOnTarget()
    {
        return (!CanFollowTarget()) && (m_FocusOnTargetTimeLeft > 0.0f);
    }

    private bool PlayerCanMove()
    {
        return (!CanFollowTarget()) && (!CanFocusOnTarget());
    }

    // Events
    private void InitEvents()
    {
        // Check that we are not already initialised.
        if (m_EventsInitialised) { return; }

        // Initialise events
        GameEventSystem.GetInstance().SubscribeToEvent<UnitStats, bool>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.FollowTarget), FollowTargetCallback);
        GameEventSystem.GetInstance().SubscribeToEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.FocusOnTarget), CameraFocusCallback);
        GameEventSystem.GetInstance().SubscribeToEvent<float>(m_GameEventNames.GetEventName(GameEventNames.TouchGestureNames.Pinch), PinchCallback);
        GameEventSystem.GetInstance().SubscribeToEvent<Vector2>(m_GameEventNames.GetEventName(GameEventNames.TouchGestureNames.Scroll), ScrollCallback);
        GameEventSystem.GetInstance().SubscribeToEvent<Vector2>(m_GameEventNames.GetEventName(GameEventNames.TouchGestureNames.Swipe), SwipeCallback);
        //GameEventSystem.GetInstance().SubscribeToEvent<TouchGestureHandler.CircleGestureDirection>(m_GameEventNames.GetEventName(GameEventNames.TouchGestureNames.CircleGesture), CircleGestureCallback);
        GameEventSystem.GetInstance().SubscribeToEvent<Vector2, Vector2>(m_GameEventNames.GetEventName(GameEventNames.TouchGestureNames.DoubleTap), DoubleTapCallback);

        m_EventsInitialised = true;
    }

    private void DeinitEvents()
    {
        // Ensure that we are initialised.
        if (!m_EventsInitialised) { return; }

        // Uninitialise events here.
        GameEventSystem.GetInstance().UnsubscribeFromEvent<UnitStats, bool>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.FollowTarget), FollowTargetCallback);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<UnitStats>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.FocusOnTarget), CameraFocusCallback);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<float>(m_GameEventNames.GetEventName(GameEventNames.TouchGestureNames.Pinch), PinchCallback);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<Vector2>(m_GameEventNames.GetEventName(GameEventNames.TouchGestureNames.Scroll), ScrollCallback);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<Vector2>(m_GameEventNames.GetEventName(GameEventNames.TouchGestureNames.Swipe), SwipeCallback);
        //GameEventSystem.GetInstance().UnsubscribeFromEvent<TouchGestureHandler.CircleGestureDirection>(m_GameEventNames.GetEventName(GameEventNames.TouchGestureNames.CircleGesture), CircleGestureCallback);
        GameEventSystem.GetInstance().UnsubscribeFromEvent<Vector2, Vector2>(m_GameEventNames.GetEventName(GameEventNames.TouchGestureNames.DoubleTap), DoubleTapCallback);

        m_EventsInitialised = false;
    }

    // Helper Function(s)
    Vector3 CalculateCameraOffset()
    {
        // Get the plane that the TileSystem is on.
        Plane horizontalPlane = new Plane(Vector3.up, m_GameCameraMovement.GetTileSystem().transform.position);

        // Raycast from the GameCamera to the TileSystem's plane.
        Ray ray = new Ray(m_GameCameraMovement.transform.position, m_GameCameraMovement.transform.forward);
        float distanceToPlane = 0.0f;

        // If the camera is not looking at the TileSystem's plane, we cannot proceed.
        if (!horizontalPlane.Raycast(ray, out distanceToPlane)) { return new Vector3(0.0f, 0.0f, 0.0f); }

        // Find the point where that camera raycast hits the plane.
        // This means that this is the position which the camera is looking at.
        Vector3 cameraLookingPosition = ray.GetPoint(distanceToPlane);

        // When we move the camera, we want it to end up looking at the selected unit's tile.
        Vector3 offset = m_GameCameraMovement.transform.position - cameraLookingPosition;

        return offset;
    }

    // Callbacks
    private void FollowTargetCallback(UnitStats _unit, bool _follow)
    {
        m_FollowingTarget = _follow && (_unit != null);
        m_FollowedTarget = m_FollowingTarget ? _unit : null;
    }

    private void CameraFocusCallback(UnitStats _unit)
    {
        // Get the tile position of the unit.
        m_FocusPosition = m_GameCameraMovement.GetTileSystem().GetTile(_unit.CurrentTileID).transform.position;
        m_FocusOnTargetTimeLeft = m_MoveToPositionDuration;
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
        if (!PlayerCanMove()) { return; }

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

    private void DoubleTapCallback(Vector2 _firstTap, Vector2 _secondTap)
    {
        // Check if both taps are on the left
        if (_firstTap.x < 0.5f && _secondTap.x < 0.5f)
        {
            m_GameCameraMovement.RotateLeft();
        }

        // Check if both taps are on the right.
        if (_firstTap.x > 0.5f && _secondTap.x > 0.5f)
        {
            m_GameCameraMovement.RotateRight();
        }
    }

    // Updates
    private void MoveToTargetPosition()
    {
        if (!CanFollowTarget()) { return; }

        m_GameCameraMovement.LerpToPosition(m_FollowedTarget.transform.position + CalculateCameraOffset(), m_MoveToPositionDuration);
    }

    private void MoveToFocusPosition()
    {
        if (!CanFocusOnTarget()) { return; }

        m_GameCameraMovement.LerpToPosition(m_FocusPosition + CalculateCameraOffset(), m_MoveToPositionDuration);
        m_FocusOnTargetTimeLeft = Mathf.Max(0.0f, m_FocusOnTargetTimeLeft - Time.deltaTime);
    }

    // Very basic Handling of Keyboard Input for development purposes.
    private void HandleKeyboardInput()
    {
        // We need a multiplier because the zoom speed on PC is too high.
        float zoomMultiplier = 0.01f;
        m_GameCameraMovement.Zoom(Input.GetAxis("Zoom") * zoomMultiplier * m_ZoomSpeed * Time.deltaTime);

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

        if (!PlayerCanMove()) { return; }
        m_GameCameraMovement.MoveRight(Input.GetAxis("Horizontal") * m_MovementSpeed * Time.deltaTime);
        m_GameCameraMovement.MoveForward(Input.GetAxis("Vertical") * m_MovementSpeed * Time.deltaTime);
    }

    private void Awake()
    {
        m_GameCameraMovement = gameObject.GetComponent<GameCameraMovement>();
        Assert.IsNotNull(m_GameCameraMovement);
        Assert.IsNotNull(m_GameEventNames);

        InitEvents();
    }

    private void OnDestroy()
    {
        DeinitEvents();
    }

    private void Update()
    {
        MoveToTargetPosition();
        MoveToFocusPosition();
        HandleKeyboardInput();
    }
}