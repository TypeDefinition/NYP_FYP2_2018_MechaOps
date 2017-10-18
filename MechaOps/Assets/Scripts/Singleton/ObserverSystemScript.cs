using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// It is to follow the observer pattern. Something like subscribers!
/// Do always remember to unsubscribe at "Void OnDisable()" whenever you subscribe from another object!
/// </summary>
public class ObserverSystemScript : MonoBehaviour {
    // This is the subscriber's base
    private Dictionary<string, UnityEvent> m_AllSubscribers = new Dictionary<string, UnityEvent>();
    // This is where all the message will be at! Basically, <MessageName, stored variable>
    private Dictionary<string, object> m_NameStoredMessage = new Dictionary<string, object>();
    // This is to remove the event and variable name!
    private string ToRemoveTheEventVariable;
    // in order to keep track of updating the coroutine
    Coroutine removeVariableCoroutine;

    public static ObserverSystemScript Instance
    {
        get
        {
            if (instance == null)
            {
                // If it is yet to be awaken, awake it!
                FindObjectOfType<ObserverSystemScript>().Awake();
            }
            return instance;
        }
    }
    // The variable to store it!
    private static ObserverSystemScript instance;

    void Awake()
    {
        // Making sure there is only 1 instance of this script!
        if (instance != null && instance != this)
            Destroy(this);
        else
        {
            DontDestroyOnLoad(this);
            instance = this;
        }
    }

    private void OnDisable()
    {
        if (removeVariableCoroutine != null)
        {
            StopCoroutine(removeVariableCoroutine);
            removeVariableCoroutine = null;
        }
    }

    //void LateUpdate()
    //{
    //    if (ToRemoveTheEventVariable != null)
    //    {
    //        m_NameStoredMessage.Remove(ToRemoveTheEventVariable);
    //        ToRemoveTheEventVariable = null;
    //    }
    //}

    /// <summary>
    /// The coroutine to remove the event variable for the next frame.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator removeVariableRoutine()
    {
        yield return null;
        m_NameStoredMessage.Remove(ToRemoveTheEventVariable);
        ToRemoveTheEventVariable = null;
        removeVariableCoroutine = null;
        yield break;
    }

    /// <summary>
    /// To Subscribe to an event!
    /// </summary>
    /// <param name="eventName"> The event name </param>
    /// <param name="listenerFunction"> The function to be passed in. Make sure the return type is void! </param>
    public void SubscribeEvent(string eventName, UnityAction listenerFunction)
    {
        UnityEvent theEvent;
        // If can't find the event name, we create another one!
        if (!m_AllSubscribers.TryGetValue(eventName, out theEvent))
        {
            theEvent = new UnityEvent();
            m_AllSubscribers.Add(eventName, theEvent);
        }
        theEvent.AddListener(listenerFunction);
    }

    /// <summary>
    /// To unsubscribe from an event!
    /// </summary>
    /// <param name="eventName"> The event name to be unsubscribed from! </param>
    /// <param name="listenerFunction"> The Function to be removed from that event! </param>
    public void UnsubscribeEvent(string eventName, UnityAction listenerFunction)
    {
        UnityEvent theEvent;
        if (m_AllSubscribers.TryGetValue(eventName, out theEvent))
        {
            theEvent.RemoveListener(listenerFunction);
        }
    }

    /// <summary>
    /// The event to be triggered!
    /// </summary>
    /// <param name="eventName"> The event name to trigger! </param>
    public void TriggerEvent(string eventName)
    {
        UnityEvent theEvent;
        if (m_AllSubscribers.TryGetValue(eventName, out theEvent))
        {
            theEvent.Invoke();
        }
    }

    /// <summary>
    /// Remove the variable from the event!
    /// </summary>
    /// <param name="eventName">The event's name</param>
    public void removeStoredVariable(string eventName)
    {
        m_NameStoredMessage.Remove(eventName);
    }

    /// <summary>
    /// To store a variable in the event so that everyone can receive it easily!
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="storedVari"></param>
    /// <returns></returns>
    public bool storeVariableInEvent(string eventName, object storedVari)
    {
        if (m_NameStoredMessage.ContainsKey(eventName))
        {
            m_NameStoredMessage.Remove(eventName);
        }
        m_NameStoredMessage.Add(eventName, storedVari);
        return true;
    }

    /// <summary>
    /// This is to ensure that all variable can receive the stored variable from the event before it is removed in the next frame.
    /// Dont be selfish!
    /// </summary>
    /// <param name="eventName">The event name to remove the stored variable!</param>
    /// <returns></returns>
    public bool removeTheEventVariableNextFrame(string eventName)
    {
        if (ToRemoveTheEventVariable != null)
        {
            ToRemoveTheEventVariable = eventName;
            removeVariableCoroutine = StartCoroutine(removeVariableRoutine());
            return true;
        }
        return false;
    }

    /// <summary>
    /// To access the stored variable!
    /// </summary>
    /// <param name="eventName">The event name</param>
    /// <returns>returns null if no variable can be found!</returns>
    public object GetStoredEventVariable(string eventName)
    {
        object storedVariable;
        m_NameStoredMessage.TryGetValue(eventName, out storedVariable);
        return storedVariable;
    }

    public T GetStoredEventVariable<T>(string eventName)
    {
        return (T)GetStoredEventVariable(eventName);
    }

    /// <summary>
    /// This will remove all the event variables from the event
    /// </summary>
    public void RemoveAllEventVariable()
    {
        m_NameStoredMessage.Clear();
    }
}
