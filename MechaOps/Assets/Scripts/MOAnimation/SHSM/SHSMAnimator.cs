using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent]
public class SHSMAnimator : MOAnimator
{
    // Gun Elevation
    [SerializeField, Range(-90, 0)] protected float m_MaxGunElevation = -80.0f;
    [SerializeField, Range(0, 90)] protected float m_MaxGunDepression = 20.0f;

    // Weapons
    [SerializeField] protected GameObject m_Turret;
    [SerializeField] protected GameObject m_Gun;

    // Hull
    [SerializeField] protected GameObject m_Hull;

    // Wheels
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
    protected ArtileryBullet m_Bullet = null;
    protected Tile m_TargetTile = null;
    protected float m_GunElevationSpeed = 60.0f; // This is the speed in degrees.
    protected float m_AccuracyTolerance = 0.5f;

    // The shooting animation is nowhere near realistic etc.
    // I'm not going to bother calculating the correct elevation for the gun etc.
    // We'll just fix it at this values and fake it.
    // The gun will be at this elevation when not shooting.
    protected float m_GunNeutralElevation = 0.0f;
    // The gun will be at this elevation when shooting.
    protected float m_GunShootingElevation = -40.0f;

    protected float m_BulletHorizontalVelocity = 10.0f;
    protected float m_BulletGravityAcceleration = -9.8f;
    protected Void_Void m_ShootAnimationCompleteCallback = null;
    IEnumerator m_ShootAnimationCoroutine;

    public float MaxGunElevation
    {
        get { return m_MaxGunElevation; }
        set { m_MaxGunElevation = Mathf.Clamp(value, -90.0f, 0.0f); }
    }
    public float MaxGunDepression
    {
        get { return m_MaxGunDepression; }
        set { m_MaxGunDepression = Mathf.Clamp(value, 0.0f, 90.0f); }
    }

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
        m_Gun.transform.Rotate(_angle, 0.0f, 0.0f);

        // Limit our angle. The possible angle we can get ranges from -360 Degrees to 360 Degrees.
        // We need to convert it to the  from -180 Degress to 180 Degress range.
        float currentAngle = ConvertAngle(m_Gun.transform.localRotation.eulerAngles.x);
        currentAngle = Mathf.Clamp(currentAngle, m_MaxGunElevation, m_MaxGunDepression);
        Quaternion desiredRotation = new Quaternion();
        desiredRotation.eulerAngles = new Vector3(currentAngle, 0.0f, 0.0f);
        m_Gun.transform.localRotation = desiredRotation;
    }

    protected virtual void ElevateGunTowardsAngle(float _angle)
    {
        float currentAngle = ConvertAngle(m_Gun.transform.localRotation.eulerAngles.x);
        float angleDifference = _angle - currentAngle;

        if (angleDifference > 0.0f)
        {
            AnimateElevateGun(Mathf.Min(angleDifference, m_GunElevationSpeed * Time.deltaTime));
        }
        else if (angleDifference < 0.0f)
        {
            AnimateElevateGun(Mathf.Max(angleDifference, -m_GunElevationSpeed * Time.deltaTime));
        }
    }

    protected virtual bool IsGunAtElevation(float _desiredElevation)
    {
        float currentAngle = ConvertAngle(m_Gun.transform.localRotation.eulerAngles.x);
        float angleDifference = _desiredElevation - currentAngle;

        if (Mathf.Abs(angleDifference) <= m_AccuracyTolerance)
        {
            return true;
        }

        if (_desiredElevation < m_MaxGunElevation)
        {
            return (Mathf.Abs(m_MaxGunElevation - currentAngle) <= m_AccuracyTolerance);
        }

        if (_desiredElevation > m_MaxGunDepression)
        {
            return (Mathf.Abs(m_MaxGunDepression - currentAngle) <= m_AccuracyTolerance);
        }

        return false;
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

    protected virtual void CalculateBulletTrajectory(Vector3 _origin, Vector3 _target, out Vector3 _initialVelocity, out float _timeToTarget)
    {
        Vector3 horizontalDirectionToTarget = _target - _origin;
        horizontalDirectionToTarget.y = 0.0f;
        float horizontalDistance = horizontalDirectionToTarget.magnitude;
        float verticalDistance = _target.y - _origin.y;

        // Find out how long it takes for the bullet to reach the target given the horizontal velocity.
        float timeToTarget = horizontalDirectionToTarget.magnitude / m_BulletHorizontalVelocity;

        // We're going to cheat a bit here. Rather than having a fixed initial bullet velocity magnitude,
        // we will adjust the vertical velocity so that our bullet can always reach the target.
        // Not the most realistic thing on earth, but this is a game, so deal with it.
        // Vertical Distance = Initial Vertical Velocity + 0.5 * Gravity Acceleration * (Time To Target)^2
        // Initial Vertical Velocity = (Vertical Distance / Time To Target) - 0.5 * Gravity Acceleration * Time To Target
        float bulletInitialVerticalVelocity = (verticalDistance / timeToTarget) - (0.5f * m_BulletGravityAcceleration * timeToTarget);

        Vector3 bulletVelocity = horizontalDirectionToTarget.normalized * m_BulletHorizontalVelocity;
        bulletVelocity.y += bulletInitialVerticalVelocity;

        _initialVelocity = bulletVelocity;
        _timeToTarget = timeToTarget;
    }

    public virtual void SetMoveAnimationParameters(Vector3 _destination, Void_Void _callback)
    {
        m_Destination = _destination;
        m_MoveAnimationCompleteCallback = _callback;
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

    protected virtual IEnumerator ShootAnimationCouroutine()
    {
        // Turn towards the target.
        while (true)
        {
            if (IsFacingTargetPosition(m_TargetTile.transform.position)) { break; }
            RotateTowardsTargetPosition(m_TargetTile.transform.position);
            yield return null;
            continue;
        }

        // Raise our gun.
        while (true)
        {
            if (IsGunAtElevation(m_GunShootingElevation)) { break; }
            ElevateGunTowardsAngle(m_GunShootingElevation);
            yield return null;
            continue;
        }

        // Fire!
        AnimateMuzzleFlash();
        m_Bullet = GameObject.Instantiate(m_Bullet_Prefab.gameObject).GetComponent<ArtileryBullet>();
        m_Bullet.transform.position = m_BulletSpawn.transform.position;
        m_Bullet.TargetTile = m_TargetTile;
        m_Bullet.CompletionCallback = m_ShootAnimationCompleteCallback;
        m_Bullet.Gravity = new Vector3(0.0f, m_BulletGravityAcceleration, 0.0f);

        Vector3 bulletVelocity;
        float bulletLifetime;
        CalculateBulletTrajectory(m_Bullet.transform.position, m_TargetTile.transform.position, out bulletVelocity, out bulletLifetime);
        m_Bullet.Lifetime = bulletLifetime + 1.0f;
        m_Bullet.InitialVelocity = bulletVelocity;

        // Wait for a while.
        yield return new WaitForSeconds(1.0f);

        // Lower our gun.
        while (true)
        {
            if (IsGunAtElevation(m_GunNeutralElevation)) { break; }
            ElevateGunTowardsAngle(m_GunNeutralElevation);
            yield return null;
            continue;
        }
    }

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
        m_MoveAnimationCoroutine = null;
    }

    // Shoot
    public virtual void SetShootAnimationParameters(Tile _targetTile, Void_Void _callback)
    {
        Assert.IsNotNull(_targetTile, MethodBase.GetCurrentMethod().Name + " - _targetTile must not be null!");
        m_TargetTile = _targetTile;
        m_ShootAnimationCompleteCallback = _callback;
    }

    public virtual void StartShootAnimation()
    {
        m_ShootAnimationCoroutine = ShootAnimationCouroutine();
        StartCoroutine(m_ShootAnimationCoroutine);
    }

    public virtual void StopShootAnimation()
    {
        StopCoroutine(m_ShootAnimationCoroutine);
        m_ShootAnimationCoroutine = null;
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

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        MaxGunElevation = m_MaxGunElevation;
        MaxGunDepression = m_MaxGunDepression;
    }
#endif // UNITY_EDTIOR

}