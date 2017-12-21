using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent]
public class PanzerAnimator : MOAnimator
{
    // Gun Elevation
    [SerializeField, Range(-90, 0)] protected float m_MaxGunElevation = -80.0f;
    [SerializeField, Range(0, 90)] protected float m_MaxGunDepression = 20.0f;

    // Weapons
    [SerializeField] protected GameObject m_Turret;
    [SerializeField] protected GameObject m_Gun;
    [SerializeField] protected GameObject[] m_MuzzleFlashes;

    // Tracks & Wheels
    [SerializeField] protected GameObject m_LeftTracks;
    [SerializeField] protected GameObject m_RightTracks;
    [SerializeField] protected GameObject[] m_LeftWheels;
    [SerializeField] protected GameObject[] m_RightWheels;
    // How fast the wheels should turn relative to the tracks.
    protected float m_TracksToWheelsSpeedRatio = 800.0f;

    // Bullet
    [SerializeField] protected GameObject m_BulletSpawn;
    [SerializeField] protected TankBullet m_BulletPrefab;

    [SerializeField, Tooltip("Hull of the panzer")] private Transform m_HullTransform;
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

    // Moving Animation
    protected Vector3 m_Destination = new Vector3();
    protected float m_MovementSpeed = 10.0f;
    protected float m_TankRotationSpeed = 3.0f;
    protected Void_Void m_MoveAnimationCompleteCallback = null;
    protected IEnumerator m_MoveAnimationCoroutine;

    // Shooting Animation
    protected GameObject m_Target = null;
    protected bool m_Hit = false; // Is the shot a hit or miss?
    protected float m_TurretRotationSpeed = 5.0f; // This speed is not in degrees. It's just by time cause I'm lazy.
    protected float m_GunElevationSpeed = 60.0f; // This is the speed in degrees.
    protected float m_AccuracyTolerance = 0.5f; // How small must the angle between where our gun is aiming and where the target is to be considered aimed.
    protected bool m_FinishShootAnimationFlag = false;
    protected TankBullet m_Bullet = null;
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

    public virtual void AnimateTracks(float _leftSpeed, float _rightSpeed)
    {
        AnimateTracksLeft(_leftSpeed);
        AnimateTracksRight(_rightSpeed);
    }

    public virtual void AnimateTracksLeft(float _speed)
    {
        Material trackMaterial = m_LeftTracks.GetComponent<MeshRenderer>().material;
        Vector2 textureOffset = new Vector2(trackMaterial.mainTextureOffset.x, trackMaterial.mainTextureOffset.y);
        textureOffset.x += _speed;
        trackMaterial.mainTextureOffset = textureOffset;
        trackMaterial.SetTextureOffset("_BumpMap", textureOffset);

        foreach (GameObject wheel in m_LeftWheels)
        {
            wheel.transform.Rotate(new Vector3(_speed * m_TracksToWheelsSpeedRatio, 0.0f, 0.0f));
        }
    }

    public virtual void AnimateTracksRight(float _speed)
    {
        Material trackMaterial = m_RightTracks.GetComponent<MeshRenderer>().material;
        Vector2 textureOffset = trackMaterial.mainTextureOffset;
        textureOffset.x += _speed;
        trackMaterial.mainTextureOffset = textureOffset;
        trackMaterial.SetTextureOffset("_BumpMap", textureOffset);

        foreach (GameObject wheel in m_RightWheels)
        {
            wheel.transform.Rotate(new Vector3(_speed * m_TracksToWheelsSpeedRatio, 0.0f, 0.0f));
        }
    }

    public virtual void AnimateTurretRotate(float _angle)
    {
        m_Turret.transform.Rotate(new Vector3(0.0f, _angle, 0.0f));
    }

    public virtual void AnimateMuzzleFlash()
    {
        for (int i = 0; i < m_MuzzleFlashes.Length; ++i)
        {
            m_MuzzleFlashes[i].SetActive(true);
            m_MuzzleFlashes[i].GetComponent<SimpleSpriteAnimation>().StartAnimation();
        }
    }

    public virtual void AnimateFireBullet(GameObject _target, bool _explodeOnContact, Void_Void _callback)
    {
        Assert.IsTrue(_target != null);
        m_Bullet = GameObject.Instantiate(m_BulletPrefab.gameObject).GetComponent<TankBullet>();
        m_Bullet.transform.position = m_BulletSpawn.transform.position;
        m_Bullet.transform.LookAt(_target.transform.position);
        m_Bullet.CompletionCallback = _callback;
        m_Bullet.Target = _target;
        // TODO, maybe can show off the slow bullet animation
    }

    public virtual void AnimateFireGun(GameObject _target, bool _bulletExplodeOnContact, Void_Void _callback)
    {
        AnimateMuzzleFlash();
        AnimateFireBullet(_target, _bulletExplodeOnContact, _callback);
    }

    public virtual void AnimateElevateGun(float _angle)
    {
        m_Gun.transform.Rotate(new Vector3(_angle, 0.0f, 0.0f));

        // Limit our angle. The possible angle we can get ranges from -360 Degrees to 360 Degrees.
        // We need to convert it to the  from -180 Degress to 180 Degress range.
        float currentAngle = ConvertAngle(m_Gun.transform.localRotation.eulerAngles.x);
        currentAngle = Mathf.Clamp(currentAngle, m_MaxGunElevation, m_MaxGunDepression);
        Quaternion desiredRotation = new Quaternion();
        desiredRotation.eulerAngles = new Vector3(currentAngle, 0.0f, 0.0f);
        m_Gun.transform.localRotation = desiredRotation;
    }

    // Private Function(s)
    // _maximumAngle is in degrees not radians.
    protected virtual bool IsTurretAimingAtTarget(Vector3 _targetPosition)
    {
        Vector3 directionToTarget = _targetPosition - m_Turret.transform.position;
        directionToTarget.y = 0.0f;
        Vector3 turretForward = m_Turret.transform.forward;
        turretForward.y = 0.0f;

        // Check if the turret is facing the target.
        if (Vector3.Dot(directionToTarget, turretForward) <= 0.0f)
        {
            return false;
        }

        if (Vector3.Angle(directionToTarget, turretForward) > m_AccuracyTolerance)
        {
            return false;
        }

        return true;
    }

    protected virtual bool IsGunAimingAtTarget(Vector3 _targetPosition)
    {
        Vector3 directionToTarget = _targetPosition - m_Gun.transform.position;
        directionToTarget.Normalize();

        float angleToTarget = -Mathf.Asin(directionToTarget.y) * Mathf.Rad2Deg; // Negate this because a position rotation is downwards.
        float currentAngle = ConvertAngle(m_Gun.transform.localRotation.eulerAngles.x);
        float angleDifference = angleToTarget - currentAngle;
        
        if (Mathf.Abs(angleDifference) <= m_AccuracyTolerance)
        {
            return true;
        }

        if (angleToTarget < m_MaxGunElevation)
        {
            return (Mathf.Abs(currentAngle - m_MaxGunElevation) <= m_AccuracyTolerance);
        }

        if (angleToTarget > m_MaxGunDepression)
        {
            return (Mathf.Abs(currentAngle - m_MaxGunDepression) <= m_AccuracyTolerance);
        }

        return false;
    }
    
    protected virtual void RotateTurretTowardsTargetPosition(Vector3 _targetPosition)
    {
        Vector3 directionToTarget = _targetPosition - m_Turret.transform.position;
        directionToTarget.y = 0.0f;

        Quaternion currentRotation = m_Turret.transform.rotation;
        Quaternion desiredRotation = Quaternion.LookRotation(directionToTarget);

        m_Turret.transform.rotation = Quaternion.Slerp(currentRotation, desiredRotation, Time.deltaTime * m_TurretRotationSpeed);
    }

    protected virtual void ElevateGunTowardsTargetPosition(Vector3 _targetPosition)
    {
        Vector3 directionToTarget = _targetPosition - m_Gun.transform.position;
        directionToTarget.Normalize();

        float angleToTarget = -Mathf.Asin(directionToTarget.y) * Mathf.Rad2Deg; // Negate this because a position rotation is downwards.
        float currentAngle = ConvertAngle(m_Gun.transform.localRotation.eulerAngles.x);
        float angleDifference = angleToTarget - currentAngle;

        if (angleDifference > 0.0f)
        {
            //Debug.Log("Angle Difference > 0.0f (" + angleDifference + ")");
            AnimateElevateGun(Mathf.Min(angleDifference, m_GunElevationSpeed * Time.deltaTime));
        }
        else if (angleDifference < 0.0f)
        {
            //Debug.Log("Angle Difference < 0.0f (" + angleDifference + ")");
            AnimateElevateGun(Mathf.Max(angleDifference, -m_GunElevationSpeed * Time.deltaTime));
        }
    }

    protected virtual void RotateTankTowardsTargetPosition(Vector3 _targetPosition)
    {
        Vector3 directionToTarget = _targetPosition - transform.position;
        directionToTarget.y = 0.0f;

        Quaternion currentRotation = gameObject.transform.rotation;
        Quaternion desiredRotation = Quaternion.LookRotation(directionToTarget);

        gameObject.transform.rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * m_TankRotationSpeed);

        // If the direction is towards the right.
        if (Vector3.Dot(transform.right, directionToTarget) > 0.0f)
        {
            AnimateTracks(m_MovementSpeed * Time.deltaTime, -m_MovementSpeed * Time.deltaTime);
        }
        // If the direction is towards the left.
        else
        {
            AnimateTracks(-m_MovementSpeed * Time.deltaTime, m_MovementSpeed * Time.deltaTime);
        }
    }

    protected virtual void TranslateTankTowardsTargetPosition(Vector3 _targetPosition)
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

        AnimateTracks(m_MovementSpeed * Time.deltaTime, m_MovementSpeed * Time.deltaTime);
    }

    // Couroutines
    protected virtual IEnumerator ShootAnimationCouroutine()
    {
        // Allow the camera to go to the turret.
        yield return new WaitForSeconds(m_TimeDelayForCamToTurret);

        // Shoot Animation
        while (true)
        {
            RotateTurretTowardsTargetPosition(m_Target.transform.position);
            bool turretAimed = IsTurretAimingAtTarget(m_Target.transform.position);

            ElevateGunTowardsTargetPosition(m_Target.transform.position);
            bool gunAimed = IsGunAimingAtTarget(m_Target.transform.position);

            if (gunAimed && turretAimed)
            {
                AnimateFireGun(m_Target, m_Hit, m_ShootAnimationCompleteCallback + ShootAnimationComplete);
                // The callback has been passed on to the bullet.
                m_ShootAnimationCompleteCallback = null;
                break;
            }

            yield return null;
        }

        // CineMachineHandler
        while (!m_FinishShootAnimationFlag) { yield return null; }
        m_FinishShootAnimationFlag = false;

        yield return new WaitForSeconds(m_TimeDelayForCamBackToNormal);
        StopCinematicCamera();
    }

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
                RotateTankTowardsTargetPosition(m_Destination);
                yield return null;
                continue;
            }

            TranslateTankTowardsTargetPosition(m_Destination);
            yield return null;
        }
    }

    // Tailored Animations

    // Shoot
    public virtual void StartShootAnimation()
    {
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

    // Move
    public virtual void StartMoveAnimation()
    {
        if (m_CineMachineHandler)
        {
            m_CineMachineHandler.SetPanzerCinematicCamActive("Walk");
            m_CineMachineHandler.ActiveCamBase.LookAt = m_HullTransform;
            m_CineMachineHandler.ActiveCamBase.Follow = m_HullTransform;
        }
        m_MoveAnimationCoroutine = MoveAnimationCouroutine();
        StartCoroutine(m_MoveAnimationCoroutine);
    }

    public virtual void StopMoveAnimation()
    {
        StopCoroutine(m_MoveAnimationCoroutine);
        m_MoveAnimationCoroutine = null;
    }

    // Death
    public override void StartDeathAnimation()
    {
        // Regardless of the tag, show the death cinematic for both sides
        if (!m_CineMachineHandler)
        {
            m_CineMachineHandler = FindObjectOfType<CineMachineHandler>();
        }
        m_CineMachineHandler.SetPanzerCinematicCamActive("Die");
        m_CineMachineHandler.ActiveCamBase.LookAt = m_HullTransform;
        m_CineMachineHandler.ActiveCamBase.Follow = m_Turret.transform;
        StartCoroutine(StartDeathCameraCoroutine());
    }

    private IEnumerator StartDeathCameraCoroutine()
    {
        m_CineMachineHandler.DelayCinemachineSetActive(false, m_TimeDelayForDeathCam);
        yield return new WaitForSeconds(m_TimeDelayForDeathCam);
        yield return new WaitForSeconds(m_TimeDelayForCamBackToNormal);
        yield break;
    }

    // Once the animation is done, the callback is removed.
    public virtual void SetShootAnimationParameters(GameObject _target, bool _hit, Void_Void _callback)
    {
        Assert.IsTrue(_target != null);
        m_Target = _target;
        m_Hit = _hit;
        m_ShootAnimationCompleteCallback = _callback;
    }

    public virtual void SetMoveAnimationParameters(Vector3 _destination, Void_Void _callback)
    {
        m_Destination = _destination;
        m_MoveAnimationCompleteCallback = _callback;
    }

    protected virtual void ShootAnimationComplete()
    {
        m_FinishShootAnimationFlag = true;
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        MaxGunElevation = m_MaxGunElevation;
        MaxGunDepression = m_MaxGunDepression;
    }
#endif // UNITY_EDTIOR

}