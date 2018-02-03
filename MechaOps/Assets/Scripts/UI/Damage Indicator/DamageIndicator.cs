using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageIndicator : MonoBehaviour
{
    // Serialised Fields
    [SerializeField] protected float m_PositionZ = 0.0f;
    [SerializeField] protected Vector3 m_TargetWorldPositionOffset = new Vector3(0.0f, 2.5f, 0.0f);
    [SerializeField] protected Vector3 m_Velocity = new Vector3(0.0f, 2.0f, 0.0f);
    [SerializeField] protected Color m_HitColor = Color.yellow;
    [SerializeField] protected Color m_MissColor = Color.grey;
    [SerializeField] protected TextMeshProUGUI m_DamageValueText = null;

    // Non-Serialised Fields
    GameObject m_Target;
    protected bool m_Hit = false;
    protected int m_DamageValue = 0;

    public GameObject Target
    {
        get { return m_Target; }
        set { m_Target = value; }
    }

    public int DamageValue
    {
        get { return m_DamageValue; }
        set { m_DamageValue = value; }
    }

    public bool Hit
    {
        get { return m_Hit; }
        set { m_Hit = value; }
    }

    protected virtual void LateUpdate()
    {
        if (m_Target != null)
        {
            // Update Position
            Camera gameCamera = GameSystemsDirectory.GetSceneInstance().GetGameCamera();
            Vector3 screenPoint = gameCamera.WorldToScreenPoint(m_Target.gameObject.transform.position + m_TargetWorldPositionOffset);
            screenPoint.z = m_PositionZ;
            transform.position = screenPoint;
            m_TargetWorldPositionOffset += m_Velocity * Time.deltaTime;

            if (m_Hit)
            {
                m_DamageValueText.color = m_HitColor;
                m_DamageValueText.text = m_DamageValue.ToString();
            }
            else
            {
                m_DamageValueText.color = m_MissColor;
                m_DamageValueText.text = "MISS";
            }
        }
    }
}