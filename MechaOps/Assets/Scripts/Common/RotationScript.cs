using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    public enum RotationAxis
    {
        X,
        Y,
        Z,
    }

    [SerializeField, Tooltip("The rotation speed in degrees/second.")]
    private float m_RotationSpeed = 60.0f;
    [SerializeField, Tooltip("The axis of rotation")]
    private RotationAxis m_RotationAxis = RotationAxis.X;

    public float GetRotationSpeed() { return m_RotationSpeed; }

    public RotationAxis GetRotationAxis() { return m_RotationAxis; }

	void Update ()
    {
        switch (m_RotationAxis)
        {
            case RotationAxis.X:
                transform.Rotate(m_RotationSpeed * Time.deltaTime, 0.0f, 0.0f);
                break;
            case RotationAxis.Y:
                transform.Rotate(0.0f, m_RotationSpeed * Time.deltaTime, 0.0f);
                break;
            case RotationAxis.Z:
                transform.Rotate(0.0f, 0.0f, m_RotationSpeed * Time.deltaTime);
                break;
        }
	}
}