using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

/*
 * NOTE ABOUT COMPLETION CALLBACK!
 * 
 * An MOAnimation's CompletionCallback is to signal that the ANIMATION has finished.
 * Therefore we pass MOAnimation's CompletionCallback into the Animator.
 * This is because an MOAnimation is pretty much tied to a MOAnimator.
 * 
 * An IUnitAction's CompletionCallback is to signal that the ACTION has finished.
 * Therefore we DO NOT pass IUnitAction's CompletionCallback to the MOAnimation!
 * Just because an ANIMATION has finished, it does not mean that the IUnitAction has finished!
 * What IUnitAction passes to MOAnimation is its OnAnimationCompleted() function!
*/
public abstract class MOAnimator : MonoBehaviour
{
    protected GameSystemsDirectory m_GameSystemsDirectory = null;
    protected TileSystem m_TileSystem = null;
    protected CineMachineHandler m_CineMachineHandler = null;

    // Death Animation Variable(s)

    // Non-Serialized Fields
    protected bool m_DeathAnimationPaused = false;

    // Death Animation Callbacks
    protected Void_Void m_DeathAnimationCompletionCallback = null;

    // Death Animation Coroutine(s)
    protected IEnumerator m_DeathAnimationCoroutine = null;

    // Movement Animation Variable(s)

    // Serialized Fields
    [SerializeField] protected float m_MovementSpeed = 10.0f;
    [SerializeField] protected float m_RotationSpeed = 360.0f;

    // Non-Serialized Fields
    protected bool m_MoveAnimationPaused = false;
    /* The GameObject is considered having reached a target position if the distance between the position of this GameObject,
    and the target position is less than m_DistanceTolerance. */
    protected float m_DistanceTolerance = 0.1f;
    /* The GameObject is considered facing a target position if the angle between the forward vector of this GameObject,
    and the director vector to the target position is less than m_RotationTolerance. */
    protected float m_RotationTolerance = 0.3f;
    protected List<TileId> m_MovementPath = new List<TileId>();

    // Movement Animation Callback(s)
    protected Void_Int m_MoveAnimationReachedTileCallback = null;
    protected Void_Void m_MoveAnimationCompletionCallback = null;

    // Movement Animation Coroutine(s)
    protected IEnumerator m_MoveAnimationCoroutine = null;

    // Shoot Animation Variable(s)

    // Non-Serialized Fields
    protected bool m_ShootAnimationPaused = false;

    // Shoot Animation Callback(s)
    protected Void_Void m_ShootAnimationCompletionCallback = null;

    // Shoot Animation Coroutine(s)
    protected IEnumerator m_ShootAnimationCoroutine = null;

    public CineMachineHandler GetCineMachineHandler() { return m_CineMachineHandler; }

    protected void InvokeCallback(Void_Void _callback)
    {
        if (_callback == null) { return; }
        _callback();
    }

    protected void InvokeCallback(Void_Int _callback, int _parameter)
    {
        if (_callback == null) { return; }
        _callback(_parameter);
    }

    /// <summary>
    /// Stopped the cinematic camera if there is any!
    /// </summary>
    public void StopCinematicCamera()
    {
        if (m_CineMachineHandler) { m_CineMachineHandler.SetCineBrain(false); }
    }

    protected virtual void OnDestroy() { StopAllCoroutines(); }

    protected virtual void Awake()
    {
        // Get the Game Systems Directory.
        m_GameSystemsDirectory = GameSystemsDirectory.GetSceneInstance();

        // Get the Systems required fromGame Systems Directory.
        m_TileSystem = m_GameSystemsDirectory.GetTileSystem();
        m_CineMachineHandler = m_GameSystemsDirectory.GetCineMachineHandler();
    }

    // Helper Function(s)
    /// <summary>
    /// Convert an angle to be between -180 and 180.
    /// </summary>
    /// <param name="_angle"></param>
    /// <returns></returns>
    protected float WrapAngle(float _angle)
    {
        while (_angle > 180.0f) { _angle -= 360.0f; }
        while (_angle < -180.0f) { _angle += 360.0f; }
        return _angle;
    }

    // Function(s)
    /// <summary>
    /// Checks if the angle between the direction from _transform to _position and _transform's x rotation
    /// is less than _tolerance. (Calculated in Degrees)
    /// This function only checks on the X Axis.
    /// </summary>
    /// <param name="_position"></param>
    /// <returns></returns>
    protected bool IsFacingVerticalPosition(Transform _transform, Vector3 _position, float _tolerance)
    {
        return IsFacingVerticalPosition(_transform, _position, _tolerance, -180.0f, 180.0f);
    }

    protected bool IsFacingVerticalPosition(Transform _transform, Vector3 _position, float _tolerance, float _minAngle, float _maxAngle)
    {
        Vector3 directionToTarget = _position - _transform.position;
        directionToTarget.Normalize();

        float angleToTarget = -Mathf.Asin(directionToTarget.y) * Mathf.Rad2Deg; // Negate this because a position rotation is downwards.
        float currentAngleX = WrapAngle(_transform.localRotation.eulerAngles.x);
        float angleDifference = angleToTarget - currentAngleX;

        if (Mathf.Abs(angleDifference) <= _tolerance)
        {
            return true;
        }

        if (angleToTarget < _minAngle)
        {
            return (Mathf.Abs(_minAngle - currentAngleX) <= _tolerance);
        }

        if (angleToTarget > _maxAngle)
        {
            return (Mathf.Abs(_maxAngle - currentAngleX) <= _tolerance);
        }

        return false;
    }

    /// <summary>
    /// Checks if the angle between the direction from _transform to _position and _transform's forward vector
    /// is less than _tolerance. (Calculated in Degrees)
    /// This function only checks on the XZ Plane. Height difference is ignored.
    /// </summary>
    /// <param name="_position"></param>
    /// <returns></returns>
    protected virtual bool IsFacingHorizontalPosition(Transform _transform, Vector3 _position, float _tolerance)
    {
        Vector3 directionToDestination = _position - _transform.position;
        directionToDestination.y = 0.0f;
        directionToDestination.Normalize();
        Vector3 currentDirection = _transform.forward;
        currentDirection.y = 0.0f;
        currentDirection.Normalize();

        // Check if the turret is facing the target.
        if (Vector3.Dot(directionToDestination, currentDirection) <= 0.0f) { return false; }
        if (Vector3.Angle(directionToDestination, currentDirection) > _tolerance) { return false; }

        return true;
    }

    protected bool IsTargetOnRight(Transform _transform, Vector3 _targetPosition)
    {
        // Treat the current position and _position as on the same plane.
        Vector3 currentPosition = _transform.position;
        currentPosition.y = 0.0f;
        _targetPosition.y = 0.0f;

        Vector3 directionToTarget = (_targetPosition - currentPosition).normalized;

        Vector3 currentDirection = _transform.forward;
        currentDirection.y = 0.0f;
        currentDirection.Normalize();

        // To find the left direction, reverse currentDirection's x and z, and negate leftDirection's x.
        // To find the right direction, reverse currentDirection's x and z, and negate leftDirection's z.
        Vector3 rightDirection = new Vector3(currentDirection.z, 0.0f, -currentDirection.x);
        return Vector3.Dot(rightDirection, directionToTarget) > 0.0f;
    }

    protected bool IsTargetOnLeft(Transform _transform, Vector3 _targetPosition)
    {
        return !IsTargetOnRight(_transform, _targetPosition);
    }

    /// <summary>
    /// Rotates this GameObject so that it faces _position.
    /// This function only checks on the XZ Plane. Height difference is ignored.
    /// This function assumes that the forward of the GameObject is not pointing up or down.
    /// </summary>
    /// <param name="_position"></param>
    /// <returns>IsFacingPosition(_position)</returns>
    protected bool FaceTowardsPositionHorizontally(Transform _transform, Vector3 _position, float _tolerance)
    {
        if (IsFacingHorizontalPosition(_transform, _position, _tolerance)) { return true; }
        
        // Treat the current position and _position as on the same plane.
        Vector3 currentPosition = _transform.position;
        currentPosition.y = 0.0f;
        _position.y = 0.0f;

        Vector3 directionToTarget = (_position - currentPosition).normalized;

        Vector3 currentDirection = _transform.forward;
        currentDirection.y = 0.0f;
        currentDirection.Normalize();

        // To find the left direction, reverse currentDirection's x and z, and negate leftDirection's x.
        // To find the right direction, reverse currentDirection's x and z, and negate leftDirection's z.
        Vector3 rightDirection = new Vector3(currentDirection.z, 0.0f, -currentDirection.x);
        bool isTargetOnRight = Vector3.Dot(rightDirection, directionToTarget) > 0.0f;

        float angleToTarget = Vector3.Angle(currentDirection, directionToTarget);
        float angleToRotate = Mathf.Min(angleToTarget, m_RotationSpeed * Time.deltaTime);
        // If the target is on our left, we need to negate angleToRotate;
        if (!isTargetOnRight) { angleToRotate = -angleToRotate; }

        _transform.Rotate(0.0f, angleToRotate, 0.0f);

        return IsFacingHorizontalPosition(_transform, _position, _tolerance);
    }

    /// <summary>
    /// Checks if this GameObject distance to _position is less than m_DistanceTolerance.
    /// This function only checks on the XZ Plane. Height difference is ignored.
    /// </summary>
    /// <param name="_position"></param>
    /// <returns>GameObject Distance To _position < m_DistanceTolerance.</returns>
    protected bool IsAtHorizontalPosition(Transform _transform, Vector3 _position)
    {
        _position.y = _transform.position.y;
        return (_position - _transform.position).sqrMagnitude < m_DistanceTolerance * m_DistanceTolerance;
    }

    /// <summary>
    /// Translates this GameObject towards _position.
    /// This function only checks on the XZ Plane. Height difference is ignored.
    /// </summary>
    /// <param name="_position"></param>
    /// <returns>IsAtPosition(_position)</returns>
    protected bool MoveTowardsPositionHorizontally(Transform _transform, Vector3 _position)
    {
        if (IsAtHorizontalPosition(_transform, _position)) { return true; }

        // Treat the current position and _position as on the same plane.
        Vector3 currentPosition = _transform.position;
        currentPosition.y = 0.0f;
        _position.y = 0.0f;

        Vector3 directionToDestination = _position - currentPosition;
        float distanceToDestinationSquared = directionToDestination.sqrMagnitude;
        float travelDistance = m_MovementSpeed * Time.deltaTime;

        if (distanceToDestinationSquared < (travelDistance * travelDistance))
        {
            _transform.position = new Vector3(_position.x, _transform.position.y, _position.z);
        }
        else
        {
            _transform.position += (directionToDestination.normalized * travelDistance);
        }

        return IsAtHorizontalPosition(_transform, _position);
    }

    protected void SetMovementPath(TileId[] _movementPath)
    {
        m_MovementPath.Clear();

        Assert.IsNotNull(_movementPath, MethodBase.GetCurrentMethod().Name + " - _movementPath must not be null!");
        Assert.IsTrue(_movementPath.Length > 0, MethodBase.GetCurrentMethod().Name + " - _movementPath must not be an empty array!");

        for (int i = 0; i < _movementPath.Length; ++i)
        {
            Assert.IsNotNull(_movementPath[i], MethodBase.GetCurrentMethod().Name + " - _movementPath must not contain null elements!");
            m_MovementPath.Add(_movementPath[i]);
        }
    }

    protected void ClearMovementPath()
    {
        m_MovementPath.Clear();
    }

    // Death Animation
    protected virtual IEnumerator DeathAnimationCoroutine()
    {
        InvokeCallback(m_DeathAnimationCompletionCallback);
        yield break;
    }

    public virtual void StartDeathAnimation(Void_Void _completionCallback)
    {
        m_DeathAnimationPaused = false;
        m_DeathAnimationCompletionCallback = _completionCallback;
        m_DeathAnimationCoroutine = DeathAnimationCoroutine();
        StartCoroutine(m_DeathAnimationCoroutine);
    }

    public virtual void PauseDeathAnimation()
    {
        m_DeathAnimationPaused = true;
    }

    public virtual void ResumeDeathAnimation()
    {
        m_DeathAnimationPaused = false;
    }

    public virtual void StopDeathAnimation()
    {
        m_DeathAnimationPaused = false;

        StopCoroutine(m_DeathAnimationCoroutine);
        m_DeathAnimationCoroutine = null;
    }

    // Move Animation
    protected virtual bool FaceTowardsTile(int _movementPathIndex)
    {
        Tile tile = m_TileSystem.GetTile(m_MovementPath[_movementPathIndex]);
        Assert.IsNotNull(tile, MethodBase.GetCurrentMethod().Name + " - Tile " + m_MovementPath[_movementPathIndex].GetAsString() + " not found!");
        return FaceTowardsPositionHorizontally(transform, tile.transform.position, m_RotationTolerance);
    }

    protected virtual bool MoveTowardsTile(int _movementPathIndex)
    {
        Tile tile = m_TileSystem.GetTile(m_MovementPath[_movementPathIndex]);
        Assert.IsNotNull(tile, MethodBase.GetCurrentMethod().Name + " - Tile " + m_MovementPath[_movementPathIndex].GetAsString() + " not found!");
        return MoveTowardsPositionHorizontally(transform, tile.transform.position);
    }

    protected virtual bool IsAtTilePosition(int _movementPathIndex)
    {
        Tile tile = m_TileSystem.GetTile(m_MovementPath[_movementPathIndex]);
        Assert.IsNotNull(tile, MethodBase.GetCurrentMethod().Name + " - Tile " + m_MovementPath[_movementPathIndex].GetAsString() + " not found!");
        return IsAtHorizontalPosition(transform, tile.transform.position);
    }

    protected virtual IEnumerator MoveAnimationCouroutine()
    {
        // Move along the path.
        int movementPathIndex = 0;
        while (movementPathIndex < m_MovementPath.Count)
        {
            if (!IsAtTilePosition(movementPathIndex))
            {
                // Face the tile we need to move towards.
                while (true)
                {
                    // Do nothing if it is paused.
                    while (m_MoveAnimationPaused) { yield return null; }

                    if (FaceTowardsTile(movementPathIndex)) { break; }

                    yield return null;
                }

                // Move towards the tile.
                while (true)
                {
                    // Do nothing if it is paused.
                    while (m_MoveAnimationPaused) { yield return null; }

                    if (MoveTowardsTile(movementPathIndex)) { break; }

                    yield return null;
                }
            }

            // We've reached a tile.
            InvokeCallback(m_MoveAnimationReachedTileCallback, movementPathIndex);
            ++movementPathIndex;
        }

        // We've reached the end of the path.
        InvokeCallback(m_MoveAnimationCompletionCallback);
    }

    public virtual void StartMoveAnimation(TileId[] _movementPath, Void_Int _reachedTileCallback, Void_Void _completionCallback)
    {
        m_MoveAnimationPaused = false;

        SetMovementPath(_movementPath);
        m_MoveAnimationReachedTileCallback = _reachedTileCallback;
        m_MoveAnimationCompletionCallback = _completionCallback;

        m_MoveAnimationCoroutine = MoveAnimationCouroutine();
        StartCoroutine(m_MoveAnimationCoroutine);
    }

    public virtual void PauseMoveAnimation()
    {
        m_MoveAnimationPaused = true;
    }
    
    public virtual void ResumeMoveAnimation()
    {
        m_MoveAnimationPaused = false;
    }
    
    public virtual void StopMoveAnimation()
    {
        m_MoveAnimationPaused = false;

        StopCoroutine(m_MoveAnimationCoroutine);
        m_MoveAnimationCoroutine = null;
    }

    // Shoot Animation
    protected virtual IEnumerator ShootAnimationCouroutine() { throw new System.NotImplementedException(); }

    /* Each Animator may require different parameters to start their shoot animation.
       Hence, no base StartShootAnimation function will be implemented. */

    public virtual void PauseShootAnimation()
    {
        m_ShootAnimationPaused = true;
    }

    public virtual void ResumeShootAnimation()
    {
        m_ShootAnimationPaused = false;
    }

    public virtual void StopShootAnimation()
    {
        m_ShootAnimationPaused = false;

        StopCoroutine(m_ShootAnimationCoroutine);
        m_ShootAnimationCoroutine = null;
    }

    // All Action(s)
    public virtual void StopAllAnimations()
    {
        StopDeathAnimation();
        StopMoveAnimation();
        StopShootAnimation();
    }
}