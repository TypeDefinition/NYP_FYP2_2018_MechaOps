using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class GameCameraMovement : MonoBehaviour
{
    private const int m_NumDirections = 6;
    private int m_CurrentDirection = 0;

    [SerializeField] private Vector3 m_ViewAngle = new Vector3(30.0f, 90.0f, 0.0f);
    [SerializeField] private float m_RotationSpeed = 5.0f;

    [SerializeField] private Vector3 m_DefaultMovementForward = new Vector3(1.0f, 0.0f, 0.0f);
    private Vector3 m_MovementForward = new Vector3(1.0f, 0.0f, 0.0f);

    [SerializeField] private float m_MinHeight = 3.0f;
    [SerializeField] private float m_MaxHeight = 15.0f;

    [SerializeField] TileSystem m_TileSystem = null;
    private Vector3 m_CentrePosition = new Vector3();
    private float m_MaxDistanceFromCentre = 50.0f;

    [SerializeField] private float m_MinFOV = 10.0f;
    [SerializeField] private float m_MaxFOV = 80.0f;

    public float MinHeight
    {
        get { return m_MinHeight; }
        set { m_MinHeight = Mathf.Min(m_MaxHeight, value); }
    }

    public float MaxHeight
    {
        get { return m_MaxHeight; }
        set { m_MaxHeight = Mathf.Max(m_MinHeight, value); }
    }

    public float RotationSpeed
    {
        get { return m_RotationSpeed; }
        set { m_RotationSpeed = Mathf.Max(0.0f, value); }
    }

    public float MaxDistanceFromCentre
    {
        get { return m_MaxDistanceFromCentre; }
        set { m_MaxDistanceFromCentre = Mathf.Max(0.0f, value); }
    }

    public float MinFOV
    {
        get { return m_MinFOV; }
        set { m_MinFOV = Mathf.Clamp(value, 1.0f, m_MaxFOV); }
    }

    public float MaxFOV
    {
        get { return m_MaxFOV; }
        set { m_MaxFOV = Mathf.Clamp(value, m_MinFOV, 179.0f); }
    }

    // 前へ行く
    public void MoveForward(float _moveValue)
    {
        gameObject.transform.position += m_MovementForward * _moveValue;
    }

    // 後ろへ行く
    public void MoveBackwards(float _moveValue)
    {
        gameObject.transform.position -= m_MovementForward * _moveValue;
    }

    // 上へ行く
    public void MoveUp(float _moveValue)
    {
        gameObject.transform.position += Vector3.up * _moveValue;
    }

    // 下へ行く
    public void MoveDown(float _moveValue)
    {
        gameObject.transform.position -= Vector3.up * _moveValue;
    }

    // 左へ行く
    public void MoveLeft(float _moveValue)
    {
        Vector3 direction = Quaternion.Euler(0.0f, 90.0f, 0.0f) * m_MovementForward;
        gameObject.transform.position -= direction * _moveValue;
    }

    // 右へ行く
    public void MoveRight(float _moveValue)
    {
        Vector3 direction = Quaternion.Euler(0.0f, 90.0f, 0.0f) * m_MovementForward;
        gameObject.transform.position += direction * _moveValue;
    }

    // 左へ曲がる
    public void RotateLeft()
    {
        Assert.IsTrue(m_CurrentDirection >= 0 && m_CurrentDirection < m_NumDirections);

        if (m_CurrentDirection == 0)
        {
            m_CurrentDirection = m_NumDirections - 1;
        }
        else
        {
            --m_CurrentDirection;
        }

        UpdateMovementForward();
    }

    // 右へ曲がる
    public void RotateRight()
    {
        Assert.IsTrue(m_CurrentDirection >= 0 && m_CurrentDirection < m_NumDirections);

        m_CurrentDirection = (m_CurrentDirection + 1) % m_NumDirections;
        UpdateMovementForward();
    }

    public void Zoom(float _zoomValue)
    {
        Camera camera = gameObject.GetComponent<Camera>();
        Assert.IsTrue(camera != null);

        camera.fieldOfView = Mathf.Clamp(camera.fieldOfView + _zoomValue, m_MinFOV, m_MaxFOV);
    }

    // 初期化(しょきか)する時、このを使う
    private void Start()
    {
        Assert.IsTrue(m_TileSystem != null);
        // Radius by distance, not tiles. This formula is figured out by looking at the tiles from the top.
        m_MaxDistanceFromCentre =
            (m_TileSystem.TileDiameter * 0.5f) + // The first tile.
            (m_TileSystem.MapRadius * m_TileSystem.TileDiameter * 0.75f); // Every subsequent tile.
        m_CentrePosition = m_TileSystem.transform.position;

        LimitHeight(m_MinHeight, m_MaxHeight);
        UpdateViewDirection();
        UpdateMovementForward();
    }

    // 更新する時、このを使う
    private void Update()
    {
        LimitHeight(m_MinHeight, m_MaxHeight);
        UpdateViewDirection();
        LimitPosition();
	}

    private void LimitHeight(float _min, float _max)
    {
        Assert.IsTrue(_min <= _max, MethodBase.GetCurrentMethod().Name + " - _max MUST be greater or equal to _min.");

        Vector3 cameraPosition = gameObject.transform.position;
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, _min, _max);
        gameObject.transform.position = cameraPosition;
    }

    private void LimitPosition()
    {
        for (int i = 0; i < m_NumDirections; ++i)
        {
            Vector3 directionToCamera = gameObject.transform.position - m_CentrePosition;

            Vector3 directionToLimit = m_DefaultMovementForward.normalized * m_MaxDistanceFromCentre;
            float angleY = 360.0f / (float)m_NumDirections * i;
            directionToLimit = Quaternion.Euler(0.0f, angleY, 0.0f) * directionToLimit;

            if (Vector3.Dot(directionToCamera, directionToLimit) < 0.0f)
            {
                continue;
            }

            Vector3 projection = Vector3.Project(directionToCamera, directionToLimit);

            if (projection.sqrMagnitude > directionToLimit.sqrMagnitude)
            {
                gameObject.transform.position = gameObject.transform.position - (projection - directionToLimit);
            }
        }
    }

    private void UpdateViewDirection()
    {
        float angleY = 360.0f / (float)m_NumDirections * m_CurrentDirection;
        Quaternion targetRotation = Quaternion.Euler(m_ViewAngle.x, m_ViewAngle.y + angleY, m_ViewAngle.z);
        gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);
    }

    private void UpdateMovementForward()
    {
        float angle = 360.0f / (float)m_NumDirections * m_CurrentDirection;
        m_MovementForward = Quaternion.Euler(0.0f, angle, 0.0f) * m_DefaultMovementForward;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        MaxHeight = m_MaxHeight;
        MinHeight = m_MinHeight;
        RotationSpeed = m_RotationSpeed;
        MaxDistanceFromCentre = m_MaxDistanceFromCentre;
        MinFOV = m_MinFOV;
        MaxFOV = m_MaxFOV;
    }
#endif

}