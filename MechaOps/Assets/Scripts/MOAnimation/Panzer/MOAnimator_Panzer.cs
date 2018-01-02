using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent]
public class MOAnimator_Panzer : MOAnimator
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
    [SerializeField] protected Transform m_BulletSpawnPoint;
    [SerializeField] protected TankBullet m_Bullet_Prefab;

    // Tracks & Wheels
    [SerializeField] protected MeshRenderer m_LeftTracks;
    [SerializeField] protected MeshRenderer m_RightTracks;
    [SerializeField] protected Transform[] m_LeftWheels;
    [SerializeField] protected Transform[] m_RightWheels;
    // How fast the wheels should turn relative to the tracks.
    protected float m_TracksToWheelsSpeedRatio = 800.0f;

    // Audio
    [SerializeField] protected AudioClip m_ShootGunfireSFX = null;

    // Shoot Animation
    protected GameObject m_Target = null;
    protected bool m_Hit = false; // Is the shot a hit or miss?
    protected float m_TurretRotationSpeed = 5.0f; // This speed is not in degrees. It's just by time cause I'm lazy.
    protected float m_GunElevationSpeed = 60.0f; // This is the speed in degrees.
    protected float m_AccuracyTolerance = 0.5f; // How small must the angle between where our gun is aiming and where the target is to be considered aimed.
    protected bool m_BulletAnimationComplete = false;
    protected TankBullet m_Bullet = null;

    // Cinematics
    [SerializeField, Tooltip("Time taken for delay in the animation from camera to the turret during camera cinematics")]
    protected float m_TimeDelayForCamToTurret = 3.0f;
    [SerializeField, Tooltip("Time taken for the death cam animation")]
    protected float m_TimeDelayForDeathCam = 2.0f;
    [SerializeField, Tooltip("Time taken for the delay before camera goes back to normal from cinematics")]
    protected float m_TimeDelayForCamBackToNormal = 0.5f;
    [SerializeField, Tooltip("Name for walking cinematic shot for panzer walk at CineMachineHandler")]
    protected string m_PanzerWalkCinematicName = "Walk";
    [SerializeField, Tooltip("Name for attack cinematic shot for panzer attack at CineMachineHandler")]
    protected string m_PanzerAttackCinematicName = "Attack";
    [SerializeField, Tooltip("Name for death cinematic shot for panzer death at CineMachineHandler")]
    protected string m_PanzerDeathCinematicName = "Die";

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

    protected virtual void AnimateTracks(float _leftSpeed, float _rightSpeed)
    {
        AnimateTracksLeft(_leftSpeed);
        AnimateTracksRight(_rightSpeed);
    }

    protected virtual void AnimateTracksLeft(float _speed)
    {
        Material trackMaterial = m_LeftTracks.material;
        Vector2 textureOffset = new Vector2(trackMaterial.mainTextureOffset.x, trackMaterial.mainTextureOffset.y);
        textureOffset.x += _speed;
        trackMaterial.mainTextureOffset = textureOffset;
        trackMaterial.SetTextureOffset("_BumpMap", textureOffset);

        foreach (Transform wheel in m_LeftWheels)
        {
            wheel.Rotate(_speed * m_TracksToWheelsSpeedRatio, 0.0f, 0.0f);
        }
    }

    protected virtual void AnimateTracksRight(float _speed)
    {
        Material trackMaterial = m_RightTracks.material;
        Vector2 textureOffset = trackMaterial.mainTextureOffset;
        textureOffset.x += _speed;
        trackMaterial.mainTextureOffset = textureOffset;
        trackMaterial.SetTextureOffset("_BumpMap", textureOffset);

        foreach (Transform wheel in m_RightWheels)
        {
            wheel.Rotate(_speed * m_TracksToWheelsSpeedRatio, 0.0f, 0.0f);
        }
    }

    protected virtual void AnimateTurretRotate(float _angle)
    {
        m_Turret.Rotate(0.0f, _angle, 0.0f);
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

    protected virtual void AnimateMuzzleFlash()
    {
        GameObject muzzleFlash = GameObject.Instantiate(m_MuzzleFlash_Prefab);
        muzzleFlash.transform.position = m_BulletSpawnPoint.position;
        muzzleFlash.transform.rotation = m_BulletSpawnPoint.rotation;
    }

    // Death Animation
    protected override IEnumerator DeathAnimationCoroutine()
    {
        StopAmbientAudioSource();

        if (m_CineMachineHandler)
        {
            m_CineMachineHandler.DelayCinemachineSetActive(false, m_TimeDelayForDeathCam);
            yield return new WaitForSeconds(m_TimeDelayForDeathCam);
            yield return new WaitForSeconds(m_TimeDelayForCamBackToNormal);
        }

        InvokeCallback(m_DeathAnimationCompletionCallback);
    }

    public override void StartDeathAnimation(Void_Void _completionCallback)
    {
        // Regardless of the tag, show the death cinematic for both sides
        if (m_CineMachineHandler)
        {
            m_CineMachineHandler.SetPanzerCinematicCamActive("Die");
            m_CineMachineHandler.ActiveCamBase.LookAt = m_Hull;
            m_CineMachineHandler.ActiveCamBase.Follow = m_Turret.transform;
        }

        base.StartDeathAnimation(_completionCallback);
    }

    // Move Animation
    protected override bool FaceTowardsTile(int _movementPathIndex)
    {
        Tile tile = m_TileSystem.GetTile(m_MovementPath[_movementPathIndex]);
        Assert.IsNotNull(tile, MethodBase.GetCurrentMethod().Name + " - Tile " + m_MovementPath[_movementPathIndex].GetAsString() + " not found!");

        // If the direction is towards the right.
        if (IsTargetOnRight(transform, tile.transform.position))
        {
            AnimateTracks(m_MovementSpeed * Time.deltaTime, -m_MovementSpeed * Time.deltaTime);
        }
        // If the direction is towards the left.
        else
        {
            AnimateTracks(-m_MovementSpeed * Time.deltaTime, m_MovementSpeed * Time.deltaTime);
        }

        return FaceTowardsPositionHorizontally(transform, tile.transform.position, m_RotationTolerance);
    }

    protected override bool MoveTowardsTile(int _movementPathIndex)
    {
        Tile tile = m_TileSystem.GetTile(m_MovementPath[_movementPathIndex]);
        Assert.IsNotNull(tile, MethodBase.GetCurrentMethod().Name + " - Tile " + m_MovementPath[_movementPathIndex].GetAsString() + " not found!");

        AnimateTracks(m_MovementSpeed * Time.deltaTime, m_MovementSpeed * Time.deltaTime);

        return MoveTowardsPositionHorizontally(transform, tile.transform.position);
    }

    public override void StartMoveAnimation(TileId[] _movementPath, Void_Int _perTileCallback, Void_Void _completionCallback)
    {
        if (m_CineMachineHandler)
        {
            m_CineMachineHandler.SetPanzerCinematicCamActive("Walk");
            m_CineMachineHandler.ActiveCamBase.LookAt = m_Hull;
            m_CineMachineHandler.ActiveCamBase.Follow = m_Hull;
        }

        base.StartMoveAnimation(_movementPath, _perTileCallback, _completionCallback);
    }

    // Shoot Animation
    protected virtual void FireGun(GameObject _target, bool _explodeOnContact, Void_Void _callback)
    {
        // Spawn Muzzle Flash
        AnimateMuzzleFlash();

        // Play Audio
        PlayOneShotSFXAudioSource(m_ShootGunfireSFX);

        // Spawn Bullet
        Assert.IsTrue(_target != null);
        m_Bullet = Instantiate(m_Bullet_Prefab.gameObject).GetComponent<TankBullet>();
        m_Bullet.Hit = m_Hit;
        m_Bullet.transform.position = m_BulletSpawnPoint.transform.position;
        m_Bullet.transform.LookAt(_target.transform.position);
        m_Bullet.CompletionCallback = _callback;
        m_Bullet.Target = _target;
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

    protected virtual void OnBulletAnimationComplete()
    {
        m_BulletAnimationComplete = true;
    }

    protected override IEnumerator ShootAnimationCouroutine()
    {
        // Allow the camera to go to the turret.
        yield return new WaitForSeconds(m_TimeDelayForCamToTurret);

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

        FireGun(m_Target, m_Hit, OnBulletAnimationComplete);

        // CineMachineHandler
        while (!m_BulletAnimationComplete) { yield return null; }
        m_BulletAnimationComplete = false;

        yield return new WaitForSeconds(m_TimeDelayForCamBackToNormal);
        StopCinematicCamera();

        // Invoke the completion callback.
        InvokeCallback(m_ShootAnimationCompletionCallback);
    }

    public virtual void StartShootAnimation(GameObject _target, bool _hit, Void_Void _callback)
    {
        Assert.IsNotNull(_target, MethodBase.GetCurrentMethod().Name + " - _target must not be null!");
        m_Target = _target;
        m_Hit = _hit;
        m_ShootAnimationCompletionCallback = _callback;

        // CineMachineHandler
        if (m_CineMachineHandler)
        {
            int randomNumber = Random.Range(1, 3);
            m_CineMachineHandler.SetPanzerCinematicCamActive("Attack" + randomNumber);
            // unfortunately, need to hardcode the cinmatic abit
            switch (randomNumber)
            {
                case 1:
                    m_CineMachineHandler.ActiveCamBase.Follow = m_Turret.transform;
                    break;
                case 2:
                    // Number 2 refers to Following the target while aiming the cinematic camera at the Turret
                    m_CineMachineHandler.ActiveCamBase.Follow = m_Target.transform;
                    break;
                default:
                    Assert.IsFalse(true, MethodBase.GetCurrentMethod().Name + " - Unhandled m_CineMachineHandler case.");
                    break;
            }
            m_CineMachineHandler.ActiveCamBase.LookAt = m_Gun.transform;
            // and then randomize between 1st and 2nd attack camera cinematics
        }

        // Panzer
        m_ShootAnimationCoroutine = ShootAnimationCouroutine();
        StartCoroutine(m_ShootAnimationCoroutine);
    }

    public override void PauseShootAnimation()
    {
        base.PauseShootAnimation();
        if (m_Bullet != null) { m_Bullet.SetPaused(true); }
    }

    public override void ResumeShootAnimation()
    {
        base.ResumeShootAnimation();
        if (m_Bullet != null) { m_Bullet.SetPaused(false); }
    }

    public override void StopShootAnimation()
    {
        base.StopShootAnimation();
        if (m_Bullet != null) { Destroy(m_Bullet.gameObject); }
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        MaxGunElevation = m_MaxGunElevation;
        MaxGunDepression = m_MaxGunDepression;
    }
#endif // UNITY_EDTIOR

}