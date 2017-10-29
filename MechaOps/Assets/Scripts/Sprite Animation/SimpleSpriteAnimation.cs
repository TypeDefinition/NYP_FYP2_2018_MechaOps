using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Assertions;
using UnityEngine;

public class SimpleSpriteAnimation : MonoBehaviour
{

    [SerializeField] private int m_Rows = 8;
    [SerializeField] private int m_Columns = 8;
    [SerializeField] private float m_AnimationDuration = 1.0f;
    [SerializeField] private bool m_Loop = true;

    private int m_NumFrames = 0;
    private float m_CurrentTime = 0.0f;
    private Material m_SpriteMaterial = null;

    public int Rows
    {
        get { return m_Rows; }
        set { m_Rows = Mathf.Max(0, value); }
    }

    public int Columns
    {
        get { return m_Columns; }
        set { m_Columns = Mathf.Max(0, value); }
    }

    public float AnimationDuration
    {
        get { return m_AnimationDuration; }
        set { m_AnimationDuration = Mathf.Max(0.01f, value); }
    }

    public bool Loop
    {
        get { return m_Loop; }
        set { m_Loop = value; }
    }

	// Use this for initialization
	void Start ()
    {
        m_NumFrames = m_Rows * m_Columns;
        m_NumFrames = 0;
        m_CurrentTime = 0.0f;

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        Assert.AreNotEqual(meshRenderer, null, MethodBase.GetCurrentMethod().Name + " - SimpleSpriteAnimation cannot work without a MeshRenderer!");
        m_SpriteMaterial = gameObject.GetComponent<MeshRenderer>().material;
        Assert.AreNotEqual(m_SpriteMaterial, null, MethodBase.GetCurrentMethod().Name + " - SimpleSpriteAnimation cannot work without a material!");
    }
	
	// Update is called once per frame
	void Update ()
    {
        while (m_Loop && m_CurrentTime > m_AnimationDuration)
        {
            m_CurrentTime -= m_AnimationDuration;
        }

        if (m_CurrentTime > m_AnimationDuration)
        {
            return;
        }

        int numFrames = m_Rows * m_Columns;
        float frameDuration = m_AnimationDuration / (float)numFrames;
        int currentFrame = (int)(m_CurrentTime / frameDuration);
        int currentRow = currentFrame / m_Columns;
        int currentColumn = currentFrame % m_Columns;

        m_SpriteMaterial.mainTextureOffset = new Vector2((float)currentColumn / (float)m_Columns, (float)(m_Rows - 1 - currentRow) / (float)m_Rows);
        m_SpriteMaterial.mainTextureScale = new Vector2(1.0f / (float)m_Columns, 1.0f / (float)m_Rows);

        m_CurrentTime += Time.deltaTime;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Columns = m_Columns;
        Rows = m_Rows;
        AnimationDuration = m_AnimationDuration;
    }
#endif

}