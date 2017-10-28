using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PanzerAnimator : MonoBehaviour
{

    [SerializeField, Range(0, 90)] private float m_MaxGunElevation = 5.0f;
    [SerializeField, Range(0, 90)] private float m_MaxGunDepression = 20.0f;

    // Weapons
    [SerializeField] private GameObject m_Turret;
    [SerializeField] private GameObject m_Gun;

    // Tracks & Wheels
    [SerializeField] private GameObject m_LeftTracks;
    [SerializeField] private GameObject m_RightTracks;
    [SerializeField] private GameObject[] m_LeftWheels;
    [SerializeField] private GameObject[] m_RightWheels;
    // How fast the wheels should turn relative to the tracks.
    private float m_TracksToWheelsSpeedRatio = 800.0f;

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

    public void AnimateElevateGun(float _speed)
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
    }

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        MaxGunElevation = m_MaxGunElevation;
        MaxGunDepression = m_MaxGunDepression;
    }
#endif // UNITY_EDTIOR

}