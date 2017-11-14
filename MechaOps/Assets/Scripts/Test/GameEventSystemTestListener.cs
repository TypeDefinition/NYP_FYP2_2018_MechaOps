using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystemTestListener : MonoBehaviour {

	// Use this for initialization
	void Start()
    {
        GameEventSystem.GetInstance().SubscribeToEvent<int>("テスト", TestCallback);
	}
	
    void OnDestroy()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<int>("テスト", TestCallback);
    }

    public void TestCallback(int _counter)
    {
        Debug.Log(gameObject.name + " " + _counter);
    }

}