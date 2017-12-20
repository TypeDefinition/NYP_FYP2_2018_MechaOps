using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOAnimator : MonoBehaviour
{
    [SerializeField] protected CineMachineHandler m_CineMachineHandler = null;

    // The GameObject is considered having reached a target position if the distance between the position of this GameObject,
    // and the target position is less than m_DistanceTolerance.
    protected float m_DistanceTolerance = 0.1f;
    // The GameObject is considered facing a target position if the angle between the forward vector of this GameObject,
    // and the director vector to the target position is less than m_RotationTolerance.
    protected float m_RotationTolerance = 0.5f;
    

    public CineMachineHandler GetCineMachineHandler() { return m_CineMachineHandler; }

    /// <summary>
    /// Stopped the cinematic camera if there is any!
    /// </summary>
    public void StopCinematicCamera()
    {
        if (m_CineMachineHandler)
        {
            m_CineMachineHandler.SetCineBrain(false);
        }
    }
    protected virtual void OnDestroy()
    {
        StopAllCoroutines();
    }

    protected virtual void Awake()
    {
        switch (tag)
        {
            case "Player":
                // TODO: Only the player's units get to do the cinematic shots for now
                if (!m_CineMachineHandler)
                {
                    m_CineMachineHandler = FindObjectOfType<CineMachineHandler>();
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// // Convert an angle to be between -180 and 180.
    /// </summary>
    /// <param name="_angle"></param>
    /// <returns></returns>
    protected float ConvertAngle(float _angle)
    {
        while (_angle > 180.0f)
        {
            _angle -= 360.0f;
        }
        while (_angle < -180.0f)
        {
            _angle += 360.0f;
        }

        return _angle;
    }

    protected virtual bool IsFacingTargetPosition(Vector3 _targetPosition)
    {
        Vector3 directionToDestination = _targetPosition - transform.position;
        directionToDestination.y = 0.0f;
        Vector3 forward = transform.forward;
        forward.y = 0.0f;

        // Check if the turret is facing the target.
        if (Vector3.Dot(directionToDestination, forward) <= 0.0f) { return false; }
        if (Vector3.Angle(directionToDestination, forward) > m_RotationTolerance) { return false; }

        return true;
    }

    protected virtual bool IsAtDestination(Vector3 _destination)
    {
        _destination.y = gameObject.transform.position.y;
        return (_destination - gameObject.transform.position).sqrMagnitude < m_DistanceTolerance * m_DistanceTolerance;
    }
}