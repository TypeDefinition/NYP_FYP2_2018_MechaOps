using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

[DisallowMultipleComponent]
public class PanzerAnimator : MOAnimator
{
    // Gun Elevation (This has been removed due to time constraints. It works but I don't want to calculate elavation and depression when doing shooting animation.)
    [SerializeField, Range(0, 90)] private float m_MaxGunElevation = 80.0f;
    [SerializeField, Range(0, 90)] private float m_MaxGunDepression = 20.0f;

    // Weapons
    [SerializeField] private GameObject m_Turret;
    [SerializeField] private GameObject m_Gun;
    [SerializeField] private GameObject[] m_MuzzleFlashes;

    // Tracks & Wheels
    [SerializeField] private GameObject m_LeftTracks;
    [SerializeField] private GameObject m_RightTracks;
    [SerializeField] private GameObject[] m_LeftWheels;
    [SerializeField] private GameObject[] m_RightWheels;
    // How fast the wheels should turn relative to the tracks.
    private float m_TracksToWheelsSpeedRatio = 800.0f;

    // Bullet
    [SerializeField] private GameObject m_BulletSpawn;
    [SerializeField] private TankBullet m_BulletPrefab;

    [SerializeField, Tooltip("Hull of the panzer")] private Transform m_HullTransform;
    [SerializeField, Tooltip("CineMachine Handler")] private CineMachineHandler m_CineHandler;
    [SerializeField, Tooltip("Time taken for delay in the animation from camera to the turret during camera cinematics")]
    private float m_TimeDelayForCamToTurret = 3.0f;
    [SerializeField, Tooltip("Time taken for the delay before camera goes back to normal from cinematics")]
    private float m_TimeDelayForCamBackToNormal = 0.5f;
    [SerializeField, Tooltip("Time taken for the death cam animation")] private float m_TimeDelayForDeathCam = 2.0f;

    // Shooting Animation
    private GameObject m_Target = null;
    private bool m_Hit = false; // Is the shot a hit or miss?
    private float m_TurretRotationSpeed = 5.0f; // This speed is not in degrees. It's just by time cause I'm lazy.
    private float m_GunElevationSpeed = 60.0f; // This is the speed in degrees.
    private float m_AccuracyTolerance = 0.5f; // How small must the angle between where our gun is aiming and where the target is to be considered aimed.
    private bool m_FinishShootAnimFlag = false;
    private Void_Void m_ShootAnimationCompleteCallback = null;
    IEnumerator m_ShootAnimationCoroutine;

    // Moving Animation
    private Vector3 m_Destination = new Vector3();
    private float m_MovementSpeed = 10.0f;
    private float m_TankRotationSpeed = 3.0f;
    private float m_DistanceTolerance = 0.1f;
    private float m_RotationTolerance = 0.5f;
    private Void_Void m_MoveAnimationCompleteCallback = null;
    IEnumerator m_MoveAnimationCoroutine;

    public float MaxGunElevation
    {
        get { return m_MaxGunElevation; }
        set { m_MaxGunElevation = Mathf.Clamp(value, 0.0f, 90.0f); }
    }
    public float MaxGunDepression
    {
        get { return m_MaxGunDepression; }
        set { m_MaxGunDepression = Mathf.Clamp(value, 0.0f, 90.0f); }
    }

    private void Start()
    {
        switch (tag)
        {
            case "Player":
                // TODO: Only the player's units get to do the cinematic shots for now
                if (!m_CineHandler)
                    m_CineHandler = FindObjectOfType<CineMachineHandler>();
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void AnimateTracks(float _leftSpeed, float _rightSpeed)
    {
        AnimateTracksLeft(_leftSpeed);
        AnimateTracksRight(_rightSpeed);
    }

    public void AnimateTracksLeft(float _speed)
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

    public void AnimateTracksRight(float _speed)
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

    public void AnimateTurretRotate(float _angle)
    {
        m_Turret.transform.Rotate(new Vector3(0.0f, _angle, 0.0f));
    }

    public void AnimateMuzzleFlash()
    {
        for (int i = 0; i < m_MuzzleFlashes.Length; ++i)
        {
            m_MuzzleFlashes[i].SetActive(true);
            m_MuzzleFlashes[i].GetComponent<SimpleSpriteAnimation>().StartAnimation();
        }
    }

    public void AnimateFireBullet(GameObject _target, bool _explodeOnContact, Void_Void _callback)
    {
        Assert.IsTrue(_target != null);
        TankBullet bullet = GameObject.Instantiate(m_BulletPrefab.gameObject).GetComponent<TankBullet>();
        bullet.transform.position = m_BulletSpawn.transform.position;
        bullet.transform.LookAt(_target.transform.position);
        bullet.CompletionCallback = _callback;
        bullet.Target = _target;
        // TODO, maybe can show off the slow bullet animation
    }

    public void AnimateFireGun(GameObject _target, bool _bulletExplodeOnContact, Void_Void _callback)
    {
        AnimateMuzzleFlash();
        AnimateFireBullet(_target, _bulletExplodeOnContact, _callback);
    }

    public void AnimateElevateGun(float _angle)
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

    // Private Function(s)
    // _maximumAngle is in degrees not radians.
    private bool IsTurretAimingAtTarget(Vector3 _targetPosition)
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

    private bool IsGunAimingAtTarget(Vector3 _targetPosition)
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

        if (angleToTarget < -m_MaxGunElevation)
        {
            //Debug.Log("Angle To Target < -Max Gun Elevation");
            return (Mathf.Abs(currentAngle - m_MaxGunElevation) <= m_AccuracyTolerance);
        }

        if (angleToTarget > m_MaxGunDepression)
        {
            //Debug.Log("Angle To Target > Max Gun Depression");
            return (Mathf.Abs(currentAngle - m_MaxGunDepression) <= m_AccuracyTolerance);
        }

        return false;
    }

    private bool IsTankAtDestination(Vector3 _destination)
    {
        _destination.y = gameObject.transform.position.y;
        return (_destination - gameObject.transform.position).sqrMagnitude < m_DistanceTolerance * m_DistanceTolerance;
    }

    private bool IsTankFacingDestination(Vector3 _destination)
    {
        Vector3 directionToDestination = _destination - m_Turret.transform.position;
        directionToDestination.y = 0.0f;
        Vector3 forward = transform.forward;
        forward.y = 0.0f;

        // Check if the turret is facing the target.
        if (Vector3.Dot(directionToDestination, forward) <= 0.0f)
        {
            return false;
        }

        if (Vector3.Angle(directionToDestination, forward) > m_RotationTolerance)
        {
            return false;
        }

        return true;
    }

    private float ConvertAngle(float _angle)
    {
        // Convert an angle to be between -180 and 180.
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

    private void RotateTurretTowardsTargetPosition(Vector3 _targetPosition)
    {
        Vector3 directionToTarget = _targetPosition - m_Turret.transform.position;
        directionToTarget.y = 0.0f;

        Quaternion currentRotation = m_Turret.transform.rotation;
        Quaternion desiredRotation = Quaternion.LookRotation(directionToTarget);

        m_Turret.transform.rotation = Quaternion.Slerp(currentRotation, desiredRotation, Time.deltaTime * m_TurretRotationSpeed);
    }

    private void ElevateGunTowardsTargetPosition(Vector3 _targetPosition)
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

    private void RotateTankTowardsTargetPosition(Vector3 _targetPosition)
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

    private void TranslateTankTowardsTargetPosition(Vector3 _targetPosition)
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
    private IEnumerator ShootAnimationCouroutine()
    {
        WaitForSeconds zeWait = new WaitForSeconds(m_TimeDelayForCamToTurret);
        yield return zeWait;
        while (true)
        {
            RotateTurretTowardsTargetPosition(m_Target.transform.position);
            bool turretAimed = IsTurretAimingAtTarget(m_Target.transform.position);

            ElevateGunTowardsTargetPosition(m_Target.transform.position);
            bool gunAimed = IsGunAimingAtTarget(m_Target.transform.position);

            if (gunAimed && turretAimed)
            {
                AnimateFireGun(m_Target, m_Hit, m_ShootAnimationCompleteCallback);
                // Once the animation is done, the callback is removed.
                m_ShootAnimationCompleteCallback = null;
                break;
            }

            yield return null;
        }
        while (!m_FinishShootAnimFlag)
            yield return null;
        m_FinishShootAnimFlag = false;
        if (m_CineHandler)
        {
            yield return new WaitForSeconds(m_TimeDelayForCamBackToNormal);
            m_CineHandler.SetCineBrain(false);
        }
    }

    private IEnumerator MoveAnimationCouroutine()
    {
        while (true)
        {
            bool tankFacingDestination = IsTankFacingDestination(m_Destination);
            if (!tankFacingDestination)
            {
                RotateTankTowardsTargetPosition(m_Destination);
                yield return null;
                continue;
            }

            bool tankAtDestination = IsTankAtDestination(m_Destination);
            if (!tankAtDestination)
            {
                TranslateTankTowardsTargetPosition(m_Destination);
                yield return null;
                continue;
            }

            if (tankFacingDestination && tankAtDestination)
            {
                if (m_MoveAnimationCompleteCallback != null)
                {
                    m_MoveAnimationCompleteCallback();
                }
                break;
            }

            yield return null;
        }
    }

    // Tailored Animations
    public void StartShootAnimation()
    {
        if (m_CineHandler)
        {
            int zeRandNum = Random.Range(1, 3);
            // unfortunately, need to hardcode the cinmatic abit
            switch (zeRandNum)
            {
                case 2:
                    // Number 2 refers to Following the target while aiming the cinematic camera at the Turret
                    m_CineHandler.CineStateCam.Follow = m_Target.transform;
                    break;
                default:
                    m_CineHandler.CineStateCam.Follow = m_Turret.transform;
                    break;
            }
            m_CineHandler.CineStateCam.LookAt = m_Gun.transform;
            // and then randomize between 1st and 2nd attack camera cinematics
            m_CineHandler.TriggerEventParam("Attack" + zeRandNum);
        }
        m_ShootAnimationCoroutine = ShootAnimationCouroutine();
        StartCoroutine(m_ShootAnimationCoroutine);
    }

    public void StopShootAnimation()
    {
        StopCoroutine(m_ShootAnimationCoroutine);
    }

    public void StartMoveAnimation()
    {
        if (m_CineHandler)
        {
            m_CineHandler.CineStateCam.LookAt = m_HullTransform;
            m_CineHandler.CineStateCam.Follow = m_HullTransform;
            m_CineHandler.TriggerEventParam("Walk");
        }
        m_MoveAnimationCoroutine = MoveAnimationCouroutine();
        StartCoroutine(m_MoveAnimationCoroutine);
    }

    public void StopMoveAnimation()
    {
        StopCoroutine(m_MoveAnimationCoroutine);
    }

    // Once the animation is done, the callback is removed.
    public void SetShootAnimationParameters(GameObject _target, bool _hit, Void_Void _callback)
    {
        Assert.IsTrue(_target != null);
        m_Target = _target;
        m_Hit = _hit;
        if (_callback != null)
        {
            m_ShootAnimationCompleteCallback = _callback;
        }
        m_ShootAnimationCompleteCallback += ShootAnimationComplete;
    }

    public void SetMoveAnimationParameters(Vector3 _destination, Void_Void _callback)
    {
        m_Destination = _destination;
        if (_callback != null)
        {
            m_MoveAnimationCompleteCallback = _callback;
        }
    }

    protected void ShootAnimationComplete()
    {
        m_FinishShootAnimFlag = true;
    }

    /// <summary>
    /// Stopped the cinematic camera if there is any!
    /// </summary>
    public void StopCinematicCamera()
    {
        if (m_CineHandler)
        {
            m_CineHandler.SetCineBrain(false);
        }
    }

    /// <summary>
    /// For now, starts the death camera cinematics
    /// </summary>
    public void StartDeathAnimation()
    {
        // Regardless of the tag, show the death cinematic for both sides
        if (!m_CineHandler)
        {
            m_CineHandler = FindObjectOfType<CineMachineHandler>();
        }
        m_CineHandler.TriggerEventParam("Die");
        StartCoroutine(StartDeathCameraCoroutine());
    }

    private IEnumerator StartDeathCameraCoroutine()
    {
        yield return new WaitForSeconds(m_TimeDelayForDeathCam);
        m_CineHandler.SetCineBrain(false);
        yield return new WaitForSeconds(m_TimeDelayForCamBackToNormal);
        yield break;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        //MaxGunElevation = m_MaxGunElevation;
        //MaxGunDepression = m_MaxGunDepression;
    }
#endif // UNITY_EDTIOR

}