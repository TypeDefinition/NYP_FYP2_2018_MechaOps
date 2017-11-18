using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class TileDisplay : MonoBehaviour {

    [SerializeField, HideInInspector] private bool m_OwnerInitialized = false;
    [SerializeField, HideInInspector] private Tile m_Owner = null;

    public void InitOwner(Tile _owner) {
        // _radius should never be < 0.
        Assert.IsTrue(m_OwnerInitialized == false, MethodBase.GetCurrentMethod().Name + " - InitTile can only be called once per TileDisplay!");

        m_Owner = _owner;
        m_OwnerInitialized = true;
    }

    public Tile GetOwner() {
        return m_Owner;
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

}
