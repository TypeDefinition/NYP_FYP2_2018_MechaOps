using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class TileDisplay : MonoBehaviour
{
    // We have to swap the material rather than modify it in order to retain the ability to batch our objects.
    [SerializeField] private Material m_VisibleMaterial = null;
    [SerializeField] private Material m_NotVisibleMaterial = null;

    [SerializeField, HideInInspector] private bool m_OwnerInitialized = false;
    [SerializeField, HideInInspector] private Tile m_Owner = null;

    [SerializeField] private bool m_Known = false;
    [SerializeField] private bool m_Visible = false;

    public bool Known { get { return m_Known; } }
    public bool Visible { get { return m_Visible; } }

    private Void_BoolBool m_VisibilityCallback = null;
    public Void_BoolBool VisibilityCallback
    {
        get { return m_VisibilityCallback; }
        set { m_VisibilityCallback = value; }
    }

    public void InitOwner(Tile _owner)
    {
        // _radius should never be < 0.
        Assert.IsTrue(m_OwnerInitialized == false, MethodBase.GetCurrentMethod().Name + " - InitTile can only be called once per TileDisplay!");

        m_Owner = _owner;
        m_OwnerInitialized = true;
    }

    public Tile GetOwner()
    {
        return m_Owner;
    }

    public void SetVisibleState(bool _known, bool _visible)
    {
        if (GetOwner().GetTileId().Equals(new TileId(0, 0)))
        {
            int i = 0;
        }

        m_Known = _known;
        m_Visible = _visible;

        GetComponent<MeshRenderer>().enabled = _known;
        GetComponent<MeshRenderer>().material = (_visible ? m_VisibleMaterial : m_NotVisibleMaterial);

        if (m_VisibilityCallback != null)
        {
            m_VisibilityCallback(_known, _visible);
        }
    }

    private void Update()
    {
        GetComponent<MeshRenderer>().enabled = m_Known;
        GetComponent<MeshRenderer>().material = (m_Visible ? m_VisibleMaterial : m_NotVisibleMaterial);
    }
}