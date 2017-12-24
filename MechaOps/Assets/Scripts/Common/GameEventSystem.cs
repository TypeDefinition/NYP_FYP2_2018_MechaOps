using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;

// Declare Delegates Here
public delegate void Void_Void();
public delegate void Void_String(string _string);
public delegate void Void_Char(char _char);
public delegate void Void_Short(short _short);
public delegate void Void_Int(int _int);
public delegate void Void_IntInt(int _int, int _int2);
public delegate void Void_Float(float _float);
public delegate void Void_Double(double _double);
public delegate void Void_Bool(bool _bool);
public delegate void Void_BoolBool(bool _bool, bool _bool2);

[System.Serializable]
public class GenericUnityEvent1<T> : UnityEvent<T> {}

[System.Serializable]
public class GenericUnityEvent2<T1, T2> : UnityEvent<T1, T2> {}

[System.Serializable]
public class GenericUnityEvent3<T1, T2, T3> : UnityEvent<T1, T2, T3> {}

[System.Serializable]
public class GenericUnityEvent4<T1, T2, T3, T4> : UnityEvent<T1, T2, T3, T4> {}

public class GameEventSystem
{
    // Singleton Instance
    private static GameEventSystem m_Instance = null;

    // Events
    private Dictionary<string, UnityEventBase> m_Events = new Dictionary<string, UnityEventBase>();

    // Constructor(s)
    private GameEventSystem() {}

    // Singleton Getter
    public static GameEventSystem GetInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = new GameEventSystem();
        }

        return m_Instance;
    }

    public bool HasEvent(string _eventName)
    {
        return m_Events.ContainsKey(_eventName);
    }

    // Cast the event manually.
    public UnityEventBase GetEvent(string _eventName)
    {
        UnityEventBase result = null;
        if (m_Events.TryGetValue(_eventName, out result))
        {
            return result;
        }

        return null;
    }

    // Walao eh, the number of times I need to copy pasta sibei chuey leh.
    public void AddEvent(string _eventName)
    {
        Assert.IsFalse(m_Events.ContainsKey(_eventName), MethodBase.GetCurrentMethod().Name + " - An event with this name already exist!");

        UnityEvent unityEvent = new UnityEvent();
        m_Events.Add(_eventName, unityEvent);
    }

    public void AddEvent<T>(string _eventName)
    {
        Assert.IsFalse(m_Events.ContainsKey(_eventName), MethodBase.GetCurrentMethod().Name + " - An event with this name already exist!");

        GenericUnityEvent1<T> unityEvent = new GenericUnityEvent1<T>();
        m_Events.Add(_eventName, unityEvent);
    }

    public void AddEvent<T1, T2>(string _eventName)
    {
        Assert.IsFalse(m_Events.ContainsKey(_eventName), MethodBase.GetCurrentMethod().Name + " - An event with this name already exist!");

        GenericUnityEvent2<T1, T2> unityEvent = new GenericUnityEvent2<T1, T2>();
        m_Events.Add(_eventName, unityEvent);
    }

    public void AddEvent<T1, T2, T3>(string _eventName)
    {
        Assert.IsFalse(m_Events.ContainsKey(_eventName), MethodBase.GetCurrentMethod().Name + " - An event with this name already exist!");

        GenericUnityEvent3<T1, T2, T3> unityEvent = new GenericUnityEvent3<T1, T2, T3>();
        m_Events.Add(_eventName, unityEvent);
    }

    public void AddEvent<T1, T2, T3, T4>(string _eventName)
    {
        Assert.IsFalse(m_Events.ContainsKey(_eventName), MethodBase.GetCurrentMethod().Name + " - An event with this name already exist!");

        GenericUnityEvent4<T1, T2, T3, T4> unityEvent = new GenericUnityEvent4<T1, T2, T3, T4>();
        m_Events.Add(_eventName, unityEvent);
    }

    public void SubscribeToEvent(string _eventName, UnityAction _unityAction)
    {
        UnityEventBase unityEventBase = GetEvent(_eventName);
        if (unityEventBase == null)
        {
            AddEvent(_eventName);
            unityEventBase = GetEvent(_eventName);
        }

        UnityEvent unityEvent = (UnityEvent)unityEventBase;
        Assert.IsFalse(unityEvent == null, MethodBase.GetCurrentMethod().Name + " - Wrong SubscribeToEvent function called! Ensure that you call the SubscribeToEvent function corresponding to the AddEvent function you called!");

        unityEvent.AddListener(_unityAction);
    }

    public void SubscribeToEvent<T>(string _eventName, UnityAction<T> _unityAction)
    {
        UnityEventBase unityEventBase = GetEvent(_eventName);
        if (unityEventBase == null)
        {
            AddEvent<T>(_eventName);
            unityEventBase = GetEvent(_eventName);
        }

        GenericUnityEvent1<T> unityEvent = (GenericUnityEvent1<T>)unityEventBase;
        Assert.IsFalse(unityEvent == null, MethodBase.GetCurrentMethod().Name + " - Wrong SubscribeToEvent function called! Ensure that you call the SubscribeToEvent function corresponding to the AddEvent function you called!");

        unityEvent.AddListener(_unityAction);
    }

    public void SubscribeToEvent<T1, T2>(string _eventName, UnityAction<T1, T2> _unityAction)
    {
        UnityEventBase unityEventBase = GetEvent(_eventName);
        if (unityEventBase == null)
        {
            AddEvent<T1, T2>(_eventName);
            unityEventBase = GetEvent(_eventName);
        }

        GenericUnityEvent2<T1, T2> unityEvent = (GenericUnityEvent2<T1, T2>)unityEventBase;
        Assert.IsFalse(unityEvent == null, MethodBase.GetCurrentMethod().Name + " - Wrong SubscribeToEvent function called! Ensure that you call the SubscribeToEvent function corresponding to the AddEvent function you called!");

        unityEvent.AddListener(_unityAction);
    }

    public void SubscribeToEvent<T1, T2, T3>(string _eventName, UnityAction<T1, T2, T3> _unityAction)
    {
        UnityEventBase unityEventBase = GetEvent(_eventName);
        if (unityEventBase == null)
        {
            AddEvent<T1, T2, T3>(_eventName);
            unityEventBase = GetEvent(_eventName);
        }

        GenericUnityEvent3<T1, T2, T3> unityEvent = (GenericUnityEvent3<T1, T2, T3>)unityEventBase;
        Assert.IsFalse(unityEvent == null, MethodBase.GetCurrentMethod().Name + " - Wrong SubscribeToEvent function called! Ensure that you call the SubscribeToEvent function corresponding to the AddEvent function you called!");

        unityEvent.AddListener(_unityAction);
    }

    public void SubscribeToEvent<T1, T2, T3, T4>(string _eventName, UnityAction<T1, T2, T3, T4> _unityAction)
    {
        UnityEventBase unityEventBase = GetEvent(_eventName);
        if (unityEventBase == null)
        {
            AddEvent<T1, T2, T3, T4>(_eventName);
            unityEventBase = GetEvent(_eventName);
        }

        GenericUnityEvent4<T1, T2, T3, T4> unityEvent = (GenericUnityEvent4<T1, T2, T3, T4>)unityEventBase;
        Assert.IsFalse(unityEvent == null, MethodBase.GetCurrentMethod().Name + " - Wrong SubscribeToEvent function called! Ensure that you call the SubscribeToEvent function corresponding to the AddEvent function you called!");

        unityEvent.AddListener(_unityAction);
    }

    public void UnsubscribeFromEvent(string _eventName, UnityAction _unityAction)
    {
        UnityEventBase unityEventBase = GetEvent(_eventName);
        Assert.IsFalse(unityEventBase == null, MethodBase.GetCurrentMethod().Name + " - Event not found!");

        UnityEvent unityEvent = (UnityEvent)unityEventBase;
        Assert.IsFalse(unityEvent == null, MethodBase.GetCurrentMethod().Name + " - Wrong UnsubscribeFromEvent function called!  Ensure that you call the UnsubscribeToEvent function corresponding to the AddEvent function you called! Ensure that you call the SubscribeToEvent function corresponding to the AddEvent function you called!");

        unityEvent.RemoveListener(_unityAction);
    }

    public void UnsubscribeFromEvent<T>(string _eventName, UnityAction<T> _unityAction)
    {
        UnityEventBase unityEventBase = GetEvent(_eventName);
        Assert.IsFalse(unityEventBase == null, MethodBase.GetCurrentMethod().Name + " - Event not found!");

        GenericUnityEvent1<T> unityEvent = (GenericUnityEvent1<T>)unityEventBase;
        Assert.IsFalse(unityEvent == null, MethodBase.GetCurrentMethod().Name + " - Wrong UnsubscribeFromEvent function called!  Ensure that you call the UnsubscribeToEvent function corresponding to the AddEvent function you called! Ensure that you call the SubscribeToEvent function corresponding to the AddEvent function you called!");

        unityEvent.RemoveListener(_unityAction);
    }

    public void UnsubscribeFromEvent<T1, T2>(string _eventName, UnityAction<T1, T2> _unityAction)
    {
        UnityEventBase unityEventBase = GetEvent(_eventName);
        Assert.IsFalse(unityEventBase == null, MethodBase.GetCurrentMethod().Name + " - Event not found!");

        GenericUnityEvent2<T1, T2> unityEvent = (GenericUnityEvent2<T1, T2>)unityEventBase;
        Assert.IsFalse(unityEvent == null, MethodBase.GetCurrentMethod().Name + " - Wrong UnsubscribeFromEvent function called!  Ensure that you call the UnsubscribeToEvent function corresponding to the AddEvent function you called! Ensure that you call the SubscribeToEvent function corresponding to the AddEvent function you called!");

        unityEvent.RemoveListener(_unityAction);
    }

    public void UnsubscribeFromEvent<T1, T2, T3>(string _eventName, UnityAction<T1, T2, T3> _unityAction)
    {
        UnityEventBase unityEventBase = GetEvent(_eventName);
        Assert.IsFalse(unityEventBase == null, MethodBase.GetCurrentMethod().Name + " - Event not found!");

        GenericUnityEvent3<T1, T2, T3> unityEvent = (GenericUnityEvent3<T1, T2, T3>)unityEventBase;
        Assert.IsFalse(unityEvent == null, MethodBase.GetCurrentMethod().Name + " - Wrong UnsubscribeFromEvent function called!  Ensure that you call the UnsubscribeToEvent function corresponding to the AddEvent function you called! Ensure that you call the SubscribeToEvent function corresponding to the AddEvent function you called!");

        unityEvent.RemoveListener(_unityAction);
    }

    public void UnsubscribeFromEvent<T1, T2, T3, T4>(string _eventName, UnityAction<T1, T2, T3, T4> _unityAction)
    {
        UnityEventBase unityEventBase = GetEvent(_eventName);
        Assert.IsFalse(unityEventBase == null, MethodBase.GetCurrentMethod().Name + " - Event not found!");

        GenericUnityEvent4<T1, T2, T3, T4> unityEvent = (GenericUnityEvent4<T1, T2, T3, T4>)unityEventBase;
        Assert.IsFalse(unityEvent == null, MethodBase.GetCurrentMethod().Name + " - Wrong UnsubscribeFromEvent function called!  Ensure that you call the UnsubscribeToEvent function corresponding to the AddEvent function you called! Ensure that you call the SubscribeToEvent function corresponding to the AddEvent function you called!");

        unityEvent.RemoveListener(_unityAction);
    }

    public void TriggerEvent(string _eventName)
    {
        UnityEventBase unityEventBase = GetEvent(_eventName);
        if (unityEventBase == null)
        {
            AddEvent(_eventName);
            unityEventBase = GetEvent(_eventName);
        }

        UnityEvent unityEvent = (UnityEvent)unityEventBase;
        Assert.IsFalse(unityEvent == null, MethodBase.GetCurrentMethod().Name + " - Wrong TriggerEvent function called! Ensure that you call the TriggerEvent function corresponding to the AddEvent function you called!");

        unityEvent.Invoke();
    }    

    public void TriggerEvent<T>(string _eventName, T _parameter)
    {
        UnityEventBase unityEventBase = GetEvent(_eventName);
        if (unityEventBase == null)
        {
            AddEvent<T>(_eventName);
            unityEventBase = GetEvent(_eventName);
        }

        GenericUnityEvent1<T> unityEvent = (GenericUnityEvent1<T>)unityEventBase;
        Assert.IsFalse(unityEvent == null, MethodBase.GetCurrentMethod().Name + " - Wrong TriggerEvent function called! Ensure that you call the TriggerEvent function corresponding to the AddEvent function you called!");

        unityEvent.Invoke(_parameter);
    }

    public void TriggerEvent<T1, T2>(string _eventName, T1 _parameter1, T2 _parameter2)
    {
        UnityEventBase unityEventBase = GetEvent(_eventName);
        if (unityEventBase == null)
        {
            AddEvent<T1, T2>(_eventName);
            unityEventBase = GetEvent(_eventName);
        }

        GenericUnityEvent2<T1, T2> unityEvent = (GenericUnityEvent2<T1, T2>)unityEventBase;
        Assert.IsFalse(unityEvent == null, MethodBase.GetCurrentMethod().Name + " - Wrong TriggerEvent function called! Ensure that you call the TriggerEvent function corresponding to the AddEvent function you called!");

        unityEvent.Invoke(_parameter1, _parameter2);
    }

    public void TriggerEvent<T1, T2, T3>(string _eventName, T1 _parameter1, T2 _parameter2, T3 _parameter3)
    {
        UnityEventBase unityEventBase = GetEvent(_eventName);
        if (unityEventBase == null)
        {
            AddEvent<T1, T2, T3>(_eventName);
            unityEventBase = GetEvent(_eventName);
        }

        GenericUnityEvent3<T1, T2, T3> unityEvent = (GenericUnityEvent3<T1, T2, T3>)unityEventBase;
        Assert.IsFalse(unityEvent == null, MethodBase.GetCurrentMethod().Name + " - Wrong TriggerEvent function called! Ensure that you call the TriggerEvent function corresponding to the AddEvent function you called!");

        unityEvent.Invoke(_parameter1, _parameter2, _parameter3);
    }

    public void TriggerEvent<T1, T2, T3, T4>(string _eventName, T1 _parameter1, T2 _parameter2, T3 _parameter3, T4 _parameter4)
    {
        UnityEventBase unityEventBase = GetEvent(_eventName);
        if (unityEventBase == null)
        {
            AddEvent<T1, T2, T3, T4>(_eventName);
            unityEventBase = GetEvent(_eventName);
        }

        GenericUnityEvent4<T1, T2, T3, T4> unityEvent = (GenericUnityEvent4<T1, T2, T3, T4>)unityEventBase;
        Assert.IsFalse(unityEvent == null, MethodBase.GetCurrentMethod().Name + " - Wrong TriggerEvent function called! Ensure that you call the TriggerEvent function corresponding to the AddEvent function you called!");

        unityEvent.Invoke(_parameter1, _parameter2, _parameter3, _parameter4);
    }

}