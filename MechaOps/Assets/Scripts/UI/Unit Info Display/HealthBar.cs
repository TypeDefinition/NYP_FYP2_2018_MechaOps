using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public class HealthBar : MonoBehaviour
{
    [SerializeField] private int m_MaxColumnSize = 5;
    [Tooltip("If true, Row Distance and Column Distance are ignored, and the distance between each health bar icon will be based on the icon's width and height.")]
    [SerializeField] private bool m_UseIconSizeAsDistanceBetweenIcons = true;
    [SerializeField] private float m_RowDistance = 32;
    [SerializeField] private float m_ColumnDistance = 64;
    [SerializeField] private Vector3 m_PositionOffset = new Vector3(0.0f, 0.0f);

    [SerializeField] private int m_MaxHealthPoints = 5;
    [SerializeField] private int m_CurrentHealthPoints = 5;
    [SerializeField] private Image m_HealthBarIcon = null;
    [SerializeField] private Material m_CurrentHealthMaterial = null;
    [SerializeField] private Material m_LostHealthMaterial = null;

    private List<GameObject> m_HealthBars = new List<GameObject>();
    private bool m_ValuesChanged = true; // Dirty Flag

    public int MaxColumnSize
    {
        get { return m_MaxColumnSize; }
        set { m_MaxColumnSize = Mathf.Max(1, value); m_ValuesChanged = true; }
    }

    public bool UseIconSizeAsDistanceBetweenIcons
    {
        get { return m_UseIconSizeAsDistanceBetweenIcons; }
        set { m_UseIconSizeAsDistanceBetweenIcons = value; m_ValuesChanged = true; }
    }

    public float RowDistance
    {
        get { return m_RowDistance; }
        set { m_RowDistance = Mathf.Max(0.0f, value); m_ValuesChanged = true; }
    }
    public float ColumnDistance
    {
        get { return m_ColumnDistance; }
        set { m_ColumnDistance = Mathf.Max(0.0f, value); m_ValuesChanged = true; }
    }

    public int MaxHealthPoints
    {
        get { return m_MaxHealthPoints; }
        set
        {
            m_MaxHealthPoints = Mathf.Max(0, value);
            m_CurrentHealthPoints = Mathf.Min(m_CurrentHealthPoints, m_MaxHealthPoints);
            m_ValuesChanged = true;
        }
    }

    public int CurrentHealthPoints
    {
        get { return m_CurrentHealthPoints; }
        set { m_CurrentHealthPoints = Mathf.Clamp(value, 0, m_MaxHealthPoints); m_ValuesChanged = true; }
    }

    private void GenerateHealthBars()
    {
        ClearHealthBars();

        Assert.IsTrue(m_HealthBarIcon != null, MethodBase.GetCurrentMethod().Name + " - m_HealthBar cannot be null!");

        for (int i = 0; i < m_MaxHealthPoints; ++i)
        {
            GameObject healthBar = GameObject.Instantiate(m_HealthBarIcon.gameObject, transform);
            healthBar.GetComponent<Image>().material = (i < m_CurrentHealthPoints) ? m_CurrentHealthMaterial : m_LostHealthMaterial;

            // Set the position of the health bar.
            int row = i / m_MaxColumnSize;
            int column = i % m_MaxColumnSize;

            if (m_UseIconSizeAsDistanceBetweenIcons)
            {
                healthBar.transform.localPosition = m_PositionOffset + new Vector3(healthBar.GetComponent<RectTransform>().sizeDelta.x * column, healthBar.GetComponent<RectTransform>().sizeDelta.y * row, 0.0f);
            }
            else
            {
                healthBar.transform.localPosition = m_PositionOffset + new Vector3(m_ColumnDistance * column, m_RowDistance * row, 0.0f);
            }
        }
    }

    private void ClearHealthBars()
    {
        for (int i = 0; i < m_HealthBars.Count; ++i)
        {
            if (m_HealthBars[i] == null) { continue; }
            GameObject.Destroy(m_HealthBars[i]);
        }

        m_HealthBars.Clear();
    }

    private void Awake()
    {
        Assert.IsTrue(m_HealthBarIcon != null, MethodBase.GetCurrentMethod().Name + " - m_HealthBar cannot be null!");
    }

    private void Start()
    {
        GenerateHealthBars();
    }

    private void Update()
    {
        if (m_ValuesChanged)
        {
            GenerateHealthBars();
            m_ValuesChanged = false;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        MaxColumnSize = m_MaxColumnSize;
        UseIconSizeAsDistanceBetweenIcons = m_UseIconSizeAsDistanceBetweenIcons;
        RowDistance = m_RowDistance;
        ColumnDistance = m_ColumnDistance;

        MaxHealthPoints = m_MaxHealthPoints;
        CurrentHealthPoints = m_CurrentHealthPoints;
    }
#endif // UNITY_EDITOR
}