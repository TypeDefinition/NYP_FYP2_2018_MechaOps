using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// It is to follow the observer pattern. Something like subscribers!
/// Do always remember to unsubscribe at "Void OnDisable()" whenever you subscribe from another object!
/// </summary>
public class ObserverSystemScript : MonoBehaviour {

    /// <summary>
    /// This is the subscriber's base.
    /// I trust that no one will mess with this!
    /// </summary>
    public Dictionary<string, UnityEvent> m_AllSubscribers = new Dictionary<string, UnityEvent>();
    // This is where all the message will be at! Basically, <MessageName, stored variable>
    private Dictionary<string, object> m_NameStoredMessage = new Dictionary<string, object>();
    // This is to remove the event and variable name!
    private string m_ToRemoveTheEventVariable;
    // in order to keep track of updating the coroutine
    Coroutine m_RemoveVariableCoroutine;
    // The variable to store it!
    private static ObserverSystemScript m_Instance;

    public static ObserverSystemScript Instance
    {
        get
        {
            if (m_Instance == null)
            {
                // If it is yet to be awaken, awake it!
                FindObjectOfType<ObserverSystemScript>().Awake();
            }
            return m_Instance;
        }
    }
    

    void Awake()
    {
        // Making sure there is only 1 instance of this script!
        if (m_Instance != null && m_Instance != this)
            Destroy(this);
        else
        {
            DontDestroyOnLoad(this);
            m_Instance = this;
        }
    }

    private void OnDisable()
    {
        if (m_RemoveVariableCoroutine != null)
        {
            StopCoroutine(m_RemoveVariableCoroutine);
            m_RemoveVariableCoroutine = null;
        }
    }

    /// <summary>
    /// The coroutine to remove the event variable for the next frame.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator RemoveVariableRoutine()
    {
        yield return null;
        m_NameStoredMessage.Remove(m_ToRemoveTheEventVariable);
        m_ToRemoveTheEventVariable = null;
        m_RemoveVariableCoroutine = null;
        yield break;
    }

    /// <summary>
    /// To Subscribe to an event!
    /// </summary>
    /// <param name="_eventName"> The event name </param>
    /// <param name="_listenerFunction"> The function to be passed in. Make sure the return type is void! </param>
    public void SubscribeEvent(string _eventName, UnityAction _listenerFunction)
    {
        UnityEvent theEvent;
        // If can't find the event name, we create another one!
        if (!m_AllSubscribers.TryGetValue(_eventName, out theEvent))
        {
            theEvent = new UnityEvent();
            m_AllSubscribers.Add(_eventName, theEvent);
        }
        theEvent.AddListener(_listenerFunction);
    }

    /// <summary>
    /// To unsubscribe from an event!
    /// </summary>
    /// <param name="_eventName"> The event name to be unsubscribed from! </param>
    /// <param name="_listenerFunction"> The Function to be removed from that event! </param>
    public void UnsubscribeEvent(string _eventName, UnityAction _listenerFunction)
    {
        UnityEvent theEvent;
        if (m_AllSubscribers.TryGetValue(_eventName, out theEvent))
        {
            theEvent.RemoveListener(_listenerFunction);
        }
    }

    /// <summary>
    /// The event to be triggered!
    /// </summary>
    /// <param name="_eventName"> The event name to trigger! </param>
    public void TriggerEvent(string _eventName)
    {
        UnityEvent theEvent;
        if (m_AllSubscribers.TryGetValue(_eventName, out theEvent))
        {
            theEvent.Invoke();
        }
    }

    /// <summary>
    /// Remove the variable from the event!
    /// </summary>
    /// <param name="_eventName">The event's name</param>
    public void RemoveStoredVariable(string _eventName)
    {
        m_NameStoredMessage.Remove(_eventName);
    }

    /// <summary>
    /// To store a variable in the event so that everyone can receive it easily!
    /// </summary>
    /// <param name="_eventName"></param>
    /// <param name="_storedVari"></param>
    /// <returns></returns>
    public bool StoreVariableInEvent(string _eventName, object _storedVari)
    {
        if (m_NameStoredMessage.ContainsKey(_eventName))
        {
            m_NameStoredMessage.Remove(_eventName);
        }
        m_NameStoredMessage.Add(_eventName, _storedVari);
        return true;
    }

    /// <summary>
    /// This is to ensure that all variable can receive the stored variable from the event before it is removed in the next frame.
    /// Dont be selfish!
    /// </summary>
    /// <param name="_eventName">The event name to remove the stored variable!</param>
    /// <returns></returns>
    public bool RemoveTheEventVariableNextFrame(string _eventName)
    {
        if (m_ToRemoveTheEventVariable != null)
        {
            m_ToRemoveTheEventVariable = _eventName;
            m_RemoveVariableCoroutine = StartCoroutine(RemoveVariableRoutine());
            return true;
        }
        return false;
    }

    /// <summary>
    /// To access the stored variable!
    /// </summary>
    /// <param name="_eventName">The event name</param>
    /// <returns>returns null if no variable can be found!</returns>
    public object GetStoredEventVariable(string _eventName)
    {
        object storedVariable;
        m_NameStoredMessage.TryGetValue(_eventName, out storedVariable);
        return storedVariable;
    }

    /// <summary>
    /// Help to convert the StoredVariable into the type they want
    /// </summary>
    /// <typeparam name="T">The Type they want</typeparam>
    /// <param name="_eventName">The eventName</param>
    /// <returns>Return the converted type variable if successful</returns>
    public T GetStoredEventVariable<T>(string _eventName)
    {
        return (T)GetStoredEventVariable(_eventName);
    }

    /// <summary>
    /// This will remove all the event variables from the event
    /// </summary>
    public void RemoveAllEventVariable()
    {
        m_NameStoredMessage.Clear();
    }
}