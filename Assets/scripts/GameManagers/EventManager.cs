using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    private static EventManager singleton = null;
    public static EventManager instance { get { return singleton; } }

    private Dictionary<string, UnityEvent> eventDictionary = new Dictionary<string, UnityEvent>();

    private void Awake()
    {
        if (singleton)
        {
            DestroyImmediate(this);
        }
        else
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void StartListening(string _eventName, UnityAction _listener)
    {
        UnityEvent _thisEvent = null;
        if (eventDictionary.TryGetValue(_eventName, out _thisEvent))
        {
            _thisEvent.AddListener(_listener);
        }
        else
        {
            _thisEvent = new UnityEvent();
            _thisEvent.AddListener(_listener);
            eventDictionary.Add(_eventName, _thisEvent);
        }
    }

    public void StopListening(string _eventName, UnityAction _listener)
    {
        UnityEvent _thisEvent = null;
        if (eventDictionary.TryGetValue(_eventName, out _thisEvent))
        {
            _thisEvent.RemoveListener(_listener);
        }
    }

    public void TriggerEvent(string _eventName)
    {
        UnityEvent _thisEvent = null;
        if (eventDictionary.TryGetValue(_eventName, out _thisEvent))
        {
            _thisEvent.Invoke();
        }
    }
}
