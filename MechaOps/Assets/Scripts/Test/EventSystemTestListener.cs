using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystemTestListener : MonoBehaviour {

	// Use this for initialization
	void Start()
    {
        EventSystem.GetInstance().SubscribeToEvent<int>("テスト", TestCallback);
	}
	
    void OnDestroy()
    {
        EventSystem.GetInstance().UnsubscribeFromEvent<int>("テスト", TestCallback);
    }

    public void TestCallback(int _counter)
    {
        Debug.Log(gameObject.name + _counter);
    }

}