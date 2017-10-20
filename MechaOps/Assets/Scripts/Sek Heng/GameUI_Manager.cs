using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all of the UI for the gameplay!
/// </summary>
public class GameUI_Manager : MonoBehaviour {
    [Tooltip("An array of all the UI manager that needs to hold and set active!")]
    public GameObject[] AllGameNeededUI;

    // The dictionary so that accessing gameobject will be faster
    protected Dictionary<string, GameObject> m_TagGO_dict = new Dictionary<string, GameObject>();

    // A quick access of singleton
    public static GameUI_Manager Instance
    {
        get; protected set;
    }

    private void Awake()
    {
        // Setting up of singleton!
        if (Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Use this for initialization
    void Start () {
		foreach (GameObject zeUI_go in AllGameNeededUI)
        {
            m_TagGO_dict.Add(zeUI_go.tag, zeUI_go);
        }
	}

    /// <summary>
    /// Set the gameobject with the specific tag to be active
    /// </summary>
    /// <param name="tagName">The tag name of the gameobj</param>
    /// <returns>return false if no gameobj is found. Return true if success.</returns>
    public bool SetTheGameObjTagActive(string tagName)
    {
        GameObject theGameObjUI;
        if (m_TagGO_dict.TryGetValue(tagName, out theGameObjUI))
        {
            theGameObjUI.SetActive(true);
            return true;
        }
        return false;
    }
}
