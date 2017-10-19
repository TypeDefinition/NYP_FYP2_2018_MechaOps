using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class TileDisplay : MonoBehaviour {

    private Tile m_Tile = null;

    public void InitTile(Tile _tile) {
        // _radius should never be < 0.
        Assert.IsTrue(m_Tile == null, MethodBase.GetCurrentMethod().Name + " - InitTile can only be called once per TileDisplay!");

        m_Tile = _tile;
    }

    public Tile GetTile() {
        return m_Tile;
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

}
