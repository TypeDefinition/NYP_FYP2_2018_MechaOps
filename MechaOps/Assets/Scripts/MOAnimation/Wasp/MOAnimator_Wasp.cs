using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class MOAnimator_Wasp : MOAnimator
{
    // Hull Elevation
    [SerializeField] protected GameObject m_Hull;
    [SerializeField, Range(-90, 0)] protected float m_MaxHullElevation = -80.0f;
    [SerializeField, Range(0, 90)] protected float m_MaxHullDepression = 20.0f;

    // Shooting
    [SerializeField] protected Transform[] m_BulletSpawnPoints;
    [SerializeField] protected GunshipBullet m_Bullet_Prefab;
    [SerializeField] protected GameObject m_MuzzleFlash_Prefab;

    // Shooting Animation
    protected GameObject m_Target = null;
    protected List<GunshipBullet> m_Bullets = new List<GunshipBullet>();
    protected float m_HullElevationSpeed = 45.0f;
    protected float m_AccuracyTolerance = 0.5f; // How small must the angle between where our gun is aiming and where the target is to be considered aimed.
    protected int m_NumBulletsToShoot = 30;
    protected float m_FireRate = 20.0f;

    // Moving Animation
    protected Vector3 m_Destination = new Vector3();
    protected float m_HullMovingElevation = 20.0f;

    // Both Shooting and Moving Animation
    protected float m_HullNeutralElevation = 0.0f;

    public float MaxHullElevation
    {
        get { return m_MaxHullElevation; }
        set { m_MaxHullElevation = Mathf.Clamp(value, -90.0f, 0.0f); }
    }

    public float MaxHullDepression
    {
        get { return m_MaxHullDepression; }
        set { m_MaxHullDepression = Mathf.Clamp(value, 0.0f, 90.0f); }
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
                GameObject.Destroy(m_Bullets[i].gameObject);
            }
        }
        m_Bullets.Clear();
    }

    protected virtual void AnimateMuzzleFlash(int _bulletSpawnIndex)
    {
        GameObject muzzleFlash = GameObject.Instantiate(m_MuzzleFlash_Prefab);
        muzzleFlash.transform.position = m_BulletSpawnPoints[_bulletSpawnIndex].position;
        muzzleFlash.transform.rotation = m_BulletSpawnPoints[_bulletSpawnIndex].rotation;
    }

    protected virtual void AnimateElevateHull(float _angle)
    {
        m_Hull.transform.Rotate(new Vector3(_angle, 0.0f, 0.0f));

        // Limit our angle. The possible angle we can get ranges from -360 Degrees to 360 Degrees.
        // We need to convert it to the  from -180 Degress to 180 Degress range.
        float currentAngle = WrapAngle(m_Hull.transform.localRotation.eulerAngles.x);
        currentAngle = Mathf.Clamp(currentAngle, m_MaxHullElevation, m_MaxHullDepression);
        Quaternion desiredRotation = new Quaternion();
        desiredRotation.eulerAngles = new Vector3(currentAngle, 0.0f, 0.0f);
        m_Hull.transform.localRotation = desiredRotation;
    }

    protected virtual void ElevateHullTowardsAngle(float _angle)
    {
        float currentAngle = WrapAngle(m_Hull.transform.localRotation.eulerAngles.x);
        float angleDifference = _angle - currentAngle;

        if (angleDifference > 0.0f)
        {
            AnimateElevateHull(Mathf.Min(angleDifference, m_HullElevationSpeed * Time.deltaTime));
        }
        else if (angleDifference < 0.0f)
        {
            AnimateElevateHull(Mathf.Max(angleDifference, -m_HullElevationSpeed * Time.deltaTime));
        }
    }

    // Move Animation
    protected virtual bool IsHullAtElevation(float _desiredElevation)
    {
        float currentAngle = WrapAngle(m_Hull.transform.localRotation.eulerAngles.x);
        float angleDifference = _desiredElevation - currentAngle;

        if (Mathf.Abs(angleDifference) <= m_AccuracyTolerance)
        {
            return true;
        }

        if (_desiredElevation < m_MaxHullElevation)
        {
            return (Mathf.Abs(m_MaxHullElevation - currentAngle) <= m_AccuracyTolerance);
        }

        if (_desiredElevation > m_MaxHullDepression)
        {
            return (Mathf.Abs(m_MaxHullDepression - currentAngle) <= m_AccuracyTolerance);
        }

        return false;
    }

    protected override IEnumerator MoveAnimationCouroutine()
    {
        // Rotate our hull to Moving Elevation.
        while (true)
        {
            // Do nothing if it is paused.
            while (m_MoveAnimationPaused) { yield return null; }

            if (IsHullAtElevation(m_HullMovingElevation)) { break; }
            ElevateHullTowardsAngle(m_HullMovingElevation);
            yield return null;
        }

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

        // Rotate our hull to Neutral Elevation.
        while (true)
        {
            // Do nothing if it is paused.
            while (m_MoveAnimationPaused) { yield return null; }

            if (IsHullAtElevation(m_HullNeutralElevation)) { break; }
            ElevateHullTowardsAngle(m_HullNeutralElevation);
            yield return null;
            continue;
        }

        // We've reached the end of the path.
        InvokeCallback(m_MoveAnimationCompletionCallback);
    }

    // Shoot Animation
    /// <summary>
    /// Pitch our Gunship so that it is facing at a target.
    /// </summary>
    /// <param name="_targetPosition"></param>
    protected virtual bool ElevateHullTowardsTargetPosition(Vector3 _targetPosition)
    {
        if (IsFacingVerticalPosition(m_Hull.transform, _targetPosition, m_AccuracyTolerance, m_MaxHullElevation, m_MaxHullDepression))
        {
            return true;
        }

        Vector3 directionToTarget = _targetPosition - m_Hull.transform.position;
        directionToTarget.Normalize();
        float angleToTarget = -Mathf.Asin(directionToTarget.y) * Mathf.Rad2Deg; // Negate this because a position rotation is downwards.
        float currentAngle = WrapAngle(m_Hull.transform.localRotation.eulerAngles.x);
        float angleDifference = angleToTarget - currentAngle;

        if (angleDifference > 0.0f)
        {
            AnimateElevateHull(Mathf.Min(angleDifference, m_HullElevationSpeed * Time.deltaTime));
        }
        else if (angleDifference < 0.0f)
        {
            AnimateElevateHull(Mathf.Max(angleDifference, -m_HullElevationSpeed * Time.deltaTime));
        }

        return IsFacingVerticalPosition(m_Hull.transform, _targetPosition, m_AccuracyTolerance, m_MaxHullElevation, m_MaxHullDepression);
    }

    protected void FireBullet(Transform _spawnPoint, GameObject _target)
    {
        // Spawn Bullet
        GunshipBullet bullet = GameObject.Instantiate(m_Bullet_Prefab.gameObject).GetComponent<GunshipBullet>();
        bullet.Target = _target;
        bullet.gameObject.transform.position = _spawnPoint.position;
        bullet.gameObject.transform.LookAt(_target.transform.position);
        m_Bullets.Add(bullet);
    }

    protected override IEnumerator ShootAnimationCouroutine()
    {
        // Turn towards the target.
        while (true)
        {
            // Do nothing while paused.
            while (m_ShootAnimationPaused) { yield return null; }

            if (FaceTowardsPositionHorizontally(transform, m_Target.transform.position, m_AccuracyTolerance)) { break; }
            yield return null;
        }

        // Rotate our hull so that it aims at the target.
        while (true)
        {
            // Do nothing while paused.
            while (m_ShootAnimationPaused) { yield return null; }

            if (ElevateHullTowardsTargetPosition(m_Target.transform.position)) { break; }
            yield return null;
        }

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

        // Rotate our hull back to Neutral Elevation.
        while (true)
        {
            // Do nothing while paused.
            while (m_ShootAnimationPaused) { yield return null; }

            if (IsHullAtElevation(m_HullNeutralElevation)) { break; }
            ElevateHullTowardsAngle(m_HullNeutralElevation);
            yield return null;
        }

        // I know this is bad, but we are shooting multiple bullets, so we cannot use a bullet callback to signal completion.
        while (HasActiveBullets())
        {
            yield return null;
        }
        ClearBullets();

        InvokeCallback(m_ShootAnimationCompletionCallback);
    }

    // Tailored Animations

    // Shoot
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
        MaxHullElevation = m_MaxHullElevation;
        MaxHullDepression = m_MaxHullDepression;
    }
#endif // UNITY_EDTIOR

}