using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard_Fire : Hazard {

    private int m_Damage = 1;

    public int Damage
    {
        get { return m_Damage; }
        set { m_Damage = Mathf.Max(0, value); }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = collision.gameObject;
        if (collidedObject.activeSelf == false)
        {
            return;
        }

        UnitStats unitStats = gameObject.GetComponent<UnitStats>();
        if (unitStats != null)
        {
            unitStats.CurrentHealthPoints -= m_Damage;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Damage = m_Damage;
    }
#endif // UNITY_EDITOR

}
