using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PanzerAnimator : MonoBehaviour
{
    public delegate void Void_Void();

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
    [SerializeField] private GameObject m_BulletPrefab;

    // Shooting Animation
    private Vector3 m_TargetPosition = new Vector3();
    private bool m_Hit = false; // Is the shot a hit or miss?
    private float m_TurretRotationSpeed = 5.0f; // This speed is not in degrees. It's just by time cause I'm lazy.
    private float m_GunElevationSpeed = 60.0f; // This is the speed in degrees.
    private float m_AccuracyTolerance = 0.5f; // How small must the angle between where our gun is aiming and where the target is to be considered aimed.
    private bool m_ShootingAnimationRunning = false;
    private Void_Void m_ShootingAnimationCompleteCallback = null;

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

    public void AnimateFireBullet(Vector3 _targetPosition, bool _explodeOnContact)
    {
        GameObject bullet = GameObject.Instantiate(m_BulletPrefab);
        bullet.transform.position = m_BulletSpawn.transform.position;

        bullet.transform.LookAt(_targetPosition);
        //bullet.transform.rotation = m_BulletSpawn.transform.rotation;
    }

    public void AnimateFireGun(Vector3 _targetPosition, bool _bulletExplodeOnContact)
    {
        AnimateMuzzleFlash();
        AnimateFireBullet(_targetPosition, _bulletExplodeOnContact);
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

    // _maximumAngle is in degrees not radians.
    public bool IsTurretAimingAtTarget(Vector3 _targetPosition)
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

    public bool IsGunAimingAtTarget(Vector3 _targetPosition)
    {
        Vector3 directionToTarget = _targetPosition - m_Gun.transform.position;
        directionToTarget.Normalize();

        float angleToTarget = -Mathf.Asin(directionToTarget.y) * Mathf.Rad2Deg; // Negate this because a position rotation is downwards.
        float currentAngle = ConvertAngle(m_Gun.transform.localRotation.eulerAngles.x);
        float angleDifference = angleToTarget - currentAngle;

        //Debug.Log("Angle To Target: " + angleToTarget);
        //Debug.Log("Current Angle: " + currentAngle);
        //Debug.Log("Angle Difference: " + angleDifference);

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

        //Debug.Log("Angle Difference Too Large");
        return false;
    }

    // Private Function(s)
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

    private void UpdateShootAnimation()
    {
        if (!m_ShootingAnimationRunning)
        {
            return;
        }

        bool turretAimed = IsTurretAimingAtTarget(m_TargetPosition);
        bool gunAimed = IsGunAimingAtTarget(m_TargetPosition);

        if (!turretAimed)
        {
            //Debug.Log("!TurretAimed");
            RotateTurretTowardsTargetPosition(m_TargetPosition);
        }

        if (!gunAimed)
        {
            //Debug.Log("!GunAimed");
            ElevateGunTowardsTargetPosition(m_TargetPosition);
        }
        
        if (gunAimed && turretAimed)
        {
            AnimateFireGun(m_TargetPosition, m_Hit);
            m_ShootingAnimationRunning = false;
            if (m_ShootingAnimationCompleteCallback != null)
            {
                m_ShootingAnimationCompleteCallback();
                m_ShootingAnimationCompleteCallback = null;
            }
        }
    }

    // Tailored Animations
    public void StartShootAnimation()
    {
        m_ShootingAnimationRunning = true;
    }

    public void StopShootAnimation()
    {
        m_ShootingAnimationRunning = false;
    }

    public void SetShootAnimationParameters(Vector3 _targetPosition, bool _hit, Void_Void _callback)
    {
        m_TargetPosition = _targetPosition;
        m_Hit = _hit;
        if (_callback != null)
        {
            m_ShootingAnimationCompleteCallback += _callback;
        }
    }

    // Use this for initialization
    private void Start () {}

    // Update is called once per frame
    private void Update ()
    {
        UpdateShootAnimation();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        //MaxGunElevation = m_MaxGunElevation;
        //MaxGunDepression = m_MaxGunDepression;
    }
#endif // UNITY_EDTIOR

}