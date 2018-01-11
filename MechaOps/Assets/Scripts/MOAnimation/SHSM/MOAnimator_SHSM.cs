using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent]
public class MOAnimator_SHSM : MOAnimator
{
    // Gun Elevation
    [SerializeField, Range(-90, 0)] protected float m_MaxGunElevation = -80.0f;
    [SerializeField, Range(0, 90)] protected float m_MaxGunDepression = 20.0f;

    // Weapons
    [SerializeField] protected Transform m_Gun;

    // Hull
    [SerializeField] protected Transform m_Hull;

    // Wheels
    [SerializeField] protected Transform[] m_LeftWheels;
    [SerializeField] protected Transform[] m_RightWheels;

    // Bullet
    [SerializeField] protected Transform m_BulletSpawnPoint;
    [SerializeField] protected ArtileryBullet m_Bullet_Prefab;
    [SerializeField] protected GameObject m_MuzzleFlash_Prefab;

    // Audio
    [SerializeField] protected AudioClip m_ShootGunfireSFX = null;

    // Shoot Animation
    /* The shooting animation is nowhere near realistic etc.
    I'm not going to bother calculating the correct elevation for the gun etc.
    We'll just fix it at this values and fake it.*/
    // The gun will be at this elevation when not shooting.
    protected float m_GunNeutralElevation = 0.0f;
    // The gun will be at this elevation when shooting.
    protected float m_GunShootingElevation = -50.0f;

    protected Tile m_TargetTile = null;
    protected ArtileryBullet m_Bullet = null;
    protected float m_GunElevationSpeed = 60.0f; // This is the speed in degrees.
    protected float m_AccuracyTolerance = 0.5f;
    protected float m_BulletHorizontalVelocity = 20.0f;
    protected float m_BulletGravityAcceleration = -9.8f;
    protected bool m_BulletAnimationComplete = false;

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
        foreach (Transform wheel in m_LeftWheels)
        {
            wheel.Rotate(_speed, 0.0f, 0.0f);
        }
    }

    public virtual void AnimateWheelsRight(float _speed)
    {
        foreach (Transform wheel in m_RightWheels)
        {
            wheel.Rotate(_speed, 0.0f, 0.0f);
        }
    }

    public virtual void AnimateMuzzleFlash()
    {
        GameObject muzzleFlash = GameObject.Instantiate(m_MuzzleFlash_Prefab);
        muzzleFlash.transform.position = m_BulletSpawnPoint.transform.position;
        muzzleFlash.transform.rotation = m_BulletSpawnPoint.transform.rotation;
    }

    public virtual void AnimateElevateGun(float _angle)
    {
        m_Gun.transform.Rotate(_angle, 0.0f, 0.0f);

        // Limit our angle. The possible angle we can get ranges from -360 Degrees to 360 Degrees.
        // We need to convert it to the  from -180 Degress to 180 Degress range.
        float currentAngle = WrapAngle(m_Gun.transform.localRotation.eulerAngles.x);
        currentAngle = Mathf.Clamp(currentAngle, m_MaxGunElevation, m_MaxGunDepression);
        Quaternion desiredRotation = new Quaternion();
        desiredRotation.eulerAngles = new Vector3(currentAngle, 0.0f, 0.0f);
        m_Gun.transform.localRotation = desiredRotation;
    }

    // Move Animation
    protected override bool FaceTowardsTile(int _movementPathIndex)
    {
        Tile tile = m_TileSystem.GetTile(m_MovementPath[_movementPathIndex]);
        Assert.IsNotNull(tile, MethodBase.GetCurrentMethod().Name + " - Tile " + m_MovementPath[_movementPathIndex].GetAsString() + " not found!");

        // If the direction is towards the right.
        if (IsTargetOnRight(transform, tile.transform.position))
        {
            AnimateWheels(m_MovementSpeed * Time.deltaTime, -m_MovementSpeed * Time.deltaTime);
        }
        // If the direction is towards the left.
        else
        {
            AnimateWheels(-m_MovementSpeed * Time.deltaTime, m_MovementSpeed * Time.deltaTime);
        }

        return FaceTowardsPositionHorizontally(transform, tile.transform.position, m_RotationTolerance);
    }

    // Shoot Animation
    protected virtual void ElevateGunTowardsAngle(float _angle)
    {
        float currentAngle = WrapAngle(m_Gun.transform.localRotation.eulerAngles.x);
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
        float currentAngle = WrapAngle(m_Gun.transform.localRotation.eulerAngles.x);
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

    protected virtual void FireGun(Tile _targetTile)
    {
        // Spawn Muzzle Flash
        AnimateMuzzleFlash();

        // Play Audio
        PlayOneShotSFXAudioSource(m_ShootGunfireSFX);

        // Spawn Bullet
        m_Bullet = Instantiate(m_Bullet_Prefab.gameObject).GetComponent<ArtileryBullet>();
        m_Bullet.transform.position = m_BulletSpawnPoint.transform.position;
        m_Bullet.TargetTile = _targetTile;
        m_Bullet.CompletionCallback = OnBulletAnimationComplete;
        m_Bullet.Gravity = new Vector3(0.0f, m_BulletGravityAcceleration, 0.0f);
        GameEventSystem.GetInstance().TriggerEvent<GameObject>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.FollowTarget), m_Bullet.gameObject);

        Vector3 bulletVelocity;
        float bulletLifetime;
        CalculateBulletTrajectory(m_Bullet.transform.position, _targetTile.transform.position, out bulletVelocity, out bulletLifetime);
        m_Bullet.Lifetime = bulletLifetime + 1.0f;
        m_Bullet.InitialVelocity = bulletVelocity;
    }

    protected virtual void OnBulletAnimationComplete()
    {
        m_BulletAnimationComplete = true;
        GameEventSystem.GetInstance().TriggerEvent<GameObject>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.FollowTarget), null);
    }

    protected override IEnumerator ShootAnimationCouroutine()
    {
        // Turn towards the target.
        while (true)
        {
            // Do nothing if we are paused.
            while (m_ShootAnimationPaused) { yield return null; }

            if (FaceTowardsPositionHorizontally(transform, m_TargetTile.transform.position, m_AccuracyTolerance)) { break; }
            yield return null;
        }

        // Raise our gun.
        while (true)
        {
            // Do nothing if we are paused.
            while (m_ShootAnimationPaused) { yield return null; }

            if (IsGunAtElevation(m_GunShootingElevation)) { break; }
            ElevateGunTowardsAngle(m_GunShootingElevation);
            yield return null;
        }

        // Fire!
        FireGun(m_TargetTile);

        // Wait for a while.
        yield return new WaitForSeconds(1.0f);

        // Lower our gun.
        while (true)
        {
            // Do nothing if we are paused.
            while (m_ShootAnimationPaused) { yield return null; }

            if (IsGunAtElevation(m_GunNeutralElevation)) { break; }
            ElevateGunTowardsAngle(m_GunNeutralElevation);
            yield return null;
        }

        // Wait for the bullet to hit its target.
        while (!m_BulletAnimationComplete) { yield return null; }
        m_BulletAnimationComplete = false;

        // Invoke the completion callback.
        InvokeCallback(m_ShootAnimationCompletionCallback);
    }

    // Shoot Animation
    public void StartShootAnimation(Tile _targetTile, Void_Void _callback)
    {
        Assert.IsNotNull(_targetTile, MethodBase.GetCurrentMethod().Name + " - _targetTile must not be null!");
        m_TargetTile = _targetTile;
        m_ShootAnimationCompletionCallback = _callback;

        m_ShootAnimationCoroutine = ShootAnimationCouroutine();
        StartCoroutine(m_ShootAnimationCoroutine);
    }

    public override void PauseShootAnimation()
    {
        base.PauseShootAnimation();
        if (m_Bullet != null)
        {
            GameEventSystem.GetInstance().TriggerEvent<GameObject>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.FollowTarget), null);
            m_Bullet.SetPaused(true);
        }
    }

    public override void ResumeShootAnimation()
    {
        base.ResumeShootAnimation();
        if (m_Bullet != null)
        {
            GameEventSystem.GetInstance().TriggerEvent<GameObject>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.FollowTarget), m_Bullet.gameObject);
            m_Bullet.SetPaused(false);
        }
    }

    public override void StopShootAnimation()
    {
        base.StopShootAnimation();
        if (m_Bullet != null)
        {
            GameEventSystem.GetInstance().TriggerEvent<GameObject>(m_GameEventNames.GetEventName(GameEventNames.GameUINames.FollowTarget), null);
            Destroy(m_Bullet.gameObject);
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