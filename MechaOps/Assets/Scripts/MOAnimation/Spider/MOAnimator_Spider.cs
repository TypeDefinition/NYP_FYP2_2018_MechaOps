using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class MOAnimator_Spider : MOAnimator
{
    // Gun Elevation
    [SerializeField, Range(-90, 0)] protected float m_MaxGunElevation = -80.0f;
    [SerializeField, Range(0, 90)] protected float m_MaxGunDepression = 20.0f;

    // Hull
    [SerializeField] private Transform m_Hull;

    // Weapon(s)
    [SerializeField] protected Transform m_Turret;
    [SerializeField] protected Transform m_Gun;
    [SerializeField] protected GameObject m_MuzzleFlash_Prefab;

    // Bullet
    [SerializeField] protected Transform[] m_BulletSpawnPoints;
    [SerializeField] protected MachineGunBullet m_Bullet_Prefab;

    // Tracks & Wheels
    protected float m_WheelSpeedMultiplier = 300.0f;
    [SerializeField] protected Transform[] m_LeftWheels;
    [SerializeField] protected Transform[] m_RightWheels;

    // Audio
    [SerializeField] protected AudioClip m_ShootGunfireSFX = null;

    // Shooting Animation
    protected GameObject m_Target = null;
    protected List<MachineGunBullet> m_Bullets = new List<MachineGunBullet>();
    protected float m_TurretRotationSpeed = 5.0f; // This speed is not in degrees. It's just by time cause I'm lazy.
    protected float m_GunElevationSpeed = 60.0f; // This is the speed in degrees.
    protected float m_AccuracyTolerance = 0.5f; // How small must the angle between where our gun is aiming and where the target is to be considered aimed.
    protected int m_NumBulletsToShoot = 30;
    protected float m_FireRate = 20.0f;

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

    protected bool HasActiveBullets()
    {
        for (int i = 0; i < m_Bullets.Count; ++i)
        {
            if (m_Bullets[i] == null)
            {
                m_Bullets.RemoveAt(i--);
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    protected void RemoveDestroyBulletsFromList()
    {
        for (int i = 0; i < m_Bullets.Count; ++i)
        {
            if (m_Bullets[i] == null)
            {
                m_Bullets.RemoveAt(i--);
            }
        }
    }

    protected void ClearBullets()
    {
        for (int i = 0; i < m_Bullets.Count; ++i)
        {
            if (m_Bullets[i] != null)
            {
                Destroy(m_Bullets[i].gameObject);
            }
        }
        m_Bullets.Clear();
    }

    protected virtual void AnimateWheels(float _leftSpeed, float _rightSpeed)
    {
        AnimateWheelsLeft(_leftSpeed);
        AnimateWheelsRight(_rightSpeed);
    }

    protected virtual void AnimateWheelsLeft(float _speed)
    {
        foreach (Transform wheel in m_LeftWheels)
        {
            wheel.Rotate(_speed, 0.0f, 0.0f);
        }
    }

    protected virtual void AnimateWheelsRight(float _speed)
    {
        foreach (Transform wheel in m_RightWheels)
        {
            wheel.Rotate(_speed, 0.0f, 0.0f);
        }
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

    protected virtual void AnimateMuzzleFlash(int _bulletSpawnIndex)
    {
        GameObject muzzleFlash = GameObject.Instantiate(m_MuzzleFlash_Prefab);
        muzzleFlash.transform.position = m_BulletSpawnPoints[_bulletSpawnIndex].position;
        muzzleFlash.transform.rotation = m_BulletSpawnPoints[_bulletSpawnIndex].rotation;
    }

    // Death Animation
    protected override IEnumerator DeathAnimationCoroutine()
    {
        if (m_ViewScript.IsVisibleToPlayer())
        {
            GameEventSystem.GetInstance().TriggerEvent<Transform, Transform>(m_GameSystemsDirectory.GetGameEventNames().GetEventName(GameEventNames.GameplayNames.SetCineUserTransform), m_Hull, m_Hull);
            GameEventSystem.GetInstance().TriggerEvent<string, float>(m_GameSystemsDirectory.GetGameEventNames().GetEventName(GameEventNames.GameplayNames.StartCinematic), m_DeathCinematicName, m_TimeDelayForDeathCam);
        }
        StopAmbientAudioSource();
        yield return new WaitForSeconds(m_TimeDelayForDeathCam);

        InvokeCallback(m_DeathAnimationCompletionCallback);
    }

    public override void StartDeathAnimation(Void_Void _completionCallback)
    {
        base.StartDeathAnimation(_completionCallback);
    }

    // Move Animations
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

    protected override bool MoveTowardsTile(int _movementPathIndex)
    {
        AnimateWheels(m_MovementSpeed * Time.deltaTime * m_WheelSpeedMultiplier, m_MovementSpeed * Time.deltaTime * m_WheelSpeedMultiplier);
        return base.MoveTowardsTile(_movementPathIndex);
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

    protected virtual bool FaceGunTowardsVerticalPosition(Vector3 _targetPosition)
    {
        if (IsFacingVerticalPosition(m_Gun.transform, _targetPosition, m_AccuracyTolerance, m_MaxGunElevation, m_MaxGunDepression)) { return true; }

        Vector3 directionToTarget = _targetPosition - m_Gun.transform.position;
        directionToTarget.Normalize();

        float angleToTarget = -Mathf.Asin(directionToTarget.y) * Mathf.Rad2Deg; // Negate this because a position rotation is downwards.
        float currentAngle = WrapAngle(m_Gun.transform.localRotation.eulerAngles.x);
        float angleDifference = angleToTarget - currentAngle;

        if (angleDifference > 0.0f)
        {
            AnimateElevateGun(Mathf.Min(angleDifference, m_GunElevationSpeed * Time.deltaTime));
        }
        else if (angleDifference < 0.0f)
        {
            AnimateElevateGun(Mathf.Max(angleDifference, -m_GunElevationSpeed * Time.deltaTime));
        }

        return IsFacingVerticalPosition(m_Gun.transform, _targetPosition, m_AccuracyTolerance, m_MaxGunElevation, m_MaxGunDepression);
    }

    protected void FireBullet(Transform _spawnPoint, GameObject _target)
    {
        // Spawn Bullet
        MachineGunBullet bullet = Instantiate(m_Bullet_Prefab.gameObject).GetComponent<MachineGunBullet>();
        bullet.Target = _target;
        bullet.gameObject.transform.position = _spawnPoint.position;
        bullet.gameObject.transform.LookAt(_target.transform.position);
        m_Bullets.Add(bullet);
    }

    protected override IEnumerator ShootAnimationCouroutine()
    {
        // Rotate our turret to face our target.
        while (true)
        {
            // Do nothing if we are paused.
            while (m_ShootAnimationPaused) { yield return null; }

            if (FaceTowardsPositionHorizontally(m_Turret, m_Target.transform.position, m_AccuracyTolerance)) { break; }
            yield return null;
        }

        while (true)
        {
            // Do nothing if we are paused.
            while (m_ShootAnimationPaused) { yield return null; }

            if (FaceGunTowardsVerticalPosition(m_Target.transform.position)) { break; }
            yield return null;
        }

        // Start the audio.
        PlaySFXAudioSource(m_ShootGunfireSFX, true);

        // Fire!
        int bulletsLeft = m_NumBulletsToShoot;
        int spawnPointIndex = 0;
        float timeToFire = 0.0f;
        while (bulletsLeft > 0)
        {
            // Do nothing while paused.
            while (m_ShootAnimationPaused) { yield return null; }

            timeToFire -= Time.deltaTime;
            if (timeToFire <= 0.0f)
            {
                FireBullet(m_BulletSpawnPoints[spawnPointIndex], m_Target);
                --bulletsLeft;
                spawnPointIndex = (spawnPointIndex + 1) % m_BulletSpawnPoints.Length;
                timeToFire = (1.0f / m_FireRate);
            }

            yield return null;
        }

        // Stop the audio.
        StopSFXAudioSource();

        // I know this is bad, but we are shooting multiple bullets, so we cannot use a bullet callback to signal completion.
        while (HasActiveBullets())
        {
            yield return null;
        }
        ClearBullets();

        // Invoke the completion callback.
        InvokeCallback(m_ShootAnimationCompletionCallback);
    }

    public virtual void StartShootAnimation(GameObject _target, Void_Void _completionCallback)
    {
        Assert.IsTrue(_target != null);
        m_Target = _target;
        m_ShootAnimationCompletionCallback = _completionCallback;

        m_ShootAnimationCoroutine = ShootAnimationCouroutine();
        StartCoroutine(m_ShootAnimationCoroutine);
    }

    public override void PauseShootAnimation()
    {
        base.PauseShootAnimation();
        RemoveDestroyBulletsFromList();
        for (int i = 0; i < m_Bullets.Count; ++i) { m_Bullets[i].SetPaused(true); }
    }

    public override void ResumeShootAnimation()
    {
        base.ResumeShootAnimation();
        RemoveDestroyBulletsFromList();
        for (int i = 0; i < m_Bullets.Count; ++i) { m_Bullets[i].SetPaused(false); }
    }

    public override void StopShootAnimation()
    {
        base.StopShootAnimation();
        ClearBullets();
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        MaxGunElevation = m_MaxGunElevation;
        MaxGunDepression = m_MaxGunDepression;
    }
#endif // UNITY_EDTIOR
}