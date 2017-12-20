using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent]
public class SHSMAnimator : MOAnimator
{
    // Gun Elevation
    [SerializeField, Range(0, 90)] protected float m_MaxGunElevation = 80.0f;
    [SerializeField, Range(0, 90)] protected float m_MaxGunDepression = 20.0f;

    // Weapons
    [SerializeField] protected GameObject m_Turret;
    [SerializeField] protected GameObject m_Gun;

    // Hull
    [SerializeField] protected GameObject m_Hull;

    // Tracks & Wheels
    [SerializeField] protected GameObject m_LeftTracks;
    [SerializeField] protected GameObject m_RightTracks;
    [SerializeField] protected GameObject[] m_LeftWheels;
    [SerializeField] protected GameObject[] m_RightWheels;

    // Bullet
    [SerializeField] protected GameObject m_BulletSpawn;
    [SerializeField] protected ArtileryBullet m_Bullet_Prefab;
    [SerializeField] protected GameObject m_MuzzleFlash_Prefab;

    // Moving Animation
    protected Vector3 m_Destination = new Vector3();
    protected float m_MovementSpeed = 10.0f;
    protected float m_TankRotationSpeed = 3.0f;
    protected Void_Void m_MoveAnimationCompleteCallback = null;
    IEnumerator m_MoveAnimationCoroutine = null;

    // Shooting Animation
    protected GameObject m_Target = null;
    protected float m_GunElevationSpeed = 60.0f; // This is the speed in degrees.
    protected ArtileryBullet m_Bullet = null;
    protected Void_Void m_ShootAnimationCompleteCallback = null;
    IEnumerator m_ShootAnimationCoroutine;

    public virtual void AnimateWheels(float _leftSpeed, float _rightSpeed)
    {
        AnimateWheelsLeft(_leftSpeed);
        AnimateWheelsRight(_rightSpeed);
    }

    public virtual void AnimateWheelsLeft(float _speed)
    {
        foreach (GameObject wheel in m_LeftWheels)
        {
            wheel.transform.Rotate(new Vector3(_speed, 0.0f, 0.0f));
        }
    }

    public virtual void AnimateWheelsRight(float _speed)
    {
        foreach (GameObject wheel in m_RightWheels)
        {
            wheel.transform.Rotate(new Vector3(_speed, 0.0f, 0.0f));
        }
    }

    public virtual void AnimateMuzzleFlash()
    {
        GameObject muzzleFlash = GameObject.Instantiate(m_MuzzleFlash_Prefab);
        muzzleFlash.transform.position = m_BulletSpawn.transform.position;
        muzzleFlash.transform.rotation = m_BulletSpawn.transform.rotation;
    }

    public virtual void AnimateElevateGun(float _angle)
    {
        m_Gun.transform.Rotate(new Vector3(_angle, 0.0f, 0.0f));

        // Limit our angle. The possible angle we can get ranges from -360 Degrees to 360 Degrees.
        // We need to convert it to the  from -180 Degress to 180 Degress range.
        float currentAngle = ConvertAngle(m_Gun.transform.localRotation.eulerAngles.x);
        currentAngle = Mathf.Clamp(currentAngle, -m_MaxGunElevation, m_MaxGunDepression);
        Quaternion desiredRotation = new Quaternion();
        desiredRotation.eulerAngles = new Vector3(currentAngle, 0.0f, 0.0f);
        m_Gun.transform.localRotation = desiredRotation;
    }

    protected virtual void RotateTowardsTargetPosition(Vector3 _targetPosition)
    {
        Vector3 directionToTarget = _targetPosition - transform.position;
        directionToTarget.y = 0.0f;

        Quaternion currentRotation = gameObject.transform.rotation;
        Quaternion desiredRotation = Quaternion.LookRotation(directionToTarget);

        gameObject.transform.rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * m_TankRotationSpeed);

        // If the direction is towards the right.
        if (Vector3.Dot(transform.right, directionToTarget) > 0.0f)
        {
            AnimateWheels(m_MovementSpeed * Time.deltaTime, -m_MovementSpeed * Time.deltaTime);
        }
        // If the direction is towards the left.
        else
        {
            AnimateWheels(-m_MovementSpeed * Time.deltaTime, m_MovementSpeed * Time.deltaTime);
        }
    }

    protected virtual void TranslateTowardsTargetPosition(Vector3 _targetPosition)
    {
        Vector3 directionToTarget = _targetPosition - transform.position;
        directionToTarget.y = 0.0f;
        Vector3 forward = transform.forward;
        forward.y = 0.0f;
        Vector3 snapPosition = _targetPosition;
        snapPosition.y = gameObject.transform.position.y;
        // 与目标距离自乘
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget < m_DistanceTolerance)
        {
            gameObject.transform.position = snapPosition;
        }
        else if (Vector3.Dot(directionToTarget, forward) < 0.0f)
        {
            transform.position = snapPosition;
        }
        else
        {
            directionToTarget.Normalize();
            gameObject.transform.position += (directionToTarget * Mathf.Min(Time.deltaTime * m_MovementSpeed, distanceToTarget));
        }

        AnimateWheels(m_MovementSpeed * Time.deltaTime, m_MovementSpeed * Time.deltaTime);
    }

    // Coroutines
    protected virtual IEnumerator MoveAnimationCouroutine()
    {
        while (true)
        {
            if (IsAtDestination(m_Destination))
            {
                if (m_MoveAnimationCompleteCallback != null)
                {
                    m_MoveAnimationCompleteCallback();
                }
                break;
            }

            if (!IsFacingTargetPosition(m_Destination))
            {
                RotateTowardsTargetPosition(m_Destination);
                yield return null;
                continue;
            }

            TranslateTowardsTargetPosition(m_Destination);
            yield return null;
        }
    }

    protected virtual IEnumerator ShootAnimationCouroutine() { yield break; }

    // Tailored Animations

    // Move
    public virtual void StartMoveAnimation()
    {
        m_MoveAnimationCoroutine = MoveAnimationCouroutine();
        StartCoroutine(m_MoveAnimationCoroutine);
    }

    public virtual void StopMoveAnimation()
    {
        StopCoroutine(m_MoveAnimationCoroutine);
    }

    // Shoot
    public virtual void StartShootAnimation()
    {
        m_ShootAnimationCoroutine = ShootAnimationCouroutine();
        StartCoroutine(m_ShootAnimationCoroutine);
    }

    public virtual void StopShootAnimation()
    {
        StopCoroutine(m_ShootAnimationCoroutine);
    }

    public virtual void PauseShootAnimation()
    {
        // If there is no bullet yet, then the gun has not fired. In that case, just stop the whole animation.
        if (m_Bullet == null)
        {
            StopShootAnimation();
        }
        // If there is already a bullet, it is moving somewhere. Pause it so it stops moving.
        else
        {
            m_Bullet.SetPaused(true);
        }
    }

    public virtual void ResumeShootAnimation()
    {
        // If there is no bullet yet, then the gun has not fired. In that case, just restart the whole animation,
        // since it will just continue where it left off.
        if (m_Bullet == null)
        {
            StartShootAnimation();
        }
        // If there is already a bullet, it is frozen somewhere. Unpause it so it can continue moving.
        else
        {
            m_Bullet.SetPaused(false);
        }
    }

}