using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PanzerAnimator : MonoBehaviour
{
    // Gun Elevation (This has been removed due to time constraints. It works but I don't want to calculate elavation and depression when doing shooting animation.)
    [SerializeField, Range(0, 90)] private float m_MaxGunElevation = 5.0f;
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

    // Shooting Animation
    [SerializeField] private GameObject m_BulletSpawn;
    [SerializeField] private GameObject m_Bullet;
    private Vector3 m_TargetPosition = new Vector3();
    private bool m_ShootAtTarget = false;
    private bool m_BulletExplodeOnContact = false;
    private float m_MaximumAngleToTarget = 1.0f; // The maximum angle difference between the target and where the turret is pointing.
    private float m_TurretRotationSpeed = 10.0f;

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
        textureOffset.x += _speed * Time.deltaTime;
        trackMaterial.mainTextureOffset = textureOffset;
        trackMaterial.SetTextureOffset("_BumpMap", textureOffset);

        foreach (GameObject wheel in m_LeftWheels)
        {
            wheel.transform.Rotate(new Vector3(Time.deltaTime * _speed * m_TracksToWheelsSpeedRatio, 0.0f, 0.0f));
        }
    }

    public void AnimateTracksRight(float _speed)
    {
        Material trackMaterial = m_RightTracks.GetComponent<MeshRenderer>().material;
        Vector2 textureOffset = trackMaterial.mainTextureOffset;
        textureOffset.x += _speed * Time.deltaTime;
        trackMaterial.mainTextureOffset = textureOffset;
        trackMaterial.SetTextureOffset("_BumpMap", textureOffset);

        foreach (GameObject wheel in m_RightWheels)
        {
            wheel.transform.Rotate(new Vector3(Time.deltaTime * _speed * m_TracksToWheelsSpeedRatio, 0.0f, 0.0f));
        }
    }

    public void AnimateTurretRotate(float _speed)
    {
        m_Turret.transform.Rotate(new Vector3(0.0f, Time.deltaTime * _speed, 0.0f));
    }

    public void AnimateMuzzleFlash()
    {
        for (int i = 0; i < m_MuzzleFlashes.Length; ++i)
        {
            m_MuzzleFlashes[i].SetActive(true);
            m_MuzzleFlashes[i].GetComponent<SimpleSpriteAnimation>().StartAnimation();
        }
    }

    public void AnimateFireBullet(bool _explodeOnContact)
    {
        GameObject bullet = GameObject.Instantiate(m_Bullet);
        bullet.transform.position = m_BulletSpawn.transform.position;
        bullet.transform.rotation = m_BulletSpawn.transform.rotation;
    }

    public void AnimateFireGun(bool _bulletExplodeOnContact)
    {
        AnimateMuzzleFlash();
        AnimateFireBullet(_bulletExplodeOnContact);
    }

    /*public void AnimateElevateGun(float _speed)
    {
        m_Gun.transform.Rotate(new Vector3(_speed * Time.deltaTime, 0.0f, 0.0f));

        // Limit our angle. The possible angle we can get ranges from -360 Degrees to 360 Degrees.
        // We need to convert it to the  from -180 Degress to 180 Degress range.
        float currentAngle = m_Gun.transform.localRotation.eulerAngles.x;
        while (currentAngle > 180.0f)
        {
            currentAngle -= 360.0f;
        }
        while (currentAngle < -180.0f)
        {
            currentAngle += 360.0f;
        }
        currentAngle = Mathf.Clamp(currentAngle, -m_MaxGunElevation, m_MaxGunDepression);
        Quaternion desiredRotation = new Quaternion();
        desiredRotation.eulerAngles = new Vector3(currentAngle, 0.0f, 0.0f);
        m_Gun.transform.localRotation = desiredRotation;
    }*/

    // _maximumAngle is in degrees not radians.
    public bool IsAimingAtTarget(Vector3 _targetPosition, float _maximumAngle /*This function returns true if the angle between the target direction and turret direction is less than this.*/)
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

        if (Vector3.Angle(directionToTarget, turretForward) > _maximumAngle)
        {
            return false;
        }

        return true;
    }

    // Tailored Animations
    public void ShootAtTargetAnimation(Vector3 _targetPosition, bool _bulletExplodeOnContact)
    {
        m_ShootAtTarget = true;
        m_TargetPosition = _targetPosition;
        m_BulletExplodeOnContact = _bulletExplodeOnContact;
    }

    // Private Function(s)
    private void RotateGunTowardsTargetPosition(Vector3 _targetPosition)
    {
        Vector3 directionToTarget = _targetPosition - m_Turret.transform.position;
        directionToTarget.y = 0.0f;

        Quaternion currentRotation = m_Turret.transform.rotation;
        Quaternion desiredRotation = Quaternion.LookRotation(directionToTarget);

        m_Turret.transform.rotation = Quaternion.Slerp(currentRotation, desiredRotation, Time.deltaTime * m_TurretRotationSpeed);
    }

    private void UpdateShootAnimation()
    {
        if (!m_ShootAtTarget)
        {
            return;
        }

        if (!IsAimingAtTarget(m_TargetPosition, m_MaximumAngleToTarget))
        {
            RotateGunTowardsTargetPosition(m_TargetPosition);
        }
        else
        {
            AnimateFireGun(m_BulletExplodeOnContact);
            m_ShootAtTarget = false;
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