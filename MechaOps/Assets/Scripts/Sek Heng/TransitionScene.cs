using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// TODO: improve this to become transition scene manager / remove this
/// </summary>
public class TransitionScene : MonoBehaviour {
    /// <summary>
    /// To transition to another scene
    /// </summary>
    /// <param name="_name">Name of the scene</param>
    public void TransitScene(string _name)
    {
        SceneManager.LoadScene(_name);
    }
}
