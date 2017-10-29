using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class HazardDisplay : MonoBehaviour {

    [SerializeField] private bool m_OwnerInitialized = false;
    [SerializeField] private Hazard m_Owner = null;
    
    public Hazard Owner
    {
        get { return m_Owner; }
    }

    public void InitOwner(Hazard _owner)
    {
        Assert.IsFalse(m_OwnerInitialized, MethodBase.GetCurrentMethod().Name + " - This function can only be called once per HazardDisplay!");

        m_Owner = _owner;
        m_OwnerInitialized = true;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
