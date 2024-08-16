using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomGameEvent : UnityEvent<Component, object> { }

public class GameEventListener : MonoBehaviour
{
    public CustomGameEvent response;

    public GameEvent gameEvent;

    public void OnEnable()
    {
        gameEvent.AddListener(this);
    }

    public void OnDisable()
    {
        gameEvent.RemoveListener(this);
    }

    public void OnEventRaised(Component sender,object data)
    {
        response.Invoke(sender,data);
    }

}
