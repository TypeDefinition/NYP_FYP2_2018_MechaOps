using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslationScript : MonoBehaviour
{
    public enum TranslationAxis
    {
        X,
        Y,
        Z,
    }

    [SerializeField]
    private bool m_Translate = false;
    [SerializeField, Tooltip("The rotation speed in degrees/second.")]
    private float m_TranslationSpeed = 50.0f;
    [SerializeField, Tooltip("The axis of rotation")]
    private TranslationAxis m_TranslationAxis = TranslationAxis.X;

    public void StartTranslation() { m_Translate = true; }

    public void StopTranslation() { m_Translate = false; }

    public float GetTranslationSpeed() { return m_TranslationSpeed; }

    public TranslationAxis GetTranslationAxis() { return m_TranslationAxis; }

    private void Update()
    {
        if (!m_Translate) { return; }

        switch (m_TranslationAxis)
        {
            case TranslationAxis.X:
                transform.Translate(m_TranslationSpeed * Time.deltaTime, 0.0f, 0.0f);
                break;
            case TranslationAxis.Y:
                transform.Translate(0.0f, m_TranslationSpeed * Time.deltaTime, 0.0f);
                break;
            case TranslationAxis.Z:
                transform.Translate(0.0f, 0.0f, m_TranslationSpeed * Time.deltaTime);
                break;
        }
    }
}