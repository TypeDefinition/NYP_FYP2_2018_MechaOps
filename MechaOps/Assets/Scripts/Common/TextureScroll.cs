using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScroll : MonoBehaviour
{
    public float m_UScrollSpeed = 0.0f;
    public float m_VScrollSpeed = 0.0f;
    private MeshRenderer m_MeshRenderer;

    void Start()
    {
        m_MeshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

	// Update is called once per frame
	void Update ()
    {
		if (m_MeshRenderer != null)
        {
            Vector2 textureOffset = new Vector2(m_MeshRenderer.material.mainTextureOffset.x, m_MeshRenderer.material.mainTextureOffset.y);
            textureOffset.x += m_UScrollSpeed * Time.deltaTime;
            textureOffset.y += m_VScrollSpeed * Time.deltaTime;
            m_MeshRenderer.material.mainTextureOffset = textureOffset;
        }
	}
}